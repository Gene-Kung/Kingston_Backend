using House.Model;
using House.Model.API;
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
    public class UserDao : MSBaseDao
    {
        public UserDao(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public List<UserModel> Query(string email, string name)
        {
            var sql = "QueryUser";
            var param = new { email, name };
            return Query<UserModel>(sql, param);
        }

        public UserModel Get(int id)
        {
            var sql = "GetUser";
            var param = new {id};
            return Get<UserModel>(sql, param);
        }

        public int Create(UserModel userModel)
        {
            var sql = "CreateUser";
            var param = new { userModel.email, userModel.name, userModel.password};
            return ExecuteScalar(sql, param);
        }

        public int Update(UserModel userModel)
        {
            var sql = "UpdateUser";
            var param = new { userModel.id, userModel.email, userModel.name, userModel.password, userModel.token };
            return Execute(sql, param);
        }

        public int Delete(int id) 
        {
            var sql = "DeleteUser";
            var param = new { id };
            return Execute(sql, param);
        }

        public UserModel Login(string email)
        {
            var sql = "Login";
            var param = new { email };
            return Get<UserModel>(sql, param);
        }
    }
}
