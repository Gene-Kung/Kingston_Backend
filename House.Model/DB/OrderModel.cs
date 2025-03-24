using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.DB
{
    public class OrderModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public DateTime created_time { get; set; }
        public int created_user_id { get; set; }
        public DateTime? updated_time { get; set; }
        public DateTime? deleted_time { get; set; }
        public decimal total_price { get; set; }
    }
}
