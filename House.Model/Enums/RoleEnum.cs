using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.Enums
{
    /// <summary>
    /// 需與資料表Role同步
    /// </summary>
    public enum Role
    {
        [Description("系統管理員")]
        Admin = 1,

        [Description("系統操作員")]
        SystemOperator = 2,

        [Description("客服人員")]
        Customer = 3,

        [Description("免費會員")]
        FreeMember = 4,

        [Description("付費會員")]
        Member = 5,

        [Description("VIP會員")]
        VIP = 6,

        [Description("未登入使用者")]
        User = 7,

    }
}
