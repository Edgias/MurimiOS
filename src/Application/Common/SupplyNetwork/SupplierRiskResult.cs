using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;

namespace EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;

using SupplierRiskBand = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskBand;

public sealed record SupplierRiskResult(int Score, SupplierRiskBand Band, string ComponentsJson, string InputsJson);
