using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ListCompanies;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;

public sealed record ListCompaniesQuery(bool? active) : IRequest<UseCaseResult>;
