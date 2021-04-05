using System;

namespace Edgias.Agrik.API.Models.View
{
    public abstract class BaseApiModel
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; }
    }
}
