using House.Model.Enums;
using House.Model.Extensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace House.Model.Attributes
{
    public class APIRequiredAttribute : RequiredAttribute
    {
        private readonly AppSettings _appSettings;

        /// <summary>
        /// 預設全部檢查
        /// </summary>
        public APIRequiredAttribute() : base()
        {
            AllowEmptyStrings = false;

            //Enum ErrorCode 
            ErrorMessage = ErrorCodeEnum.req_null.GetHashCode().ToString();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!base.IsValid(value))
            {
                return new ValidationResult($"{ErrorCodeEnum.req_null.GetDesc()}, 欄位: {validationContext.DisplayName}");
            }


            return ValidationResult.Success;
        }
    }
}
