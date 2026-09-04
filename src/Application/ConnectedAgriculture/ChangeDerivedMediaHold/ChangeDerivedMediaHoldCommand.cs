using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeDerivedMediaHold;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record ChangeDerivedMediaHoldCommand(Guid assetId, string command, DerivedMediaHoldRequest request) : IRequest<UseCaseResult>;
