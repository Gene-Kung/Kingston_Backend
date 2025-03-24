using House.API.Controllers;
using House.Model.API;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using NLog;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLevel = NLog.LogLevel;

namespace House.API.Middleware
{
    /// <summary>
    /// 記錄所有的Request與Response
    /// </summary>
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Logger _logger = LogManager.GetLogger("insert_api_log");
        private static readonly Logger _error_logger = LogManager.GetLogger("insert_error_log");
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public LogMiddleware(RequestDelegate next, ILogger<BaseController> logger)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            //_logger = LogManager.GetLogger("insert_api_file");
        }

        public async Task Invoke(HttpContext context)
        {
            await HandleLogAsync(context);
        }

        private async Task HandleLogAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;
            string response_payload = string.Empty;
            try
            {
                await HandleRequestAsync(context);
                await _next(context);
                responseBody.Seek(0, SeekOrigin.Begin);
                response_payload = await new StreamReader(responseBody).ReadToEndAsync();
                await HandleResponseAsync(context, response_payload);
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                var logEventInfo = new LogEventInfo(LogLevel.Error, _error_logger.Name, ex.Message);
                logEventInfo.Properties["stack_trace"] = ex.StackTrace;
                logEventInfo.Properties["inner_messgae"] = ex.InnerException?.Message;
                logEventInfo.Properties["inner_stack_trace"] = ex.InnerException?.StackTrace;
                //_error_logger.Log(logEventInfo);

                if (!context.Response.HasStarted)
                {
                    var resBase = new ResBaseBodyModel();
                    if (ex is APIException)
                    {
                        // 處理異常並設置適當的回應結果
                        var exception = (APIException)ex;
                        resBase.code = exception.RetCode;
                        resBase.message = exception.RetMsg;
                    }
                    else //其他所有的Exception
                    {
                        resBase.code = ErrorCodeEnum.failed.GetHashCode();
                        resBase.message = ErrorCodeEnum.failed.GetDesc();
                    }
                    response_payload = JsonConvert.SerializeObject(resBase);
                    await HandleResponseAsync(context, response_payload);
                    var buffer = Encoding.UTF8.GetBytes(response_payload);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentLength = buffer.Length;
                    await originalBodyStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// Request Log
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task HandleRequestAsync(HttpContext context)
        {
            var request = context.Request;
            context.Items["Method"] = request.Method;
            context.Items["ClientIP"] = context.Connection.RemoteIpAddress.ToString().Replace("::1", "127.0.0.1");
            context.Items["Url"] = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            context.Items["RequestTime"] = DateTime.UtcNow;
            context.Items["ActionName"] = request.Path.ToString().Split('/').LastOrDefault();

            if (!request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) && !request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                var (strRequest, checksum) = await GetRequestBody(context);
                context.Items["RequestBody"] = strRequest;
                context.Items["CheckSum"] = checksum;
            }
        }

        /// <summary>
        /// Response LOG
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response_payload"></param>
        /// <returns></returns>
        private async Task HandleResponseAsync(HttpContext context, string response_payload)
        {
            var logEventInfo = new LogEventInfo(NLog.LogLevel.Info, _logger.Name, "API Log");
            logEventInfo.Properties["client_ip"] = context.Connection.RemoteIpAddress.ToString().Replace("::1", "127.0.0.1");
            logEventInfo.Properties["url"] = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
            logEventInfo.Properties["request_payload"] = context.Items["RequestBody"] != null ? context.Items["RequestBody"].ToString() : null;
            logEventInfo.Properties["request_time"] = context.Items["RequestTime"] != null ? Convert.ToDateTime(context.Items["RequestTime"]) : null;
            logEventInfo.Properties["response_payload"] = response_payload;
            logEventInfo.Properties["response_time"] = DateTime.UtcNow;
            logEventInfo.Properties["method"] = context.Request.Method;
            //_logger.Log(logEventInfo);
        }

        private async Task<(string, string)> GetRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            string strRequest = string.Empty;
            string checksum = string.Empty;
            dynamic payload = null;

            if (context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                await context.Request.Body.CopyToAsync(requestStream);
                strRequest = ReadStreamInChunks(requestStream);
                strRequest = strRequest.ToLower();
                payload = JsonConvert.DeserializeObject<dynamic>(strRequest, new JsonSerializerSettings { });
                strRequest = JsonConvert.SerializeObject(payload); //去除換行與回車
                checksum = payload?.checksum;
                context.Request.Body.Position = 0;
            }
            else if (context.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                strRequest = JsonConvert.SerializeObject(context.Request.Form);
            }
            else if (context.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                strRequest = JsonConvert.SerializeObject(context.Request.Form);
            }

            return (strRequest, checksum);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }

    public static class LogMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
