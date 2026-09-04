using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface ITenantDatabaseRegistry
{
    string ControlDatabaseKey { get; }

    IReadOnlyCollection<string> DatabaseKeys { get; }

    bool Contains(string databaseKey);
}
