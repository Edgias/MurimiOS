﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.Interfaces
{
    public interface IMessagingService
    {
        Task SendMessageAsync(string recipient, string subject, string message);

        Task SendMessageAsync(IEnumerable<string> recipients, string subject, string message);

    }
}
