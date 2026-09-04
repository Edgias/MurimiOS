using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetCatalogues;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;

public sealed record GetCataloguesQuery(DateOnly? effectiveOn) : IRequest<UseCaseResult>;
