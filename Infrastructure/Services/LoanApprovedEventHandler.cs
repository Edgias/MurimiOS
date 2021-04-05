using Edgias.Agrik.ApplicationCore.Events;
using System;
using System.Threading.Tasks;

namespace Edgias.Agrik.Infrastructure.Services
{
    public class LoanApprovedEventHandler : DomainEventHandler<LoanApprovedEvent>
    {
        protected override Task HandleAsync(LoanApprovedEvent domainEvent)
        {
            throw new NotImplementedException();
        }
    }
}
