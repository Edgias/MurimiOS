using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class IntegratePlanningFinanceLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE finance.transactions DROP CONSTRAINT ck_finance_transaction_source;
                ALTER TABLE finance.transactions
                    ADD CONSTRAINT ck_finance_transaction_source CHECK (source_type BETWEEN 1 AND 7);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $down$
                BEGIN
                    IF EXISTS (SELECT 1 FROM finance.transactions WHERE source_type = 7) THEN
                        RAISE EXCEPTION 'Cannot downgrade while planning-linked finance transactions exist.' USING ERRCODE = '55000';
                    END IF;
                END
                $down$;
                ALTER TABLE finance.transactions DROP CONSTRAINT ck_finance_transaction_source;
                ALTER TABLE finance.transactions
                    ADD CONSTRAINT ck_finance_transaction_source CHECK (source_type BETWEEN 1 AND 6);
                """);
        }
    }
}
