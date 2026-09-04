using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateModel;
public sealed record CreatePredictiveModelRequest(Guid? Id, string ModelKey, int ModelVersion, string Name,
    string TargetMetricCode, string Algorithm, string FeatureSchemaJson, int TrainingWindowDays,
    int MinimumObservations, decimal ValidationMae, decimal? ValidationMapePercent,
    string ValidationDatasetReference);
