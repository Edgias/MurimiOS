using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.Shared;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
public sealed record CropFeedTransferResponse(Guid Id, Guid FarmId, CropFeedSourceType SourceType, Guid SourceId,
    Guid FeedBatchId, Guid TraceEventId, decimal Quantity, string Unit, decimal EnteredQuantity,
    string EnteredUnit, DateTimeOffset TransferredAt, string TransferredBy, string? Notes);
