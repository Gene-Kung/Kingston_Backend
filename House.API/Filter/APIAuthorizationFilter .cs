using House.Model;
using House.Model.API;
using House.Model.Enums;
using House.Model.Extensions;
using House.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace House.API.Filter
{
    public class APIAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        private readonly string _policy;
        private readonly List<string> _roles;
        private readonly AppSettings _appSettings;
        //private readonly MemberService _memberService;
        private readonly UserService _userService;
        private readonly SecurityService _securityService;

        public APIAuthorizationFilter(IOptions<AppSettings> appSettings,
            //MemberService memberService,
            UserService userService,
            SecurityService securityService,
            PolicyEnum policy)
        {
            _appSettings = appSettings.Value;
            //_memberService = memberService;
            _userService = userService;
            _securityService = securityService;
            _policy = policy.ToString();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_policy == PolicyEnum.User.ToString() && 
                string.IsNullOrEmpty(context.HttpContext.Request.Headers["Authorization"].ToString()))
            {
                //User 權限，未登入故沒有 Token 可以解析
            }
            else
            {
                var jwt = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty).Trim();

                var validateParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _securityService.GetPublicKey(),
                    ValidateIssuer = true,
                    ValidIssuer = _appSettings.JWTDefault.iss,
                    ValidateAudience = true,
                    ValidAudience = _appSettings.JWTDefault.aud,
                    //ValidateLifetime = true,
                    //ClockSkew = TimeSpan.FromMinutes(5)
                };

                var validresult = ValidateToken(jwt, validateParameters);

                if (validresult != ErrorCodeEnum.success)
                {
                    var code = validresult.GetHashCode();
                    var message = validresult.GetDesc();
                    var res = new ResBaseBodyModel()
                    {
                        code = code,
                        message = message
                    };
                    context.Result = new JsonResult(res);
                    return;
                }

                var payload = Jose.JWT.Payload(jwt);
                var jwtPayLoad = JsonConvert.DeserializeObject<JWTPayload>(payload);

                //取得MemberID
                context.HttpContext.Items["MemberID"] = jwtPayLoad.sub;
                context.HttpContext.Items["RoleIDs"] = jwtPayLoad.role;

                //var token = _memberService.GetToken(jwtPayLoad.sub);
                //var token = _userService.GetToken(jwtPayLoad.sub);
                
                //if (token != jwt)
                //{
                //    var code = ErrorCodeEnum.token_invalid.GetHashCode();
                //    var message = ErrorCodeEnum.token_invalid.GetDesc();
                //    var res = new ResBaseBodyModel()
                //    {
                //        code = code,
                //        message = message
                //    };
                //    context.Result = new JsonResult(res);
                //    return;
                //}

                if (!ValidatePolicy(context, jwtPayLoad))
                {
                    var code = ErrorCodeEnum.permission.GetHashCode();
                    var message = ErrorCodeEnum.permission.GetDesc();
                    var res = new ResBaseBodyModel()
                    {
                        code = code,
                        message = message
                    };
                    context.Result = new JsonResult(res);
                    return;
                }
                //取得sign
                //先詢問Redis沒有的話再進DB去查
                //context.HttpContext.Items[sign] = sign;
            }
        }

        private ErrorCodeEnum ValidateToken(string token, TokenValidationParameters validationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return ErrorCodeEnum.success;
            }
            catch (SecurityTokenExpiredException ex) { return ErrorCodeEnum.token_expired; }
            catch (Exception ex) { return ErrorCodeEnum.token; }
        }

        private bool ValidatePolicy(AuthorizationFilterContext context, JWTPayload jwtPayLoad)
        {
            var claims = jwtPayLoad.role.Select(x => new Claim(ClaimTypes.Role, x));
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = authorizationService.AuthorizeAsync(principal, null, _policy).Result;
            return authorizationResult.Succeeded;
        }
    }
}
