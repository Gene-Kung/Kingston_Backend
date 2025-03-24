using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace House.Model.Enums
{
    public enum PolicyEnum
    {
        [Description("未登入使用者以上權限")]
        User = 20,

        [Description("免費會員以上權限")]
        FreeMember = 30,

        [Description("付費會員以上權限")]
        Member = 40,

        [Description("VIP會員以上權限")]
        VIP = 50,

        [Description("系統操作人員以上權限")]
        Customer = 60,

        [Description("系統操作人員以上權限")]
        SystemOperator = 75,

        [Description("僅系統管理員")]
        AdminOnly = 99,
    }
}
