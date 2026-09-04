using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDerivedMedia;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using DerivedMediaLifecycleStatus = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaLifecycleStatus;

public sealed record ListDerivedMediaQuery(Guid farmId, DerivedMediaLifecycleStatus? status) : IRequest<UseCaseResult>;
