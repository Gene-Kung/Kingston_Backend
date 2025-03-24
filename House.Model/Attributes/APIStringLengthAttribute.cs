using House.Model.Enums;
using House.Model.Extensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace House.Model.Attributes
{
    public class APIStringLengthAttribute : StringLengthAttribute
    {
        /// <summary>
        /// Http 欄位長度
        /// </summary>
        /// <param name="maximumLength">最大值</param>
        /// <param name="httpMethods">預設GET, POST, PATCH, DELETE</param>
        public APIStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
            ErrorMessage = ErrorCodeEnum.req_leng.GetHashCode().ToString();
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!base.IsValid(value))
            {
                return new ValidationResult($"{string.Format(ErrorCodeEnum.req_leng.GetDesc(), MaximumLength)}, 欄位: {validationContext.DisplayName}");
            }

            return ValidationResult.Success;
        }
    }
}
