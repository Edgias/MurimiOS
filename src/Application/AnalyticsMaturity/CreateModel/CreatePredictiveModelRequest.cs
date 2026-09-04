using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateModel;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
public sealed record CreatePredictiveModelRequest(Guid? Id, string ModelKey, int ModelVersion, string Name,
    string TargetMetricCode, string Algorithm, string FeatureSchemaJson, int TrainingWindowDays,
    int MinimumObservations, decimal ValidationMae, decimal? ValidationMapePercent,
    string ValidationDatasetReference);
