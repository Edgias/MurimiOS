using System.Collections.Generic;

namespace Edgias.Agrik.API.Models.View
{
    public class ApiResponse<T>
    {
        public int Count { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
