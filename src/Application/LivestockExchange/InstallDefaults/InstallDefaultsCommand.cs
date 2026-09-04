using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.InstallDefaults;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

public sealed record InstallDefaultsCommand() : IRequest<UseCaseResult>;
