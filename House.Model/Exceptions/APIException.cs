using House.Model.Enums;
using House.Model.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace House.Model.Exceptions
{
    public class APIException : Exception
    {
        private ErrorCodeEnum _code;
        private int _retCode;
        private string _retMsg;
        private string _logMsg;
        
        public virtual ErrorCodeEnum Code { get { return _code; } }
        public virtual int RetCode { get { return _retCode; } }
        public virtual string RetMsg { get { return _retMsg; } }
        public virtual string LogMsg { get { return _logMsg; } }
        public virtual JObject ResParams { get; set; }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼</param>
        public APIException(JObject resParams, ErrorCodeEnum code = ErrorCodeEnum.failed) :
            this(code, code.GetDesc(), new Exception())
        {
            ResParams = resParams;
        }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼</param>
        public APIException(ErrorCodeEnum code = ErrorCodeEnum.failed) :
            this(code, code.GetDesc(), new Exception())
        {
        }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼</param>
        /// <param name="ex">內部錯誤內容</param>
        public APIException(ErrorCodeEnum code, Exception ex) :
            this(code, code.GetDesc(), ex)
        {
        }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼 for response</param>
        /// <param name="logMsg">內部錯誤內容 for log</param>
        public APIException(ErrorCodeEnum code, string logMsg) :
            this(code, logMsg, new Exception())
        {
        }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼 for response</param>
        /// <param name="resMsg">錯誤內容 for response</param>
        /// <param name="logMsg">內部錯誤內容 for log</param>
        public APIException(ErrorCodeEnum code, string resMsg, string logMsg) : base(logMsg, new Exception())
        {
            _code = code;
            _retCode = code.GetHashCode();
            _retMsg = resMsg;
            _logMsg = logMsg;
        }

        /// <summary>
        /// 系統應用程式的錯誤訊息。
        /// </summary>
        /// <param name="code">錯誤代碼</param>
        /// <param name="message">自訂錯誤訊息</param>
        /// <param name="ex">內部錯誤內容</param>
        public APIException(ErrorCodeEnum code, string message, Exception ex) :
            base(message, ex)
        {
            _code = code;
            _retCode = code.GetHashCode();
            _retMsg = _logMsg = code.GetDesc();
        }
    }
}
