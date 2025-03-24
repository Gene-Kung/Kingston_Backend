using System.ComponentModel;

namespace House.Model.Enums
{
    public enum OrderStatusEnum
    {
        [Description("建立")]
        Created = 0,

        [Description("授權")]
        Authorization = 1,

        [Description("取消授權")]
        CancelAuthorization = 2,

        [Description("請款")]
        Capture = 3,

        [Description("取消請款")]
        CancelCapture = 4,

        [Description("退款")]
        Refund = 5,

        [Description("取消退款")]
        CancelRefund = 6,
    }
}
