using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

public sealed record TemplateQuery(string type) : IRequest<UseCaseResult>;
