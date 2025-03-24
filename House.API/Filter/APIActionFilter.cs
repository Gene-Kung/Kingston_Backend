using House.Model;
using House.Model.API;
using House.Model.Enums;
using House.Model.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace House.API.Filter
{
    public class APIActionFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly AppSettings _appSettings;
        private StringValues _version;
        private StringValues _platfrom;
        private List<string> _enabledVersion = new List<string> { "1.0.0" }; //之後改由config取得
        private List<string> _enabledPlatform = new List<string> { "app", "web", "support" };

        public APIActionFilter(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ValidHeader(context))
                return;

            if (_appSettings.Validating.PropAttribute && !context.ModelState.IsValid)
            {
                var messageList = new List<string>();
                foreach (var key in context.ModelState.Keys) 
                {
                    var value = context.ModelState[key].Errors.First().ErrorMessage;
                    messageList.Add(value);
                }
                var res = new ResBaseBodyModel()
                {
                    code = ErrorCodeEnum.req_invalid.GetHashCode(),
                    message = string.Join(" \n ", messageList)
                };
                context.Result = new JsonResult(res);
                return;
            }
        }

        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        public override async void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }

        public override async void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }

        private bool ValidHeader(ActionExecutingContext context)
        {
            var isSuccess = false;

            if (!context.HttpContext.Request.Headers.TryGetValue("version", out _version))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.version.GetHashCode(),
                    message = $"{ErrorCodeEnum.version.GetDesc()}, Request Field: version",
                };
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            if (!_enabledVersion.Contains(_version))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.version_disabled.GetHashCode(),
                    message = $"{ErrorCodeEnum.version_disabled.GetDesc()}, Request Field: version",
                };
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("platform", out _platfrom))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.platform.GetHashCode(),
                    message = $"{ErrorCodeEnum.platform.GetDesc()}, Request Field: platform",
                };
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            if (!_enabledPlatform.Contains(_platfrom))
            {
                var res_body = new ResBaseBodyModel
                {
                    code = ErrorCodeEnum.platform_disabled.GetHashCode(),
                    message = $"{ErrorCodeEnum.platform_disabled.GetDesc()}, Request Field: platform",
                };
                context.Result = new JsonResult(res_body);
                return isSuccess;
            }

            return isSuccess = true;
        }
    }
}
