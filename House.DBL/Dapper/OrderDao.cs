using Dapper;
using House.Model;
using House.Model.API.Order;
using House.Model.API.Product;
using House.Model.DB;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace House.DAL.Dapper
{
    public class OrderDao : MSBaseDao
    {
        public OrderDao(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public List<OrderModel> Query(ReqQueryOrderModel model)
        {
            var sql = "QueryOrder";
            var param = new { model.name, model.phone, model.user_name };
            return Query<OrderModel>(sql, param);
        }

        public OrderModel Get(int id)
        {
            var sql = "GetOrder";
            var param = new { id };
            return Get<OrderModel>(sql, param);
        }

        public int Create(ReqCreateOrderModel model)
        {
            var sql = "CreateOrder";

            // 把 List<UserDto> 轉成 DataTable
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("sugar", typeof(string));
            dataTable.Columns.Add("ice", typeof(string));
            dataTable.Columns.Add("product_id", typeof(int));

            foreach (var order_detail in model.order_detail)
            {
                dataTable.Rows.Add(order_detail.sugar, order_detail.ice, order_detail.product_id);
            }

            var param = new DynamicParameters();
            param.Add("@name", model.name);
            param.Add("@phone", model.phone);
            param.Add("@created_user_id", model.created_user_id);
            param.Add("@total_price", model.total_price);
            param.Add("@order_details", dataTable.AsTableValuedParameter("dbo.OrderDetailType"));
            return ExecuteScalar(sql, param);
        }

        public int Update(OrderModel model)
        {
            var sql = "UpdateOrder";
            var param = new { model.id, model.name, model.phone, model.total_price };
            return Execute(sql, param);
        }

        public int Delete(int id)
        {
            var sql = "DeleteOrder";
            var param = new { id };
            return Execute(sql, param);
        }

        public List<OrderDetailModel> QueryOrderDetail(OrderModel model) 
        {
            var sql = "QueryOrderDetail";
            var param = new { order_id = model.id };
            return Query<OrderDetailModel>(sql, param);
        }
    }
}
