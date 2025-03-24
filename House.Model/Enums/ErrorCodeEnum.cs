using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.Enums
{
    public enum ErrorCodeEnum
    {
        //0~99 基本Error
        //100~199 會員 
        //200~299 訂單(含藍新金流)
        #region 0~99 基本Error
        [Description("成功")]
        success = 0,

        [Description("系統異常，請聯絡客服人員")]
        failed = 1,

        [Description("欄位不能為空")]
        req_null = 2,

        [Description("欄位值錯誤")]
        req_value = 3,

        [Description("欄位字數錯誤，超過{0}字")]
        req_leng = 4,

        [Description("版本錯誤")]
        version = 5,

        //[Description("權限異常")]
        //permission = 6,

        [Description("資料更新錯誤")]
        db_update = 7,

        [Description("資料刪除錯誤")]
        db_delete = 8,

        [Description("資料新增錯誤")]
        db_insert = 9,

        [Description("資料查詢錯誤")]
        db_select = 10,

        [Description("Timestamp錯誤")]
        timestamp = 11,

        [Description("Platform錯誤")]
        platform = 12,

        [Description("版本未支援")]
        version_disabled = 13,

        [Description("平台未支援")]
        platform_disabled = 14,

        [Description("查無資料")]
        not_found_data = 15,

        [Description("操作頻率異常")]
        request_too_much = 16,

        [Description("CheckSum錯誤")]
        cks = 99,
        #endregion

        #region 100~199 會員 
        [Description("Email不存在")]
        not_found_email = 100,

        [Description("Email驗證碼錯誤")]
        invalid_verification_code = 101,

        [Description("Email驗證碼逾時")]
        expired_verification_code = 102,

        [Description("Email未驗證成功")]
        invalid_email = 103,

        [Description("Email已被使用")]
        email_exist = 104,

        [Description("Token異常")]
        token = 105,

        [Description("Token無效")]
        token_invalid = 106,

        [Description("OAuth使用者憑證API錯誤")]
        oauth_accesstoken_api = 107,

        [Description("OAuth使用者資訊API錯誤")]
        oauth_userinfo_api = 108,

        [Description("會員編號不存在")]
        not_found_memberid = 109,

        [Description("密碼錯誤")]
        pwd = 110,

        [Description("公司不存在")]
        not_found_company = 111,

        [Description("Token逾時")]
        token_expired = 112,

        [Description("權限不足")]
        permission = 113,

        /// <summary>
        /// request field invalid 通用回傳代碼
        /// 供前端判斷使用
        /// </summary>
        req_invalid = 114,

        [Description("欄位字數錯誤，不得少於{0}字")]
        req_min_leng = 115,

        [Description("欄位項目錯誤，不得少於{0}項")]
        req_min_leng_list = 116,

        [Description("欄位字數錯誤，不得超過{0}字")]
        req_max_leng = 117,

        [Description("欄位項目錯誤，不得超過{0}項")]
        req_max_leng_list = 118,
        #endregion

        #region 200~299 訂單(含藍新金流)
        [Description("藍新金流CheckCode錯誤")]
        newebpay_checkcode = 200,

        [Description("藍新金流Status錯誤")]
        newebpay_status = 201,

        [Description("藍新金流請款錯誤")]
        newebpay_capture = 202,

        [Description("藍新金流取消請款錯誤")]
        newebpay_cancel_capture = 203,

        [Description("藍新金流退貨錯誤")]
        newebpay_refund = 204,

        [Description("藍新金流取消退貨錯誤")]
        newebpay_cancel_refund = 205,
        #endregion

        #region 300~399 首頁查詢相關
        [Description("查無此地址")]
        not_found_address = 300,

        [Description("TGos API 超過30萬次")]
        tgos_exceeded_limit = 301,
        #endregion

        #region 400~499 Service API
        [Description("code 錯誤")]
        service_code_invalid = 400,

        [Description("client_secret 錯誤")]
        service_client_secret = 401,
        #endregion

        #region 報表
        [Description("報表產生已達上限")]
        report_upper_limit = 500,
        #endregion

        [Description("未定義錯誤")]
        undefined = 999
    }
}
