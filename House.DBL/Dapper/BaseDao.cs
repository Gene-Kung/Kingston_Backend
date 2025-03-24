using Dapper;
using House.Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System;
using House.Model.DB;
using Microsoft.Extensions.Options;
using House.Model.API;

namespace House.DAL.Dapper
{
    public class BaseDao
    {
        protected readonly AppSettings _appSettings;
        protected readonly string _connectionString;
        private readonly string _tableName;

        protected BaseDao(IOptions<AppSettings> appSettings, string tableName)
        {
            _appSettings = appSettings.Value;
            _connectionString = _appSettings.ConnectionStrings.DefaultConnection;
            _tableName = tableName;
        }

        protected List<T> Query<T>(string sql, object param = null)
        {
            List<T> results;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                results = conn.Query<T>(sql, param).ToList();
            }
            return results;
        }

        protected T Get<T>(string sql, object param = null)
        {
            T result = default(T);
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                result = conn.QueryFirstOrDefault<T>(sql, param);
            }
            return result;
        }



        protected List<T> StoredProcedureQuery<T>(string sql, object param, CommandType commandType)
        {
            List<T> results;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                results = conn.Query<T>(sql, param).ToList();
            }
            return results;
        }

        protected int Execute(string sql, object param = null)
        {
            int rowCount;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                rowCount = conn.Execute(sql, param);
            }
            return rowCount;
        }

        protected int Execute(string sql, NpgsqlConnection conn, object param = null, NpgsqlTransaction trans = null)
        {
            int rowCount;
            rowCount = conn.Execute(sql, param, trans);
            return rowCount;
        }

        protected T ExecuteScalar<T>(string sql, object param = null)
        {
            T result = default(T);
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                result = conn.ExecuteScalar<T>(sql, param);
            }
            return result;
        }

        protected T ExecuteScalar<T>(string sql, NpgsqlConnection conn, object param = null, NpgsqlTransaction trans = null)
        {
            T result = default(T);
            result = conn.ExecuteScalar<T>(sql, param, trans);
            return result;
        }

        protected List<T> QueryByPaging<T>(StringBuilder sqlCmd, BaseQueryParams baseQueryParams, int page_size, int page_index)
        {
            var offset = page_size * page_index;

            if (baseQueryParams.QueryString != null && baseQueryParams.QueryString.Any())
            {
                sqlCmd.Append($" AND {string.Join(" AND ", baseQueryParams.QueryString)}");
            }
            //sql.Append($" ORDER BY id LIMIT {page_size} OFFSET {offset};");
            sqlCmd.Append($" ORDER BY ID OFFSET {offset} ROWS FETCH NEXT {page_size} ROWS ONLY;");

            var list = Query<T>(sqlCmd.ToString(), baseQueryParams.QueryParams);

            return list;
        }

        /// <summary>
        /// 單一資料表分頁筆數取得
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="baseQueryParams"></param>
        /// <param name="page_size"></param>
        /// <returns></returns>
        private (long row_total, int page_total) GetRowCount(BaseQueryParams baseQueryParams, int page_size)
        {
            var sql = new StringBuilder($"SELECT COUNT(*) FROM public.{_tableName} WHERE 1 = 1");
            if (baseQueryParams.QueryString != null && baseQueryParams.QueryString.Any())
            {
                sql.Append($" AND {string.Join(" AND ", baseQueryParams.QueryString)}");
            }
            var row_total = ExecuteScalar<long>(sql.ToString(), baseQueryParams.QueryParams);
            var page_total = Convert.ToInt32(Math.Ceiling((Decimal)row_total / page_size));
            return (row_total, page_total);
        }

        protected (long row_total, int page_total) GetRowCount(StringBuilder sqlCmd, BaseQueryParams baseQueryParams, int page_size)
        {
            if (baseQueryParams.QueryString != null && baseQueryParams.QueryString.Any())
            {
                sqlCmd.Append($" AND {string.Join(" AND ", baseQueryParams.QueryString)}");
            }
            var row_total = ExecuteScalar<long>(sqlCmd.ToString(), baseQueryParams.QueryParams);
            var page_total = Convert.ToInt32(Math.Ceiling((Decimal)row_total / page_size));
            return (row_total, page_total);
        }

        /// <summary>
        /// 單一資料表分頁查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="baseQueryParams"></param>
        /// <param name="page_size"></param>
        /// <param name="page_index"></param>
        /// <returns>資料集合, 總筆數, 總頁數</returns>
        public ResPagingModel<T> QueryByPaging<T>(BaseQueryParams baseQueryParams, int page_size, int page_index)
        {
            var res = new ResPagingModel<T>() { page_size = page_size, page_index = page_index };
            var sqlCmd = new StringBuilder($"SELECT * FROM public.{_tableName} WHERE 1 = 1");
            var sqlCmdCount = new StringBuilder($"SELECT COUNT(*) FROM public.{_tableName} WHERE 1 = 1");
            res.list = QueryByPaging<T>(sqlCmd, baseQueryParams, page_size, page_index);
            (res.row_total, res.page_total) = GetRowCount(sqlCmdCount, baseQueryParams, page_size);
            return res;
        }

        public T Get<T>(long id)
        {
            var sql = $"SELECT * FROM public.{_tableName} WHERE id = @id;";
            var param = new { id };
            return Get<T>(sql, param);
        }


        public List<T> Query<T>(BaseQueryParams baseQueryParams = null)
        {
            var sql = new StringBuilder($"SELECT * FROM public.{_tableName} WHERE 1 = 1");

            if (baseQueryParams == null) return Query<T>(sql.ToString());

            if (baseQueryParams.QueryString != null && baseQueryParams.QueryString.Any())
            {
                sql.Append($" AND {string.Join(" AND ", baseQueryParams.QueryString)}");
            }
            return Query<T>(sql.ToString(), baseQueryParams.QueryParams);
        }

        public T Get<T>(BaseQueryParams baseQueryParams)
        {
            var sql = new StringBuilder($"SELECT * FROM public.{_tableName} WHERE 1 = 1");

            if (baseQueryParams.QueryString != null && baseQueryParams.QueryString.Any())
            {
                sql.Append($" AND {string.Join(" AND ", baseQueryParams.QueryString)}");
            }
            return Get<T>(sql.ToString(), baseQueryParams.QueryParams);
        }
    }
}
