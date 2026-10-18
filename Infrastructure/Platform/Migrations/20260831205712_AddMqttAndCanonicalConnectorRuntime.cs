using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddMqttAndCanonicalConnectorRuntime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION integrations.list_mqtt_subscriptions()
                RETURNS TABLE(
                    tenant_id uuid,
                    connection_id uuid,
                    feed_id uuid,
                    farm_id uuid,
                    connection_version bigint,
                    feed_version bigint,
                    credential_id uuid)
                LANGUAGE sql
                SECURITY DEFINER
                STABLE
                SET search_path = pg_catalog, integrations
                AS $function$
                    SELECT connection.tenant_id,
                           connection.id,
                           feed.id,
                           feed.farm_id,
                           connection.version,
                           feed.version,
                           credential.id
                    FROM integrations.connections connection
                    JOIN integrations.feeds feed
                      ON feed.tenant_id = connection.tenant_id
                     AND feed.connection_id = connection.id
                    JOIN LATERAL (
                        SELECT value.id
                        FROM integrations.credential_versions value
                        WHERE value.tenant_id = connection.tenant_id
                          AND value.connection_id = connection.id
                          AND value.status = 1
                          AND value.valid_from <= CURRENT_TIMESTAMP
                          AND (value.expires_at IS NULL OR value.expires_at > CURRENT_TIMESTAMP)
                        ORDER BY value.credential_version DESC
                        LIMIT 1
                    ) credential ON true
                    WHERE connection.status = 2
                      AND feed.status = 1
                      AND feed.feed_type IN (3, 4)
                      AND connection.category IN (3, 4)
                      AND connection.provider_code IN ('mqtt', 'the-things-stack');
                $function$;
                REVOKE ALL ON FUNCTION integrations.list_mqtt_subscriptions() FROM PUBLIC;
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT EXECUTE ON FUNCTION integrations.list_mqtt_subscriptions() TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS integrations.list_mqtt_subscriptions();");
        }
    }
}
