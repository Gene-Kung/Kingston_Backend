using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.DB
{
    public class BaseQueryParams
    {
        public DynamicParameters QueryParams { get; set; } = new DynamicParameters();
        public List<string> QueryString { get; set; } = new List<string>();
    }
}
