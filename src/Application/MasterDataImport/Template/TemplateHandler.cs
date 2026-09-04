using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Application.Common.Imports;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.Template;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;

using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;

public sealed class TemplateHandler() : IRequestHandler<TemplateQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(TemplateQuery message, CancellationToken cancellationToken) => Task.FromResult(TemplateAsync(message.type));

    private static UseCaseResult TemplateAsync(string type)
    {
        if (!TryType(type, out MasterDataImportType target)) return Bad("master_data_type_invalid", TypeMessage());
        return UseCaseResults.File(MasterDataCsv.Template(target), "text/csv; charset=utf-8", $"murimios-{Slug(target)}-template.csv");
    }

    private static bool TryType(string value, out MasterDataImportType type) =>
        Enum.TryParse(value.Replace("-", ""), true, out type) && Enum.IsDefined(type);

    private static string TypeMessage() => "Use Farms, Fields, Livestock, Inventory, Suppliers, OpeningBalances, or FinanceTransactions.";

    private static string Slug(MasterDataImportType type) => type.ToString().ToLowerInvariant();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
