using House.Model.Attributes;
using House.Model.DB;
using System;
using System.Collections.Generic;

namespace House.Model.API.Order
{
    public class ReqCreateOrderModel
    {
        [APIRequired]
        public string name { get; set; }
        [APIRequired]
        [APIRegularExpression(@"^09\d{8}$|^0\d{1,2}-?\d{6,8}$")]
        public string phone { get; set; }
        public int created_user_id { get; set; }
        public decimal total_price { get; set; }
        public List<OrderDetailModel> order_detail { get; set; }
    }
}
