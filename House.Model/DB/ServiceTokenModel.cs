using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.DB
{
    public class ServiceTokenModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public string client_id { get; set; }
        public string code { get; set; }
        public string client_secret { get; set; }
        public DateTime created_time { get; set; }
        public DateTime? updated_time { get; set; }
        public bool is_deleted { get; set; }
        public string token { get; set; }
    }
}
