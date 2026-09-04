using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.CreateCase;
using EudrCommodity = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCommodity;

public sealed record CreateEudrCaseRequest(Guid? Id, string CaseNumber, EudrCommodity Commodity,
    string ProductDescription, string HsCode, decimal Quantity, string Unit, string ProductionCountry,
    DateOnly ProductionFrom, DateOnly ProductionTo, string OperatorName, string OperatorRegistrationNumber,
    string OperatorAddress, string Market);
