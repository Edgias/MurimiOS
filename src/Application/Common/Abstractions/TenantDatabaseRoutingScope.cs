using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class TenantDatabaseRoutingScope
{
    private int controlDepth;
    public bool UseControlDatabase => Volatile.Read(ref controlDepth) > 0;
    public IDisposable EnterControlPlane()
    {
        Interlocked.Increment(ref controlDepth);
        return new Exit(this);
    }

    private sealed class Exit(TenantDatabaseRoutingScope owner) : IDisposable
    {
        private TenantDatabaseRoutingScope? current = owner;
        public void Dispose()
        {
            TenantDatabaseRoutingScope? value = Interlocked.Exchange(ref current, null);
            if (value is not null) Interlocked.Decrement(ref value.controlDepth);
        }
    }
}
