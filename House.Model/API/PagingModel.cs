using House.Model.DB;
using House.Model.Enums;
using House.Model.Extension;
using House.Model.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;


namespace House.Model.API
{
    public class ReqPagingModel
    {
        public int page_size { get; set; } = 10;
        public int page_index { get; set; } = 0;
    }

    public class ResPagingModel<T> : ResBaseBodyModel
    {
        public List<T> list { get; set; }
        public int page_size { get; set; }
        public int page_index { get; set; }
        public int page_total { get; set; }
        public long row_total { get; set; }
    }
}
