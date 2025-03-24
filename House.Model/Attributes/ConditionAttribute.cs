using House.Model;
using House.Model.API;
using House.Model.Enums;
using House.Model.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace House.Model.Attributes
{
    public class ConditionAttribute : Attribute
    {
        public ParamAttributeEnum Type { get; set; }
        public string TargetField { get; set; }
        public bool IsStart { get; set; }

        /// <summary>
        /// 模糊查詢
        /// </summary>
        /// <param name="isLike"></param>
        public ConditionAttribute(bool isLike = false) 
        {
            if (isLike) Type = ParamAttributeEnum.Like;
            else Type = ParamAttributeEnum.Equal;
        }

        /// <summary>
        /// 區間查詢 ex: 時間起迄，金額範圍
        /// </summary>
        /// <param name="targetField">DB資料表欄位名稱</param>
        /// <param name="isStart">True:開始參數, False:結束參數</param>
        public ConditionAttribute(string targetField, bool isStart)
        {
            Type = ParamAttributeEnum.Interval;
            TargetField = targetField;
            IsStart = isStart;
        }
    }
}
