using System;

namespace Murimi.API.Models.Responses
{
    public abstract class BaseResponse
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

    }
}
