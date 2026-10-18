using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddLivestockProductionExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "biological_profiles",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sex = table.Column<int>(type: "integer", nullable: false),
                    born_on = table.Column<DateOnly>(type: "date", nullable: true),
                    breed = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    dam_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sire_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_biological_profiles", x => x.id);
                    table.ForeignKey(
                        name: "fk_biological_profiles_subjects_tenant_id_dam_id",
                        columns: x => new { x.tenant_id, x.dam_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_biological_profiles_subjects_tenant_id_sire_id",
                        columns: x => new { x.tenant_id, x.sire_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_biological_profiles_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "grazing_plans",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    holding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    target_head_count = table.Column<int>(type: "integer", nullable: false),
                    rest_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    available_dry_matter_kg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grazing_plans", x => x.id);
                    table.ForeignKey(
                        name: "fk_grazing_plans_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_grazing_plans_holdings_tenant_id_holding_id",
                        columns: x => new { x.tenant_id, x.holding_id },
                        principalSchema: "livestock",
                        principalTable: "holdings",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_grazing_plans_subjects_tenant_id_group_subject_id",
                        columns: x => new { x.tenant_id, x.group_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "milk_records",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    milked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    session = table.Column<int>(type: "integer", nullable: false),
                    quantity_litres = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    discarded_litres = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    saleable_litres = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_discarded_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    withdrawal_hold_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fat_percent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    protein_percent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: true),
                    quality_grade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_milk_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_milk_records_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_milk_records_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ration_plans",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    target_head_count = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ration_plans", x => x.id);
                    table.UniqueConstraint("ak_ration_plans_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_ration_plans_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ration_plans_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reproduction_events",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    outcome = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    related_subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    breeding_event_id = table.Column<Guid>(type: "uuid", nullable: true),
                    method = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    expected_due_on = table.Column<DateOnly>(type: "date", nullable: true),
                    offspring_count = table.Column<int>(type: "integer", nullable: true),
                    weight_kg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    entered_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    entered_weight_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    practitioner = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reproduction_events", x => x.id);
                    table.UniqueConstraint("ak_reproduction_events_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_reproduction_events_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reproduction_events_reproduction_events_tenant_id_breeding_",
                        columns: x => new { x.tenant_id, x.breeding_event_id },
                        principalSchema: "livestock",
                        principalTable: "reproduction_events",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reproduction_events_subjects_tenant_id_related_subject_id",
                        columns: x => new { x.tenant_id, x.related_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reproduction_events_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ration_lines",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    feed_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity_kg_per_head_per_day = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dry_matter_percent = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ration_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_ration_lines_feed_batches_tenant_id_feed_batch_id",
                        columns: x => new { x.tenant_id, x.feed_batch_id },
                        principalSchema: "livestock",
                        principalTable: "feed_batches",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ration_lines_ration_plans_tenant_id_plan_id",
                        columns: x => new { x.tenant_id, x.plan_id },
                        principalSchema: "livestock",
                        principalTable: "ration_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "offspring_links",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parturition_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    offspring_subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    outcome = table.Column<int>(type: "integer", nullable: false),
                    birth_weight_kg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    entered_birth_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    entered_birth_weight_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_offspring_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_offspring_links_reproduction_events_tenant_id_parturition_e",
                        columns: x => new { x.tenant_id, x.parturition_event_id },
                        principalSchema: "livestock",
                        principalTable: "reproduction_events",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_offspring_links_subjects_tenant_id_offspring_subject_id",
                        columns: x => new { x.tenant_id, x.offspring_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_biological_profiles_tenant_id",
                schema: "livestock",
                table: "biological_profiles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_biological_profiles_tenant_id_dam_id",
                schema: "livestock",
                table: "biological_profiles",
                columns: new[] { "tenant_id", "dam_id" });

            migrationBuilder.CreateIndex(
                name: "ix_biological_profiles_tenant_id_sire_id",
                schema: "livestock",
                table: "biological_profiles",
                columns: new[] { "tenant_id", "sire_id" });

            migrationBuilder.CreateIndex(
                name: "ix_biological_profiles_tenant_id_subject_id",
                schema: "livestock",
                table: "biological_profiles",
                columns: new[] { "tenant_id", "subject_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_grazing_plans_tenant_id",
                schema: "livestock",
                table: "grazing_plans",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_grazing_plans_tenant_id_farm_id_starts_at_ends_at",
                schema: "livestock",
                table: "grazing_plans",
                columns: new[] { "tenant_id", "farm_id", "starts_at", "ends_at" });

            migrationBuilder.CreateIndex(
                name: "ix_grazing_plans_tenant_id_group_subject_id",
                schema: "livestock",
                table: "grazing_plans",
                columns: new[] { "tenant_id", "group_subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_grazing_plans_tenant_id_holding_id_status_starts_at_ends_at",
                schema: "livestock",
                table: "grazing_plans",
                columns: new[] { "tenant_id", "holding_id", "status", "starts_at", "ends_at" });

            migrationBuilder.CreateIndex(
                name: "ix_milk_records_tenant_id",
                schema: "livestock",
                table: "milk_records",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_milk_records_tenant_id_farm_id_milked_at",
                schema: "livestock",
                table: "milk_records",
                columns: new[] { "tenant_id", "farm_id", "milked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_milk_records_tenant_id_subject_id_milked_at",
                schema: "livestock",
                table: "milk_records",
                columns: new[] { "tenant_id", "subject_id", "milked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_offspring_links_tenant_id",
                schema: "livestock",
                table: "offspring_links",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_offspring_links_tenant_id_offspring_subject_id",
                schema: "livestock",
                table: "offspring_links",
                columns: new[] { "tenant_id", "offspring_subject_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_offspring_links_tenant_id_parturition_event_id_offspring_su",
                schema: "livestock",
                table: "offspring_links",
                columns: new[] { "tenant_id", "parturition_event_id", "offspring_subject_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ration_lines_tenant_id",
                schema: "livestock",
                table: "ration_lines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ration_lines_tenant_id_feed_batch_id",
                schema: "livestock",
                table: "ration_lines",
                columns: new[] { "tenant_id", "feed_batch_id" });

            migrationBuilder.CreateIndex(
                name: "ix_ration_lines_tenant_id_plan_id_feed_name",
                schema: "livestock",
                table: "ration_lines",
                columns: new[] { "tenant_id", "plan_id", "feed_name" });

            migrationBuilder.CreateIndex(
                name: "ix_ration_plans_tenant_id",
                schema: "livestock",
                table: "ration_plans",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ration_plans_tenant_id_farm_id_effective_from_effective_to",
                schema: "livestock",
                table: "ration_plans",
                columns: new[] { "tenant_id", "farm_id", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_ration_plans_tenant_id_subject_id_status_effective_from",
                schema: "livestock",
                table: "ration_plans",
                columns: new[] { "tenant_id", "subject_id", "status", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "ix_reproduction_events_tenant_id",
                schema: "livestock",
                table: "reproduction_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_reproduction_events_tenant_id_breeding_event_id",
                schema: "livestock",
                table: "reproduction_events",
                columns: new[] { "tenant_id", "breeding_event_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reproduction_events_tenant_id_farm_id_type_occurred_at",
                schema: "livestock",
                table: "reproduction_events",
                columns: new[] { "tenant_id", "farm_id", "type", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_reproduction_events_tenant_id_related_subject_id",
                schema: "livestock",
                table: "reproduction_events",
                columns: new[] { "tenant_id", "related_subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_reproduction_events_tenant_id_subject_id_occurred_at",
                schema: "livestock",
                table: "reproduction_events",
                columns: new[] { "tenant_id", "subject_id", "occurred_at" });

            migrationBuilder.Sql(
                """
                ALTER TABLE livestock.biological_profiles ADD CONSTRAINT ck_biological_profile CHECK
                    (sex BETWEEN 1 AND 3 AND subject_id IS DISTINCT FROM dam_id AND subject_id IS DISTINCT FROM sire_id
                     AND (dam_id IS NULL OR sire_id IS NULL OR dam_id <> sire_id));
                ALTER TABLE livestock.reproduction_events ADD CONSTRAINT ck_reproduction_event CHECK
                    (type BETWEEN 1 AND 4 AND outcome BETWEEN 1 AND 9 AND (offspring_count IS NULL OR offspring_count >= 0)
                     AND ((weight_kg > 0 AND entered_weight > 0 AND entered_weight_unit IS NOT NULL)
                       OR (weight_kg IS NULL AND entered_weight IS NULL AND entered_weight_unit IS NULL)));
                ALTER TABLE livestock.offspring_links ADD CONSTRAINT ck_offspring_link CHECK
                    (outcome BETWEEN 1 AND 2 AND ((birth_weight_kg > 0 AND entered_birth_weight > 0 AND entered_birth_weight_unit IS NOT NULL)
                     OR (birth_weight_kg IS NULL AND entered_birth_weight IS NULL AND entered_birth_weight_unit IS NULL)));
                ALTER TABLE livestock.milk_records ADD CONSTRAINT ck_milk_record CHECK
                    (session BETWEEN 1 AND 4 AND quantity_litres > 0 AND discarded_litres BETWEEN 0 AND quantity_litres
                     AND saleable_litres = quantity_litres - discarded_litres
                     AND (withdrawal_hold_until IS NULL OR withdrawal_hold_until <= milked_at OR discarded_litres = quantity_litres)
                     AND (fat_percent IS NULL OR fat_percent BETWEEN 0 AND 100)
                     AND (protein_percent IS NULL OR protein_percent BETWEEN 0 AND 100));
                ALTER TABLE livestock.grazing_plans ADD CONSTRAINT ck_grazing_plan CHECK
                    (status BETWEEN 1 AND 4 AND ends_at > starts_at AND target_head_count > 0
                     AND (rest_until IS NULL OR rest_until >= ends_at) AND (available_dry_matter_kg IS NULL OR available_dry_matter_kg >= 0)
                     AND ((status IN (1,2) AND closed_at IS NULL) OR (status IN (3,4) AND closed_at IS NOT NULL)));
                ALTER TABLE livestock.ration_plans ADD CONSTRAINT ck_ration_plan CHECK
                    (status BETWEEN 1 AND 3 AND target_head_count > 0 AND (effective_to IS NULL OR effective_to >= effective_from)
                     AND ((status = 1 AND activated_at IS NULL AND retired_at IS NULL)
                       OR (status = 2 AND activated_at IS NOT NULL AND retired_at IS NULL)
                       OR (status = 3 AND activated_at IS NOT NULL AND retired_at IS NOT NULL)));
                ALTER TABLE livestock.ration_lines ADD CONSTRAINT ck_ration_line CHECK
                    (quantity_kg_per_head_per_day > 0 AND entered_quantity > 0 AND dry_matter_percent > 0 AND dry_matter_percent <= 100);

                CREATE OR REPLACE FUNCTION livestock.validate_production_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE subject_farm uuid; subject_species text; event_farm uuid;
                BEGIN
                    IF TG_TABLE_NAME = 'biological_profiles' THEN
                        SELECT farm_id, species INTO subject_farm, subject_species FROM livestock.subjects
                          WHERE tenant_id = NEW.tenant_id AND id = NEW.subject_id AND kind = 1;
                        IF subject_farm IS NULL THEN RAISE EXCEPTION 'Biological profile requires an individual animal.' USING ERRCODE = '23514'; END IF;
                        IF NEW.dam_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.subjects s
                            WHERE s.tenant_id=NEW.tenant_id AND s.id=NEW.dam_id AND s.farm_id=subject_farm AND s.species=subject_species AND s.kind=1) THEN
                            RAISE EXCEPTION 'Dam must be an individual of the same farm and species.' USING ERRCODE = '23514'; END IF;
                        IF NEW.sire_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.subjects s
                            WHERE s.tenant_id=NEW.tenant_id AND s.id=NEW.sire_id AND s.farm_id=subject_farm AND s.species=subject_species AND s.kind=1) THEN
                            RAISE EXCEPTION 'Sire must be an individual of the same farm and species.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'reproduction_events' THEN
                        SELECT farm_id, species INTO subject_farm, subject_species FROM livestock.subjects
                          WHERE tenant_id=NEW.tenant_id AND id=NEW.subject_id AND kind=1;
                        IF subject_farm IS NULL OR subject_farm <> NEW.farm_id THEN RAISE EXCEPTION 'Reproduction subject scope is invalid.' USING ERRCODE = '23514'; END IF;
                        IF NEW.related_subject_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.subjects s WHERE s.tenant_id=NEW.tenant_id
                            AND s.id=NEW.related_subject_id AND s.farm_id=NEW.farm_id AND s.species=subject_species AND s.kind=1) THEN
                            RAISE EXCEPTION 'Related reproduction subject scope is invalid.' USING ERRCODE = '23514'; END IF;
                        IF NEW.breeding_event_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.reproduction_events e
                            WHERE e.tenant_id=NEW.tenant_id AND e.id=NEW.breeding_event_id AND e.subject_id=NEW.subject_id
                              AND e.type=1 AND e.occurred_at <= NEW.occurred_at) THEN
                            RAISE EXCEPTION 'Breeding lineage is invalid.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'offspring_links' THEN
                        SELECT e.farm_id INTO event_farm FROM livestock.reproduction_events e
                          WHERE e.tenant_id=NEW.tenant_id AND e.id=NEW.parturition_event_id AND e.type=3;
                        IF event_farm IS NULL OR NOT EXISTS (SELECT 1 FROM livestock.subjects s
                            WHERE s.tenant_id=NEW.tenant_id AND s.id=NEW.offspring_subject_id AND s.farm_id=event_farm AND s.kind=1) THEN
                            RAISE EXCEPTION 'Offspring scope or parturition type is invalid.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'milk_records' THEN
                        IF NOT EXISTS (SELECT 1 FROM livestock.subjects s WHERE s.tenant_id=NEW.tenant_id
                            AND s.id=NEW.subject_id AND s.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Milk subject farm scope is invalid.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'grazing_plans' THEN
                        IF NOT EXISTS (SELECT 1 FROM livestock.subjects s WHERE s.tenant_id=NEW.tenant_id
                            AND s.id=NEW.group_subject_id AND s.farm_id=NEW.farm_id AND s.kind=2)
                           OR NOT EXISTS (SELECT 1 FROM livestock.holdings h WHERE h.tenant_id=NEW.tenant_id
                            AND h.id=NEW.holding_id AND h.farm_id=NEW.farm_id AND h.type=2) THEN
                            RAISE EXCEPTION 'Grazing group or paddock scope is invalid.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'ration_plans' THEN
                        IF NOT EXISTS (SELECT 1 FROM livestock.subjects s WHERE s.tenant_id=NEW.tenant_id
                            AND s.id=NEW.subject_id AND s.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Ration subject farm scope is invalid.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'ration_lines' AND NEW.feed_batch_id IS NOT NULL THEN
                        IF NOT EXISTS (SELECT 1 FROM livestock.ration_plans p JOIN livestock.feed_batches b
                            ON b.tenant_id=p.tenant_id AND b.farm_id=p.farm_id WHERE p.tenant_id=NEW.tenant_id
                            AND p.id=NEW.plan_id AND b.id=NEW.feed_batch_id) THEN RAISE EXCEPTION 'Ration feed batch scope is invalid.' USING ERRCODE = '23514'; END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION livestock.guard_production_records() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Livestock production records are retained.' USING ERRCODE = '23514'; END IF;
                    IF TG_TABLE_NAME IN ('biological_profiles','reproduction_events','offspring_links','milk_records','ration_lines') THEN
                        RAISE EXCEPTION 'Livestock production evidence is immutable.' USING ERRCODE = '23514';
                    ELSIF TG_TABLE_NAME = 'grazing_plans' THEN
                        IF (to_jsonb(NEW) - ARRAY['status','closed_at','updated_at_utc','version']) <>
                           (to_jsonb(OLD) - ARRAY['status','closed_at','updated_at_utc','version']) THEN
                            RAISE EXCEPTION 'A grazing plan may only transition lifecycle.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'ration_plans' THEN
                        IF (to_jsonb(NEW) - ARRAY['status','activated_at','retired_at','updated_at_utc','version']) <>
                           (to_jsonb(OLD) - ARRAY['status','activated_at','retired_at','updated_at_utc','version']) THEN
                            RAISE EXCEPTION 'A ration plan may only transition lifecycle.' USING ERRCODE = '23514'; END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER biological_profile_scope BEFORE INSERT ON livestock.biological_profiles FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER reproduction_scope BEFORE INSERT ON livestock.reproduction_events FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER offspring_scope BEFORE INSERT ON livestock.offspring_links FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER milk_scope BEFORE INSERT ON livestock.milk_records FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER grazing_scope BEFORE INSERT ON livestock.grazing_plans FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER ration_plan_scope BEFORE INSERT ON livestock.ration_plans FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER ration_line_scope BEFORE INSERT ON livestock.ration_lines FOR EACH ROW EXECUTE FUNCTION livestock.validate_production_scope();
                CREATE TRIGGER biological_profile_guard BEFORE UPDATE OR DELETE ON livestock.biological_profiles FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER reproduction_guard BEFORE UPDATE OR DELETE ON livestock.reproduction_events FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER offspring_guard BEFORE UPDATE OR DELETE ON livestock.offspring_links FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER milk_guard BEFORE UPDATE OR DELETE ON livestock.milk_records FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER grazing_guard BEFORE UPDATE OR DELETE ON livestock.grazing_plans FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER ration_plan_guard BEFORE UPDATE OR DELETE ON livestock.ration_plans FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();
                CREATE TRIGGER ration_line_guard BEFORE UPDATE OR DELETE ON livestock.ration_lines FOR EACH ROW EXECUTE FUNCTION livestock.guard_production_records();

                ALTER TABLE livestock.biological_profiles ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.biological_profiles FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.reproduction_events ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.reproduction_events FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.offspring_links ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.offspring_links FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.milk_records ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.milk_records FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.grazing_plans ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.grazing_plans FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.ration_plans ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.ration_plans FORCE ROW LEVEL SECURITY;
                ALTER TABLE livestock.ration_lines ENABLE ROW LEVEL SECURITY; ALTER TABLE livestock.ration_lines FORCE ROW LEVEL SECURITY;
                CREATE POLICY biological_profiles_tenant ON livestock.biological_profiles USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY reproduction_events_tenant ON livestock.reproduction_events USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY offspring_links_tenant ON livestock.offspring_links USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY milk_records_tenant ON livestock.milk_records USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY grazing_plans_tenant ON livestock.grazing_plans USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY ration_plans_tenant ON livestock.ration_plans USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                CREATE POLICY ration_lines_tenant ON livestock.ration_lines USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid) WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                GRANT SELECT, INSERT, UPDATE, DELETE ON livestock.biological_profiles, livestock.reproduction_events,
                    livestock.offspring_links, livestock.milk_records, livestock.grazing_plans, livestock.ration_plans,
                    livestock.ration_lines TO murimios_app;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "biological_profiles",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "grazing_plans",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "milk_records",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "offspring_links",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "ration_lines",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "reproduction_events",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "ration_plans",
                schema: "livestock");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livestock.validate_production_scope(); DROP FUNCTION IF EXISTS livestock.guard_production_records();");
        }
    }
}
