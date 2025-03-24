using System;

namespace House.Model.API.Order
{
    public class ReqQueryOrderModel
    {
        public string? name { get; set; }
        public string? phone { get; set; }
        public DateTime? created_time { get; set; }
        public string? user_name { get; set; }
        public DateTime? updated_time { get; set; }
        public DateTime? deleted_time { get; set; }
        public decimal? total_price { get; set; }
    }
}
