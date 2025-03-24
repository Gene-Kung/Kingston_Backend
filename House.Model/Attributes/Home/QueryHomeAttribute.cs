using House.Model.API.Home;
using House.Model.Enums;
using House.Model.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace House.Model.Attributes.Home
{
    public class QueryHomeAttribute : ValidationAttribute
    {
        public QueryHomeAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var reqModel = validationContext.ObjectInstance as ReqQueryHomeModel;
            var message = "機能、實價登錄請擇一選填";

            if (reqModel == null)
                return new ValidationResult($"{ErrorCodeEnum.req_null.GetDesc()}, 欄位: {message}");

            if ((reqModel.env_categories == null || !reqModel.env_categories.Any()) && reqModel.price_govs == null)
                return new ValidationResult($"{ErrorCodeEnum.req_null.GetDesc()}, 欄位: {message}");

            if ((reqModel.env_categories == null || !reqModel.env_categories.Any()) && reqModel.price_govs != null)
            {
                var hasValue = false;
                if (reqModel.price_govs.unit_price_start != null) hasValue = true;
                else if (reqModel.price_govs.age != null) hasValue = true;
                else if (reqModel.price_govs.building_pattern_room != null) hasValue = true;
                else if (reqModel.price_govs.build_state != null) hasValue = true;
                else if (reqModel.price_govs.shifting_floor != null) hasValue = true;

                if (!hasValue)
                    return new ValidationResult($"{ErrorCodeEnum.req_null.GetDesc()}, 欄位: {message}");
            }

            return ValidationResult.Success;
        }
    }
}
