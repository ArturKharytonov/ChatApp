using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.DTOs.Http.Responses
{
    public class GridModelResponse<T>
    {
        public IEnumerable<T> Users { get; set; }
        public int TotalCount { get; set; }
    }
}
