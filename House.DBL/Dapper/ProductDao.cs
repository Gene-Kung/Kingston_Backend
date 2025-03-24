using House.Model;
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

namespace House.DAL.Dapper
{
    public class ProductDao : MSBaseDao
    {
        public ProductDao(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public List<ProductModel> Query(ReqQueryProductModel productModel)
        {
            var sql = "QueryProduct";
            var param = new { productModel.name, productModel.price };
            return Query<ProductModel>(sql, param);
        }

        public ProductModel Get(int id)
        {
            var sql = "GetProduct";
            var param = new {id};
            return Get<ProductModel>(sql, param);
        }

        public int Create(ProductModel productModel)
        {
            var sql = "CreateProduct";
            var param = new { productModel.name, productModel.price };
            return ExecuteScalar(sql, param);
        }

        public int Update(ProductModel productModel)
        {
            var sql = "UpdateProduct";
            var param = new { productModel.id, productModel.name, productModel.price };
            return Execute(sql, param);
        }

        public int Delete(int id) 
        {
            var sql = "DeleteProduct";
            var param = new { id };
            return Execute(sql, param);
        }
    }
}
