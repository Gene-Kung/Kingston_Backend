using House.Model;
using House.Model.API;
using House.Model.Enums;
using House.Model.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace House.Model.Attributes.Order.Query
{
    public class MemberAttribute : ValidationAttribute
    {
        private readonly string[] roleIDs = new string[] { 
            Role.Admin.GetHashCodeString(),
            Role.SystemOperator.GetHashCodeString(),
            Role.Customer.GetHashCodeString(),
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var model = (ResQueryOrderModel)validationContext.ObjectInstance;

            var memberID = (long)value;

            var httpContextAccessor = (HttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            var userRoleIDs = (List<string>)httpContextAccessor.HttpContext.Items["RoleIDs"];
            
            if (!roleIDs.Any(x => userRoleIDs.Contains(x)))
                return new ValidationResult(ErrorCodeEnum.permission.GetHashCodeString());

            return ValidationResult.Success;
        }
    }
}
