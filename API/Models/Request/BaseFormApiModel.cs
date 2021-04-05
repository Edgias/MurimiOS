using System;

namespace Murimi.API.Models.Request
{
    public abstract class BaseRequestApiModel
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

    }
}
