using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.RegisterDerivedMedia;
using DerivedMediaKind = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaKind;
public sealed record RegisterDerivedMediaRequest(Guid? Id, Guid? FieldId, DerivedMediaKind Kind, string SourceType,
    Guid SourceId, string ObjectKey, string ContentType, long ByteSize, string Sha256, DateTimeOffset GeneratedAt);
