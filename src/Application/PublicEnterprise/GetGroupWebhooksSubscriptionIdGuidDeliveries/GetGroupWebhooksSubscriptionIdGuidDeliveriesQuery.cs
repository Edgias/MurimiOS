using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;

using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;
using WebhookDeliveryStatus = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDeliveryStatus;

public sealed record GetGroupWebhooksSubscriptionIdGuidDeliveriesQuery(Guid subscriptionId, WebhookDeliveryStatus? status) : IRequest<UseCaseResult>;
