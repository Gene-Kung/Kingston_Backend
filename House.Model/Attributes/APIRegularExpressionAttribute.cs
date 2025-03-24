using House.Model.Enums;
using House.Model.Extensions;
using System.ComponentModel.DataAnnotations;

namespace House.Model.Attributes
{
    public class APIRegularExpressionAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Http 欄位長度
        /// </summary>
        /// <param name="maximumLength">最大值</param>
        /// <param name="httpMethods">預設GET, POST, PATCH, DELETE</param>
        public APIRegularExpressionAttribute(string pattern) : base(pattern)
        {
            ErrorMessage = ErrorCodeEnum.req_value.GetHashCode().ToString();
        }

        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!base.IsValid(value))
                //return new ValidationResult(ErrorCodeEnum.req_value.GetHashCode().ToString());
                return new ValidationResult($"{ErrorCodeEnum.req_value.GetDesc()}, 欄位: {validationContext.MemberName}");

            return ValidationResult.Success;
        }
    }
}
