using Murimi.ApplicationCore.Events;
using System;
using System.Threading.Tasks;

namespace Murimi.Infrastructure.Services
{
    public class LoanApprovedEventHandler : DomainEventHandler<LoanApprovedEvent>
    {
        protected override Task HandleAsync(LoanApprovedEvent domainEvent)
        {
            throw new NotImplementedException();
        }
    }
}
