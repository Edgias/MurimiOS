using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddReliableOutboxDispatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_processed_at_occurred_at",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "available_at",
                schema: "messaging",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE messaging.outbox_messages
                SET available_at = occurred_at
                WHERE available_at IS NULL;

                ALTER TABLE messaging.outbox_messages
                ALTER COLUMN available_at SET NOT NULL;
                """);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "dead_lettered_at",
                schema: "messaging",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "lock_id",
                schema: "messaging",
                table: "outbox_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "locked_until",
                schema: "messaging",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_dispatch",
                schema: "messaging",
                table: "outbox_messages",
                columns: new[] { "processed_at", "dead_lettered_at", "available_at", "locked_until", "occurred_at" });

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION messaging.claim_outbox(
                    p_worker_id uuid,
                    p_batch_size integer,
                    p_now timestamp with time zone,
                    p_lock_seconds integer)
                RETURNS TABLE (
                    id uuid,
                    tenant_id uuid,
                    type text,
                    payload text,
                    occurred_at timestamp with time zone,
                    attempts integer,
                    lock_id uuid)
                LANGUAGE sql
                SECURITY DEFINER
                SET search_path = pg_catalog, messaging
                AS $function$
                    WITH candidates AS (
                        SELECT candidate.id
                        FROM messaging.outbox_messages AS candidate
                        WHERE candidate.processed_at IS NULL
                          AND candidate.dead_lettered_at IS NULL
                          AND candidate.available_at <= p_now
                          AND (candidate.locked_until IS NULL OR candidate.locked_until <= p_now)
                        ORDER BY candidate.occurred_at, candidate.id
                        FOR UPDATE SKIP LOCKED
                        LIMIT LEAST(GREATEST(p_batch_size, 1), 500)
                    )
                    UPDATE messaging.outbox_messages AS message
                    SET lock_id = p_worker_id,
                        locked_until = p_now + make_interval(secs => GREATEST(p_lock_seconds, 1)),
                        attempts = message.attempts + 1
                    FROM candidates
                    WHERE message.id = candidates.id
                    RETURNING message.id,
                              message.tenant_id,
                              message.type,
                              message.payload::text,
                              message.occurred_at,
                              message.attempts,
                              message.lock_id;
                $function$;

                CREATE OR REPLACE FUNCTION messaging.complete_outbox(
                    p_message_id uuid,
                    p_lock_id uuid,
                    p_processed_at timestamp with time zone)
                RETURNS boolean
                LANGUAGE plpgsql
                SECURITY DEFINER
                SET search_path = pg_catalog, messaging
                AS $function$
                BEGIN
                    UPDATE messaging.outbox_messages
                    SET processed_at = p_processed_at,
                        last_error = NULL,
                        lock_id = NULL,
                        locked_until = NULL
                    WHERE id = p_message_id
                      AND lock_id = p_lock_id
                      AND processed_at IS NULL
                      AND dead_lettered_at IS NULL;
                    RETURN FOUND;
                END;
                $function$;

                CREATE OR REPLACE FUNCTION messaging.fail_outbox(
                    p_message_id uuid,
                    p_lock_id uuid,
                    p_failed_at timestamp with time zone,
                    p_retry_at timestamp with time zone,
                    p_error text,
                    p_maximum_attempts integer)
                RETURNS boolean
                LANGUAGE plpgsql
                SECURITY DEFINER
                SET search_path = pg_catalog, messaging
                AS $function$
                BEGIN
                    UPDATE messaging.outbox_messages
                    SET last_error = LEFT(COALESCE(NULLIF(BTRIM(p_error), ''), 'Unknown publishing failure.'), 2000),
                        lock_id = NULL,
                        locked_until = NULL,
                        dead_lettered_at = CASE
                            WHEN attempts >= GREATEST(p_maximum_attempts, 1) THEN p_failed_at
                            ELSE NULL
                        END,
                        available_at = CASE
                            WHEN attempts >= GREATEST(p_maximum_attempts, 1) THEN available_at
                            ELSE p_retry_at
                        END
                    WHERE id = p_message_id
                      AND lock_id = p_lock_id
                      AND processed_at IS NULL
                      AND dead_lettered_at IS NULL;
                    RETURN FOUND;
                END;
                $function$;

                REVOKE ALL ON FUNCTION messaging.claim_outbox(uuid, integer, timestamp with time zone, integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION messaging.complete_outbox(uuid, uuid, timestamp with time zone) FROM PUBLIC;
                REVOKE ALL ON FUNCTION messaging.fail_outbox(uuid, uuid, timestamp with time zone, timestamp with time zone, text, integer) FROM PUBLIC;

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT EXECUTE ON FUNCTION messaging.claim_outbox(uuid, integer, timestamp with time zone, integer) TO murimios_app;
                        GRANT EXECUTE ON FUNCTION messaging.complete_outbox(uuid, uuid, timestamp with time zone) TO murimios_app;
                        GRANT EXECUTE ON FUNCTION messaging.fail_outbox(uuid, uuid, timestamp with time zone, timestamp with time zone, text, integer) TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS messaging.fail_outbox(uuid, uuid, timestamp with time zone, timestamp with time zone, text, integer);
                DROP FUNCTION IF EXISTS messaging.complete_outbox(uuid, uuid, timestamp with time zone);
                DROP FUNCTION IF EXISTS messaging.claim_outbox(uuid, integer, timestamp with time zone, integer);
                """);

            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_dispatch",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "available_at",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "dead_lettered_at",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "lock_id",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "locked_until",
                schema: "messaging",
                table: "outbox_messages");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_at_occurred_at",
                schema: "messaging",
                table: "outbox_messages",
                columns: new[] { "processed_at", "occurred_at" });
        }
    }
}
