using House.Model.Enums;
using House.Model.Extension;
using House.Model.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;


namespace House.Model.API
{
    public class ResBaseBodyModel
    {
        public int code { get; set; } = ErrorCodeEnum.success.GetHashCode();
        public string message { get; set; } = ErrorCodeEnum.success.GetDesc();
    }
}
