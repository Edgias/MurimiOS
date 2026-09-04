using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListRecovery;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using AccountRecoveryStatus = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryStatus;

public sealed record ListRecoveryQuery(AccountRecoveryStatus? status) : IRequest<UseCaseResult>;
