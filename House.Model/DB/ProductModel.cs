using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.DB
{
    public class ProductModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public DateTime deleted_time { get; set; }
    }
}
