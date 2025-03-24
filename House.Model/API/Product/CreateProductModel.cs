using House.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.API.Product
{
    public class ReqCreateProductModel
    {
        public int id { get; set; }
        [APIRequired]
        public string name { get; set; }
        [APIRequired]
        public decimal price { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public DateTime deleted_time { get; set; }
    }
}
