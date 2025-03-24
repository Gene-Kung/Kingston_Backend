using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.API.Product
{
    public class ReqQueryProductModel
    {
        public string? name { get; set; } = null;
        public decimal? price { get; set; } = null;
        public DateTime? created_time { get; set; } = null;
        public DateTime? updated_time { get; set; } = null;
        public DateTime? deleted_time { get; set; } = null;
    }
}
