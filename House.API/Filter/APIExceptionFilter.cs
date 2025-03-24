using House.Model.API;
using House.Model.Enums;
using House.Model.Exceptions;
using House.Model.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace House.API.Filter
{
    public class APIExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        private static readonly Logger _logger = LogManager.GetLogger("insert_error_log");
        public override void OnException(ExceptionContext context)
        {
            //var logEventInfo = new LogEventInfo(LogLevel.Error, _logger.Name, context.Exception.Message);
            //logEventInfo.Properties["stack_trace"] = context.Exception.StackTrace;
            //logEventInfo.Properties["inner_messgae"] = context.Exception.InnerException?.Message;
            //logEventInfo.Properties["inner_stack_trace"] = context.Exception.InnerException?.StackTrace;
            //_logger.Log(logEventInfo);

            if (context.Exception is APIException)
            {
                // 處理異常並設置適當的回應結果
                var exception = (APIException)context.Exception;
                var resBase = new ResBaseBodyModel
                {
                    code = exception.RetCode,
                    message = exception.RetMsg,
                };
                var result = new ObjectResult(resBase);
                result.StatusCode = 200;
                context.Result = result;
            }
            else //其他所有的Exception
            {
                // 處理異常並設置適當的回應結果
                var resBase = new ResBaseBodyModel()
                {
                    code = ErrorCodeEnum.failed.GetHashCode(),
                    message  = ErrorCodeEnum.failed.GetDesc()
                };
                var result = new ObjectResult(resBase);
                result.StatusCode = 200;
                context.Result = result;
            }
            base.OnException(context);
        }
    }
}
