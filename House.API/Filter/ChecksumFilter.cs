using House.CBL;
using House.Model;
using House.Model.API;
using House.Model.Attributes;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extension;
using House.Model.Extensions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace House.API.Filter
{
    public class ChecksumFilter : ActionFilterAttribute
    {
        private readonly AppSettings _appSettings;
        private StringValues _checksum;
        private StringValues _timestamp;
        private StringValues _validPlatform;
        private string _sign = "123456";
        private string _urlPath;
        private string _method;

        public ChecksumFilter(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _validPlatform = "app";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //手機 app 裝置才需要 checksum 驗證
            if (context.HttpContext.Request.Headers["platform"].ToString().Trim().ToLower() == _validPlatform)
            {
                _urlPath = context.HttpContext.Request.Path;
                _method = context.HttpContext.Request.Method;

                if (!ValidHeader(context))
                    return;

                //從 APIAuthorizationFilter 取得
                //_sign = context.HttpContext.Items["Sign"]?.ToString();

                if (!ValidateChecksum(context))
                {
                    var res_body = new ResBaseBodyModel
                    {
                        code = ErrorCodeEnum.cks.GetHashCode(),
                        message = $"{ErrorCodeEnum.cks.GetDesc()}, Request Field: CheckSum",
                    };
                    var res_timestamp = DateTime.Now.ToUnixTimestamp().ToString();
                    context.HttpContext.Response.Headers.Add("timestamp", res_timestamp);
                    context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, res_body));
                    context.Result = new JsonResult(res_body);
                    return;
                }
            }
            base.OnActionExecuting(context);
        }

        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Headers["platform"].ToString().Trim().ToLower() == _validPlatform)
            {
                var res_timestamp = DateTime.Now.ToUnixTimestamp().ToString();
                context.HttpContext.Response.Headers.Add("timestamp", res_timestamp);

                if (context.Exception == null)
                {
                    //使用_sign去處理Checksum
                    if (context.Result is ObjectResult objectResult)
                    {
                        context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, objectResult.Value));
                        //context.Result = new JsonResult(objectResult.Value);
                    }
                }
                else
                {
                    if (context.Exception is APIException)
                    {
                        // 處理異常並設置適當的回應結果
                        var exception = (APIException)context.Exception;
                        var resBody = new ResBaseBodyModel
                        {
                            code = exception.RetCode,
                            message = exception.RetMsg,
                        };
                        context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, resBody));
                    }
                    else //其他所有的Exception
                    {
                        // 處理異常並設置適當的回應結果
                        var resBody = new ResBaseBodyModel()
                        {
                            code = ErrorCodeEnum.failed.GetHashCode(),
                            message = ErrorCodeEnum.failed.GetDesc()
                        };
                        context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, resBody));
                    }
                }
            }
            base.OnActionExecuted(context);
        }

        public bool ValidateChecksum(ActionExecutingContext context)
        {
            if (!_appSettings.Validating.CheckSum)
                return true;

            var isSuccess = false;

            switch (_method)
            {
                case "GET":
                    if (_checksum == GenChecksum(_timestamp))
                        isSuccess = true;

                    break;
                case "POST": //新增、查詢、更新時使用
                    var requestBody = context.HttpContext.Items["RequestBody"]?.ToString();
                    var parameterType = context.ActionDescriptor.Parameters?.First().ParameterType;
                    var payload = JsonConvert.DeserializeObject(requestBody, parameterType);

                    if (_checksum == GenChecksum(_timestamp, payload))
                        isSuccess = true;

                    break;
                //case "PATCH": //暫時不用PATCH
                //    _id = context.HttpContext.Request.Query["id"].ToString();
                //    break;
                case "DELETE":
                    if (_checksum == GenChecksum(_timestamp))
                        isSuccess = true;

                    break;
                default:
                    break;
            }

            return isSuccess;
        }

        private static string GetChecksumArg(object model, bool isFirstLayer)
        {
            var dicChecksumFields = new Dictionary<string, string>();

            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (isFirstLayer)
                {
                    var attribute = property.GetCustomAttribute<ChecksumAttribute>();
                    if (attribute == null)
                        continue;
                }

                if (property.GetValue(model) == null)
                    continue;

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var checksumArgSerial = GetChecksumArg(property.GetValue(model), false);
                    dicChecksumFields.Add(property.Name, checksumArgSerial);
                }
                else
                {
                    dicChecksumFields.Add(property.Name, property.GetValue(model).ToString());
                }
            }
            return SortDictionary(dicChecksumFields);
        }

        private static string SortDictionary(Dictionary<string, string> dic)
        {
            var values = dic.OrderBy(x => x.Key.ToLower()).Select(x => (x.Value != null && x.Value != "") ? x.Value.ToString() : "").ToList();
            var param = string.Join("&", values);
            return param;
        }

        public string GenChecksum(string timestamp, object payload = null)
        {
            var argList = new List<string>() { _sign, timestamp, _urlPath };

            if (payload != null)
                argList.Add(GetChecksumArg(payload, true));

            var checksumArgSerial = string.Join("&", argList);

            //Url encode
            checksumArgSerial = HttpUtility.UrlEncode(checksumArgSerial);

            // 回傳有中文在前端與後端urlencode會有大小寫差異，需要特別處理，前後端皆居在SHA256前轉小寫
            checksumArgSerial = checksumArgSerial.ToLower();

            //Hash param by SHA256 for generate checksum
            var checksum = SecurityUtil.Sha256(checksumArgSerial);
            return checksum;
        }

        private bool ValidHeader(ActionExecutingContext context)
        {
            var isSuccess = false;

            if (!context.HttpContext.Request.Headers.TryGetValue("checksum", out _checksum))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.cks.GetHashCode(),
                    message = $"{ErrorCodeEnum.cks.GetDesc()}, Request Field: CheckSum",
                };
                var res_timestamp = DateTime.Now.ToUnixTimestamp().ToString();
                context.HttpContext.Response.Headers.Add("timestamp", res_timestamp);
                context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, res_body));
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("timestamp", out _timestamp))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.timestamp.GetHashCode(),
                    message = $"{ErrorCodeEnum.timestamp.GetDesc()}, Request Field: timestamp",
                };
                var res_timestamp = DateTime.Now.ToUnixTimestamp().ToString();
                context.HttpContext.Response.Headers.Add("timestamp", res_timestamp);
                context.HttpContext.Response.Headers.Add("checksum", GenChecksum(res_timestamp, res_body));
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            return isSuccess = true;
        }
    }
}
