using System.Collections.Generic;

namespace Murimi.API.Models.Response
{
    public class ApiResponse<T>
    {
        public int Count { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
