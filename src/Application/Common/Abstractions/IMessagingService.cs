using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions
{
    public interface IMessagingService
    {
        Task SendMessageAsync(string recipient, string subject, string message);

        Task SendMessageAsync(IEnumerable<string> recipients, string subject, string message);

    }
}
