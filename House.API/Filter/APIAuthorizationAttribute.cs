using House.DAL.Dapper;
using House.Model;
using House.Model.Enums;
using House.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace House.API.Filter
{
    public class APIAuthorizationAttribute : Attribute, IFilterFactory
    {
        public PolicyEnum Policy { get; }


        public APIAuthorizationAttribute()
        {
            Policy = PolicyEnum.User;
        }

        public APIAuthorizationAttribute(PolicyEnum policy)
        {
            Policy = policy;
        }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            //var memberService = serviceProvider.GetRequiredService<MemberService>();
            var memberService = serviceProvider.GetRequiredService<UserService>();
            var SecurityService = serviceProvider.GetRequiredService<SecurityService>();
            return new APIAuthorizationFilter(options, memberService, SecurityService, Policy);
        }
    }
}
