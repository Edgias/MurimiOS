using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplyNetworkAndDataGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "supply");

            migrationBuilder.CreateTable(
                name: "relationships",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producer_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_by_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    responded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relationships", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "data_grants",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grant_series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producer_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    purpose = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    entity_categories = table.Column<string[]>(type: "text[]", nullable: false),
                    attributes = table.Column<string[]>(type: "text[]", nullable: false),
                    farm_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    season_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    commodities = table.Column<string[]>(type: "text[]", nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    downstream_disclosure_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    grant_version = table.Column<int>(type: "integer", nullable: false),
                    previous_grant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_grants", x => x.id);
                    table.ForeignKey(
                        name: "fk_data_grants_data_grants_previous_grant_id",
                        column: x => x.previous_grant_id,
                        principalSchema: "supply",
                        principalTable: "data_grants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_data_grants_relationships_relationship_id",
                        column: x => x.relationship_id,
                        principalSchema: "supply",
                        principalTable: "relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "access_audit",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    producer_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    purpose = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    entity_category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    result_count = table.Column<int>(type: "integer", nullable: false),
                    outcome = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_audit", x => x.id);
                    table.ForeignKey(
                        name: "fk_access_audit_data_grants_grant_id",
                        column: x => x.grant_id,
                        principalSchema: "supply",
                        principalTable: "data_grants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_audit_grant_id_occurred_at",
                schema: "supply",
                table: "access_audit",
                columns: new[] { "grant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_access_audit_producer_tenant_id_occurred_at",
                schema: "supply",
                table: "access_audit",
                columns: new[] { "producer_tenant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_access_audit_recipient_tenant_id_occurred_at",
                schema: "supply",
                table: "access_audit",
                columns: new[] { "recipient_tenant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_data_grants_grant_series_id_grant_version",
                schema: "supply",
                table: "data_grants",
                columns: new[] { "grant_series_id", "grant_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_data_grants_previous_grant_id",
                schema: "supply",
                table: "data_grants",
                column: "previous_grant_id");

            migrationBuilder.CreateIndex(
                name: "ix_data_grants_producer_tenant_id_recipient_tenant_id_status_v",
                schema: "supply",
                table: "data_grants",
                columns: new[] { "producer_tenant_id", "recipient_tenant_id", "status", "valid_from", "valid_to" });

            migrationBuilder.CreateIndex(
                name: "ix_data_grants_relationship_id",
                schema: "supply",
                table: "data_grants",
                column: "relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_relationships_buyer_tenant_id_status_requested_at",
                schema: "supply",
                table: "relationships",
                columns: new[] { "buyer_tenant_id", "status", "requested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_relationships_producer_tenant_id_buyer_tenant_id_status",
                schema: "supply",
                table: "relationships",
                columns: new[] { "producer_tenant_id", "buyer_tenant_id", "status" });

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX ux_supply_relationships_open_parties
                    ON supply.relationships (producer_tenant_id, buyer_tenant_id)
                    WHERE status IN (1, 2);

                ALTER TABLE supply.relationships
                    ADD CONSTRAINT ck_supply_relationship_parties_distinct
                        CHECK (producer_tenant_id <> buyer_tenant_id),
                    ADD CONSTRAINT ck_supply_relationship_requester_party
                        CHECK (requested_by_tenant_id IN (producer_tenant_id, buyer_tenant_id));

                ALTER TABLE supply.data_grants
                    ADD CONSTRAINT ck_supply_data_grant_parties_distinct
                        CHECK (producer_tenant_id <> recipient_tenant_id),
                    ADD CONSTRAINT ck_supply_data_grant_validity
                        CHECK (valid_to IS NULL OR valid_to > valid_from),
                    ADD CONSTRAINT ck_supply_data_grant_version
                        CHECK (grant_version > 0),
                    ADD CONSTRAINT ck_supply_data_grant_farms
                        CHECK (cardinality(farm_ids) > 0),
                    ADD CONSTRAINT ck_supply_data_grant_categories
                        CHECK (cardinality(entity_categories) > 0),
                    ADD CONSTRAINT ck_supply_data_grant_attributes
                        CHECK (cardinality(attributes) > 0);

                ALTER TABLE supply.access_audit
                    ADD CONSTRAINT ck_supply_access_audit_actor_party
                        CHECK (actor_tenant_id IN (producer_tenant_id, recipient_tenant_id)),
                    ADD CONSTRAINT ck_supply_access_audit_result_count
                        CHECK (result_count >= 0);

                ALTER TABLE supply.relationships ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.relationships FORCE ROW LEVEL SECURITY;
                CREATE POLICY relationship_party_select ON supply.relationships
                    FOR SELECT USING (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        OR buyer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                CREATE POLICY relationship_request_insert ON supply.relationships
                    FOR INSERT WITH CHECK (
                        requested_by_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        AND requested_by_tenant_id IN (producer_tenant_id, buyer_tenant_id));
                CREATE POLICY relationship_party_update ON supply.relationships
                    FOR UPDATE USING (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        OR buyer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        OR buyer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE supply.data_grants ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.data_grants FORCE ROW LEVEL SECURITY;
                CREATE POLICY data_grant_party_select ON supply.data_grants
                    FOR SELECT USING (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        OR recipient_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                CREATE POLICY data_grant_producer_insert ON supply.data_grants
                    FOR INSERT WITH CHECK (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                CREATE POLICY data_grant_producer_update ON supply.data_grants
                    FOR UPDATE USING (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE supply.access_audit ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.access_audit FORCE ROW LEVEL SECURITY;
                CREATE POLICY access_audit_party_select ON supply.access_audit
                    FOR SELECT USING (
                        producer_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        OR recipient_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                CREATE POLICY access_audit_actor_insert ON supply.access_audit
                    FOR INSERT WITH CHECK (
                        actor_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                        AND actor_tenant_id IN (producer_tenant_id, recipient_tenant_id));

                CREATE OR REPLACE FUNCTION supply.enforce_relationship_transition()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, supply
                AS $function$
                DECLARE
                    actor_tenant uuid := nullif(current_setting('app.tenant_id', true), '')::uuid;
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Supply relationships are retained and cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['status', 'responded_at', 'revoked_at', 'revoked_by_tenant_id', 'revocation_reason', 'updated_at_utc', 'version'])
                        IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['status', 'responded_at', 'revoked_at', 'revoked_by_tenant_id', 'revocation_reason', 'updated_at_utc', 'version']) THEN
                        RAISE EXCEPTION 'Supply relationship identity and request details are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF NEW.version <> OLD.version + 1 OR NEW.updated_at_utc < OLD.updated_at_utc THEN
                        RAISE EXCEPTION 'Supply relationship version transition is invalid.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status = 1 AND NEW.status IN (2, 3) THEN
                        IF actor_tenant IS NULL OR actor_tenant = OLD.requested_by_tenant_id
                           OR actor_tenant NOT IN (OLD.producer_tenant_id, OLD.buyer_tenant_id)
                           OR NEW.responded_at IS NULL THEN
                            RAISE EXCEPTION 'Only the counterparty can accept or reject a pending relationship.' USING ERRCODE = '42501';
                        END IF;
                    ELSIF OLD.status = 2 AND NEW.status = 4 THEN
                        IF actor_tenant IS NULL OR actor_tenant NOT IN (OLD.producer_tenant_id, OLD.buyer_tenant_id)
                           OR NEW.revoked_at IS NULL OR NEW.revoked_by_tenant_id <> actor_tenant
                           OR nullif(btrim(NEW.revocation_reason), '') IS NULL THEN
                            RAISE EXCEPTION 'Only a relationship party can revoke an active relationship with a reason.' USING ERRCODE = '42501';
                        END IF;
                    ELSE
                        RAISE EXCEPTION 'Invalid supply relationship state transition.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $function$;

                CREATE TRIGGER supply_relationship_transition_guard
                    BEFORE UPDATE OR DELETE ON supply.relationships
                    FOR EACH ROW EXECUTE FUNCTION supply.enforce_relationship_transition();

                CREATE OR REPLACE FUNCTION supply.enforce_data_grant_transition()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, supply
                AS $function$
                DECLARE
                    actor_tenant uuid := nullif(current_setting('app.tenant_id', true), '')::uuid;
                    relationship_row supply.relationships%ROWTYPE;
                    previous_row supply.data_grants%ROWTYPE;
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Data grants are retained and cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'INSERT' THEN
                        IF actor_tenant IS NULL OR NEW.producer_tenant_id <> actor_tenant OR NEW.status <> 1 THEN
                            RAISE EXCEPTION 'Only a producer can create an active data grant.' USING ERRCODE = '42501';
                        END IF;
                        SELECT * INTO relationship_row FROM supply.relationships WHERE id = NEW.relationship_id;
                        IF NOT FOUND OR relationship_row.status <> 2
                           OR relationship_row.producer_tenant_id <> NEW.producer_tenant_id
                           OR relationship_row.buyer_tenant_id <> NEW.recipient_tenant_id THEN
                            RAISE EXCEPTION 'A data grant requires a matching active supply relationship.' USING ERRCODE = '23514';
                        END IF;
                        IF NEW.previous_grant_id IS NULL THEN
                            IF NEW.grant_version <> 1 THEN
                                RAISE EXCEPTION 'The first data grant version must be one.' USING ERRCODE = '23514';
                            END IF;
                        ELSE
                            SELECT * INTO previous_row FROM supply.data_grants WHERE id = NEW.previous_grant_id;
                            IF NOT FOUND OR previous_row.relationship_id <> NEW.relationship_id
                               OR previous_row.grant_series_id <> NEW.grant_series_id
                               OR NEW.grant_version <> previous_row.grant_version + 1 THEN
                                RAISE EXCEPTION 'Data grant version lineage is invalid.' USING ERRCODE = '23514';
                            END IF;
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['status', 'revoked_at', 'revoked_by_tenant_id', 'revocation_reason', 'updated_at_utc', 'version'])
                        IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['status', 'revoked_at', 'revoked_by_tenant_id', 'revocation_reason', 'updated_at_utc', 'version'])
                       OR OLD.status <> 1 OR NEW.status <> 2
                       OR actor_tenant IS NULL OR actor_tenant <> OLD.producer_tenant_id
                       OR NEW.revoked_at IS NULL OR NEW.revoked_by_tenant_id <> actor_tenant
                       OR nullif(btrim(NEW.revocation_reason), '') IS NULL
                       OR NEW.version <> OLD.version + 1 OR NEW.updated_at_utc < OLD.updated_at_utc THEN
                        RAISE EXCEPTION 'Only the producer can revoke an active data grant; grant definitions are versioned, not edited.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $function$;

                CREATE TRIGGER supply_data_grant_transition_guard
                    BEFORE INSERT OR UPDATE OR DELETE ON supply.data_grants
                    FOR EACH ROW EXECUTE FUNCTION supply.enforce_data_grant_transition();

                CREATE TRIGGER supply_access_audit_immutable
                    BEFORE UPDATE OR DELETE ON supply.access_audit
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                CREATE OR REPLACE FUNCTION supply.read_granted_harvest_lots(p_grant_id uuid)
                RETURNS TABLE (
                    grant_id uuid,
                    producer_tenant_id uuid,
                    harvest_lot_id uuid,
                    farm_id uuid,
                    season_id uuid,
                    lot_code text,
                    crop text,
                    variety text,
                    quantity numeric,
                    unit text,
                    harvested_at timestamp with time zone)
                LANGUAGE sql
                STABLE
                SECURITY DEFINER
                SET search_path = pg_catalog, supply, production
                AS $function$
                    WITH authorized_grant AS (
                        SELECT data_grant.*
                        FROM supply.data_grants AS data_grant
                        JOIN supply.relationships AS relationship ON relationship.id = data_grant.relationship_id
                        WHERE data_grant.id = p_grant_id
                          AND data_grant.recipient_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                          AND data_grant.status = 1
                          AND relationship.status = 2
                          AND data_grant.valid_from <= statement_timestamp()
                          AND (data_grant.valid_to IS NULL OR data_grant.valid_to > statement_timestamp())
                          AND 'harvest-lots' = ANY(data_grant.entity_categories)
                    )
                    SELECT data_grant.id,
                           data_grant.producer_tenant_id,
                           harvest.id,
                           season.farm_id,
                           season.id,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'lotCode' = ANY(data_grant.attributes) THEN harvest.lot_code END,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'crop' = ANY(data_grant.attributes) THEN zone.crop END,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'variety' = ANY(data_grant.attributes) THEN zone.variety END,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'quantity' = ANY(data_grant.attributes) THEN harvest.quantity END,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'unit' = ANY(data_grant.attributes) THEN harvest.unit END,
                           CASE WHEN '*' = ANY(data_grant.attributes) OR 'harvestedAt' = ANY(data_grant.attributes) THEN harvest.harvested_at END
                    FROM authorized_grant AS data_grant
                    JOIN production.harvest_lots AS harvest ON harvest.tenant_id = data_grant.producer_tenant_id
                    JOIN production.crop_zones AS zone
                      ON zone.tenant_id = harvest.tenant_id AND zone.id = harvest.crop_zone_id
                    JOIN production.seasons AS season
                      ON season.tenant_id = zone.tenant_id AND season.id = zone.season_id
                    WHERE season.farm_id = ANY(data_grant.farm_ids)
                      AND (cardinality(data_grant.season_ids) = 0 OR season.id = ANY(data_grant.season_ids))
                      AND (cardinality(data_grant.commodities) = 0 OR zone.crop = ANY(data_grant.commodities));
                $function$;

                CREATE OR REPLACE FUNCTION supply.read_granted_trace_links(p_grant_id uuid, p_root_harvest_id uuid)
                RETURNS TABLE (
                    grant_id uuid,
                    producer_tenant_id uuid,
                    link_id uuid,
                    event_id uuid,
                    from_type integer,
                    from_id uuid,
                    to_type integer,
                    to_id uuid,
                    relationship integer,
                    depth integer)
                LANGUAGE sql
                STABLE
                SECURITY DEFINER
                SET search_path = pg_catalog, supply, production, traceability
                AS $function$
                    WITH RECURSIVE authorized_root AS (
                        SELECT data_grant.id AS grant_id, data_grant.producer_tenant_id, harvest.id AS harvest_lot_id
                        FROM supply.data_grants AS data_grant
                        JOIN supply.relationships AS supply_relationship ON supply_relationship.id = data_grant.relationship_id
                        JOIN production.harvest_lots AS harvest
                          ON harvest.tenant_id = data_grant.producer_tenant_id AND harvest.id = p_root_harvest_id
                        JOIN production.crop_zones AS zone
                          ON zone.tenant_id = harvest.tenant_id AND zone.id = harvest.crop_zone_id
                        JOIN production.seasons AS season
                          ON season.tenant_id = zone.tenant_id AND season.id = zone.season_id
                        WHERE data_grant.id = p_grant_id
                          AND data_grant.recipient_tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid
                          AND data_grant.status = 1
                          AND supply_relationship.status = 2
                          AND data_grant.valid_from <= statement_timestamp()
                          AND (data_grant.valid_to IS NULL OR data_grant.valid_to > statement_timestamp())
                          AND 'traceability' = ANY(data_grant.entity_categories)
                          AND season.farm_id = ANY(data_grant.farm_ids)
                          AND (cardinality(data_grant.season_ids) = 0 OR season.id = ANY(data_grant.season_ids))
                          AND (cardinality(data_grant.commodities) = 0 OR zone.crop = ANY(data_grant.commodities))
                    ), graph AS (
                        SELECT link.id AS link_id, link.event_id, link.from_type, link.from_id,
                               link.to_type, link.to_id, link.relationship, 1 AS depth,
                               ARRAY[link.from_type::text || ':' || link.from_id::text,
                                     link.to_type::text || ':' || link.to_id::text] AS visited
                        FROM authorized_root AS root
                        JOIN traceability.links AS link
                          ON link.tenant_id = root.producer_tenant_id
                         AND link.from_type = 2
                         AND link.from_id = root.harvest_lot_id
                        UNION ALL
                        SELECT link.id, link.event_id, link.from_type, link.from_id,
                               link.to_type, link.to_id, link.relationship, graph.depth + 1,
                               graph.visited || (link.to_type::text || ':' || link.to_id::text)
                        FROM graph
                        JOIN authorized_root AS root ON true
                        JOIN traceability.links AS link
                          ON link.tenant_id = root.producer_tenant_id
                         AND link.from_type = graph.to_type
                         AND link.from_id = graph.to_id
                        WHERE graph.depth < 25
                          AND NOT ((link.to_type::text || ':' || link.to_id::text) = ANY(graph.visited))
                    )
                    SELECT root.grant_id, root.producer_tenant_id, graph.link_id, graph.event_id,
                           graph.from_type, graph.from_id, graph.to_type, graph.to_id,
                           graph.relationship, graph.depth
                    FROM authorized_root AS root
                    JOIN graph ON true;
                $function$;

                REVOKE ALL ON FUNCTION supply.read_granted_harvest_lots(uuid) FROM PUBLIC;
                REVOKE ALL ON FUNCTION supply.read_granted_trace_links(uuid, uuid) FROM PUBLIC;

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA supply TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE ON supply.relationships, supply.data_grants TO murimios_app;
                        GRANT SELECT, INSERT ON supply.access_audit TO murimios_app;
                        REVOKE DELETE ON supply.relationships, supply.data_grants, supply.access_audit FROM murimios_app;
                        REVOKE UPDATE ON supply.access_audit FROM murimios_app;
                        GRANT EXECUTE ON FUNCTION supply.read_granted_harvest_lots(uuid) TO murimios_app;
                        GRANT EXECUTE ON FUNCTION supply.read_granted_trace_links(uuid, uuid) TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS supply.read_granted_trace_links(uuid, uuid);
                DROP FUNCTION IF EXISTS supply.read_granted_harvest_lots(uuid);
                DROP FUNCTION IF EXISTS supply.enforce_data_grant_transition();
                DROP FUNCTION IF EXISTS supply.enforce_relationship_transition();
                """);

            migrationBuilder.DropTable(
                name: "access_audit",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "data_grants",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "relationships",
                schema: "supply");
        }
    }
}
