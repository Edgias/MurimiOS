using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateCropFeedTransfer;
using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
public sealed record CreateCropFeedTransferRequest(Guid? Id, Guid? FeedBatchId, CropFeedSourceType SourceType,
    Guid SourceId, string BatchCode, string FeedName, decimal Quantity, string Unit,
    DateTimeOffset TransferredAt, DateOnly? ExpiresOn, string? Reference, string? Notes);
