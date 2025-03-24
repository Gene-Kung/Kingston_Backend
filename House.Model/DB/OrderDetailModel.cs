using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.DB
{
    public class OrderDetailModel
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public string sugar { get; set; }
        public string ice { get; set; }
        public int product_id { get; set; }
    }
}
