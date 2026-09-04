using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class MasterDataImportIntegrationTests
{
    [IntegrationFact]
    public async Task Csv_jobs_validate_preview_export_errors_resume_and_apply_every_supported_target()
    {
        await using var factory = new MurimiApiFactory();
        Guid marker = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(Guid.NewGuid());
        string farmCode = $"MI{marker:N}"[..12].ToUpperInvariant();

        HttpResponseMessage farmCreated = await client.PostAsJsonAsync("/api/v1/farms", new
        {
            name = "Master Import Existing Farm", code = farmCode, countryCode = "ZW", timeZone = "Africa/Harare"
        });
        Assert.Equal(HttpStatusCode.Created, farmCreated.StatusCode);

        string fieldsCsv = "farm_code,name,boundary_wkt,valid_from,source\r\n"
            + $"{farmCode},Imported North,\"POLYGON ((31 -17.8, 31.01 -17.8, 31.01 -17.81, 31 -17.81, 31 -17.8))\",2026-09-01,surveyed\r\n"
            + $"{farmCode},,\"POLYGON ((31.02 -17.8, 31.03 -17.8, 31.03 -17.81, 31.02 -17.81, 31.02 -17.8))\",2026-09-01,surveyed\r\n";
        JsonElement fieldJob = await UploadAsync(client, "Fields", fieldsCsv, "fields.csv");
        Assert.Equal(2, fieldJob.GetProperty("rowCount").GetInt32());
        Assert.Equal(1, fieldJob.GetProperty("readyRowCount").GetInt32());
        Assert.Equal(1, fieldJob.GetProperty("errorRowCount").GetInt32());
        Guid fieldJobId = fieldJob.GetProperty("id").GetGuid();

        HttpResponseMessage preview = await client.GetAsync($"/api/v1/imports/master-data/jobs/{fieldJobId}/preview?limit=1");
        Assert.Equal(HttpStatusCode.OK, preview.StatusCode);
        Assert.Single((await JsonAsync(preview)).GetProperty("rows").EnumerateArray());
        HttpResponseMessage errorFile = await client.GetAsync($"/api/v1/imports/master-data/jobs/{fieldJobId}/errors.csv");
        Assert.Equal(HttpStatusCode.OK, errorFile.StatusCode);
        Assert.Contains("required", await errorFile.Content.ReadAsStringAsync());

        JsonElement appliedFields = await ApplyAsync(client, fieldJobId, 1, "Fields");
        Assert.Equal(5, appliedFields.GetProperty("job").GetProperty("status").GetInt32());
        Assert.Equal(1, appliedFields.GetProperty("job").GetProperty("checkpointOrdinal").GetInt32());
        HttpResponseMessage fieldsExport = await client.GetAsync("/api/v1/imports/master-data/exports/Fields");
        Assert.Equal(HttpStatusCode.OK, fieldsExport.StatusCode);
        Assert.Contains("Imported North", await fieldsExport.Content.ReadAsStringAsync());

        var cases = new (string Type, string Csv)[]
        {
            ("Livestock", "farm_code,kind,name,species,official_identifier\r\n" +
                $"{farmCode},Individual,Imported Cow,Cattle,IMP-{marker:N}\r\n"),
            ("Inventory", "farm_code,input_name,input_category,unit,lot_code,received_on,expires_on,quantity\r\n" +
                $"{farmCode},Imported Fertiliser,Fertiliser,kg,LOT-{marker:N},2026-09-01,2027-09-01,500\r\n"),
            ("Suppliers", "supplier_code,name,contact_name,email,phone\r\n" +
                $"SUP-{marker:N},Imported Supplier,Jane Import,jane-{marker:N}@example.test,+263700000000\r\n"),
            ("OpeningBalances", "farm_code,account_code,account_name,direction,amount,currency,as_of,reference\r\n" +
                $"{farmCode},1000,Cash,Debit,2500,USD,2026-09-01,OPEN-{marker:N}\r\n"),
            ("FinanceTransactions", "farm_code,entry_type,category,amount,currency,occurred_at,reference,counterparty,description\r\n" +
                $"{farmCode},Expense,Seed,450,USD,2026-09-01T08:00:00Z,FIN-{marker:N},Imported Supplier,Imported expense\r\n"),
            ("Farms", "name,code,country_code,time_zone,parent_organisation_node_id\r\n" +
                $"Imported Second Farm,NEW{marker:N},ZW,Africa/Harare,\r\n")
        };
        foreach ((string type, string csv) in cases)
        {
            JsonElement job = await UploadAsync(client, type, csv, $"{type}.csv");
            Assert.Equal(1, job.GetProperty("readyRowCount").GetInt32());
            JsonElement applied = await ApplyAsync(client, job.GetProperty("id").GetGuid(), 1, type);
            Assert.Equal(5, applied.GetProperty("job").GetProperty("status").GetInt32());
            HttpResponseMessage export = await client.GetAsync($"/api/v1/imports/master-data/exports/{type}");
            Assert.Equal(HttpStatusCode.OK, export.StatusCode);
            Assert.True((await export.Content.ReadAsByteArrayAsync()).Length > 20);
        }

        HttpResponseMessage repeatApply = await client.PostAsync(
            $"/api/v1/imports/master-data/jobs/{fieldJobId}/apply?batchSize=1", null);
        Assert.Equal(HttpStatusCode.OK, repeatApply.StatusCode);
        JsonElement repeatBody = await JsonAsync(repeatApply);
        Assert.Equal(fieldJobId, repeatBody.GetProperty("job").GetProperty("id").GetGuid());
        Assert.Equal(0, repeatBody.GetProperty("batchApplied").GetInt32());
        Assert.True(repeatBody.GetProperty("completed").GetBoolean());
    }

    [IntegrationFact]
    public async Task Master_data_jobs_rows_and_opening_balances_are_forced_rls_and_least_privilege()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where (n.nspname,c.relname) in (('imports','master_data_jobs'),('imports','master_data_rows'),('finance','opening_balances'))),
              not has_table_privilege(current_user,'imports.master_data_jobs','DELETE'),
              not has_table_privilege(current_user,'imports.master_data_rows','DELETE'),
              not has_table_privilege(current_user,'finance.opening_balances','UPDATE'),
              not has_table_privilege(current_user,'finance.opening_balances','DELETE')
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        for (int index = 0; index < 5; index++) Assert.True(reader.GetBoolean(index));
    }

    private static async Task<JsonElement> UploadAsync(HttpClient client, string type, string csv, string fileName)
    {
        using var content = new StringContent(csv, Encoding.UTF8, "text/csv");
        HttpResponseMessage response = await client.PostAsync(
            $"/api/v1/imports/master-data/jobs?type={type}&fileName={Uri.EscapeDataString(fileName)}", content);
        string body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK,
            $"Upload {type} failed with {(int)response.StatusCode}: {body}");
        using JsonDocument document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
    }

    private static async Task<JsonElement> ApplyAsync(
        HttpClient client,
        Guid jobId,
        int batchSize,
        string targetType)
    {
        HttpResponseMessage response = await client.PostAsync(
            $"/api/v1/imports/master-data/jobs/{jobId}/apply?batchSize={batchSize}", null);
        string body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK,
            $"Apply {targetType} failed with {(int)response.StatusCode}: {body}");
        using JsonDocument document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
    }

    private static async Task<JsonElement> JsonAsync(HttpResponseMessage response)
    {
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
