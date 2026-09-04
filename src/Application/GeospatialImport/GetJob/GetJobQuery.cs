using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.GeospatialImport.GetJob;
using GeospatialImportFormat = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFormat;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GeospatialImportStatus = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportStatus;

public sealed record GetJobQuery(Guid jobId) : IRequest<UseCaseResult>;
