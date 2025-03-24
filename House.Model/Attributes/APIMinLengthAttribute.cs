using House.Model.Enums;
using House.Model.Extensions;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace House.Model.Attributes
{
    public class APIMinLengthAttribute : MinLengthAttribute
    {
        /// <summary>
        /// Http 欄位長度
        /// </summary>
        /// <param name="maximumLength">最大值</param>
        /// <param name="httpMethods">預設GET, POST, PATCH, DELETE</param>
        public APIMinLengthAttribute(int length) : base(length)
        {
            ErrorMessage = ErrorCodeEnum.req_leng.GetHashCode().ToString();
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!base.IsValid(value))
            {
                if (value is IList)
                {
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_min_leng_list.GetDesc(), Length)}, 欄位: {validationContext.DisplayName}");
                    //if (list.Count < Length)
                    //{
                    //    return new ValidationResult($"The field {validationContext.DisplayName} must have at least {_minLength} elements.");
                    //}
                }
                else
                {
                    // 如果不是 IList，則返回一個錯誤
                    //return new ValidationResult($"The field {validationContext.DisplayName} is not a valid list.");
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_min_leng.GetDesc(), Length)}, 欄位: {validationContext.DisplayName}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
