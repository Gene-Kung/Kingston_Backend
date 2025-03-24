using House.Model;
//using House.Model.API.Login;
using House.Model.Enums;
using House.Model.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace House.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IHttpContextAccessor _accessor;
        protected string _requestIP;
        protected Dictionary<string, StringValues> _headers;
        protected readonly Logger _logger;
        protected readonly long _memberID;
        protected readonly List<string> _roleIDs;
        protected readonly AppSettings _appSettings;
        protected readonly string _ip;

        protected BaseController(IHttpContextAccessor accessor, IOptions<AppSettings> appSettings)
        {
            _accessor = accessor;
            _appSettings = appSettings.Value;
            _headers = accessor.HttpContext.Request.Headers.ToDictionary(a => a.Key, a => a.Value);
            _requestIP = accessor.HttpContext.Connection.RemoteIpAddress.ToString().Replace("::1", "127.0.0.1");
            _logger = LogManager.GetLogger("insert_api_file");
            if (accessor.HttpContext.Items["MemberID"] != null)
                _memberID = Convert.ToInt64(accessor.HttpContext.Items["MemberID"]);
            if (accessor.HttpContext.Items["RoleIDs"] != null)
                _roleIDs = (List<string>)accessor.HttpContext.Items["RoleIDs"];
            if (accessor.HttpContext.Items["ClientIP"] != null)
                _ip = Convert.ToString(accessor.HttpContext.Items["ClientIP"]);

            HandleRequestCount();
        }

        private void HandleRequestCount()
        {
            // 檢查請求中是否包含 Cookie
            if (!_accessor.HttpContext.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                if (_accessor.HttpContext.Request.Cookies.TryGetValue("request_count", out string requestCountValue))
                {
                    int requestCount = int.Parse(requestCountValue);

                    // 節流限制的邏輯，例如每分鐘不超過 10 次請求
                    if (requestCount > 20)
                    {
                        throw new APIException(ErrorCodeEnum.request_too_much);
                    }

                    // 增加計數器並更新 Cookie
                    requestCount++;
                    //SetCookie("request_count", requestCount.ToString(), 1);
                }
                else
                {
                    // 初始設置請求計數器 Cookie
                    //SetCookie("request_count", "1", 1);
                }
            }
        }

        //protected void SetCookie(ResBaseLoginModel res)
        //{
        //    SetCookie(nameof(res.token), res.token);
        //    SetCookie(nameof(res.latest_access_type), res.latest_access_type);
        //    SetCookie(nameof(res.member_id), res.member_id.ToString());
        //    SetCookie(nameof(res.email), res.email);
        //    SetCookie(nameof(res.role_ids), string.Join(",", res.role_ids));
        //}

        //protected void SetCookie(string key, string value, int expires = 1440)
        //{
        //    _accessor.HttpContext.Response.Cookies.Append(key, value, new CookieOptions
        //    {
        //        SameSite = SameSiteMode.None,
        //        Secure = true,
        //        Expires = DateTimeOffset.UtcNow.AddMinutes(expires)
        //    });
        //}
    }
}
