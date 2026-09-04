using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateCompany;

public sealed record CreateLegalCompanyRequest(Guid? Id, Guid OrganisationNodeId, string CompanyCode,
    string LegalName, string RegistrationNumber, string? TaxIdentifier, string CountryCode,
    string FunctionalCurrency, DateOnly EffectiveFrom);
