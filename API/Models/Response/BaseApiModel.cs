using System;

namespace Murimi.API.Models.Response
{
    public abstract class BaseApiModel
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; }
    }
}
