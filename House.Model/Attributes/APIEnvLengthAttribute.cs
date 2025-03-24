using House.Model.Enums;
using House.Model.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace House.Model.Attributes
{
    public class APIEnvLengthAttribute : ValidationAttribute
    {
        private readonly int _userLen;
        private readonly int _freeMemberLen;
        private readonly int _memberLen;
        private readonly int _vipLen;
        private readonly int _customerLen;
        private readonly int _systemOperatorLen;
        private readonly int _adminLen;
        /// <summary>
        /// Http 欄位長度
        /// </summary>
        /// <param name="maximumLength">最大值</param>
        /// <param name="httpMethods">預設GET, POST, PATCH, DELETE</param>
        public APIEnvLengthAttribute(
            int userLen,
            int freeMemberLen,
            int memberLen,
            int vipLen,
            int customerLen,
            int systemOperatorLen,
            int adminLen
        )
        {
            ErrorMessage = ErrorCodeEnum.req_max_leng_list.GetHashCode().ToString();
            _userLen = userLen;
            _freeMemberLen = freeMemberLen;
            _memberLen = memberLen;
            _vipLen = vipLen;
            _customerLen = customerLen;
            _systemOperatorLen = systemOperatorLen;
            _adminLen = adminLen;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var httpContextAccessor = (HttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            var userRoleIDs = (List<string>)httpContextAccessor.HttpContext.Items["RoleIDs"];

            var list = (IList)value;

            if (userRoleIDs == null) //userRoleIDs == null 為未登入，所以是 User
            {
                if (list != null && list.Count > _userLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _userLen)}, 欄位: {validationContext.DisplayName}。加入會員即可查詢{_freeMemberLen}項機能。");
            }
            else if (userRoleIDs.Contains(Role.FreeMember.GetHashCodeString()))
            {
                if (list != null && list.Count > _freeMemberLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _freeMemberLen)}, 欄位: {validationContext.DisplayName}。升級付費會員即可查詢{_memberLen}項機能。");
            }
            else if (userRoleIDs.Contains(Role.Member.GetHashCodeString()))
            {
                if (list != null && list.Count > _memberLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _memberLen)}, 欄位: {validationContext.DisplayName}。升級VIP會員即可查詢{_vipLen}機能。");
            }
            else if (userRoleIDs.Contains(Role.VIP.GetHashCodeString()))
            {
                if (list != null && list.Count > _vipLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _vipLen)}, 欄位: {validationContext.DisplayName} ");
            }
            else if (userRoleIDs.Contains(Role.Customer.GetHashCodeString()))
            {
                if (list != null && list.Count > _customerLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _customerLen)}, 欄位: {validationContext.DisplayName}");
            }
            else if (userRoleIDs.Contains(Role.SystemOperator.GetHashCodeString()))
            {
                if (list != null && list.Count > _systemOperatorLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _systemOperatorLen)}, 欄位: {validationContext.DisplayName}");
            }
            else if (userRoleIDs.Contains(Role.Admin.GetHashCodeString()))
            {
                if (list != null && list.Count > _adminLen)
                    return new ValidationResult($"{string.Format(ErrorCodeEnum.req_max_leng_list.GetDesc(), _adminLen)}, 欄位: {validationContext.DisplayName}");
            }
            else
                return new ValidationResult(ErrorCodeEnum.permission.GetDesc());

            return ValidationResult.Success;
        }
    }
}
