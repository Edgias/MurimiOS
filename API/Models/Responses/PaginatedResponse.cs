using System.Collections.Generic;

namespace Murimi.API.Models.Responses
{
    public class PaginatedResponse<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
