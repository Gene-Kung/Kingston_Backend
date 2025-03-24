using Dapper;
using House.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System;
using House.Model.DB;
using Microsoft.Extensions.Options;
using House.Model.API;
using Microsoft.Data.SqlClient;

namespace House.DAL.Dapper
{
    public class MSBaseDao
    {
        protected readonly AppSettings _appSettings;
        protected readonly string _connectionString;

        protected MSBaseDao(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _connectionString = _appSettings.MSConnectionStrings.DefaultConnection;
        }

        protected List<T> Query<T>(string name, object param)
        {
            List<T> results;
            using (var conn = new SqlConnection(_connectionString))
            {
                results = conn.Query<T>(name, param, commandType: CommandType.StoredProcedure).ToList();
            }
            return results;
        }

        protected T Get<T>(string name, object param)
        {
            var results = default(T);
            using (var conn = new SqlConnection(_connectionString))
            {
                results = conn.QueryFirstOrDefault<T>(name, param, commandType: CommandType.StoredProcedure);
            }
            return results;
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected int ExecuteScalar(string name, object param)
        {
            int results;
            using (var conn = new SqlConnection(_connectionString))
            {
                results = conn.ExecuteScalar<int>(name, param, commandType: CommandType.StoredProcedure);
            }
            return results;
        }

        /// <summary>
        /// Update and Delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected int Execute(string name, object param)
        {
            int results;
            using (var conn = new SqlConnection(_connectionString))
            {
                results = conn.Execute(name, param, commandType: CommandType.StoredProcedure);
            }
            return results;
        }
    }
}
