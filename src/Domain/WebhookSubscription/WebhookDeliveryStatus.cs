using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.WebhookSubscription;
public enum WebhookDeliveryStatus { Pending = 1, Running = 2, RetryScheduled = 3, Delivered = 4, DeadLettered = 5 }
