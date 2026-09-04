using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListSessions;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;

public sealed record ListSessionsQuery(string? subject, IdentitySessionStatus? status) : IRequest<UseCaseResult>;
