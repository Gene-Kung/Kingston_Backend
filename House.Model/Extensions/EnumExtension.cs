using System;
using System.ComponentModel;
using System.Linq;

namespace House.Model.Extensions
{
    public static class EnumExtension
    {
        public static string GetDesc(this Enum e)
        {
            var field = e.GetType().GetField(e.ToString());
            var desc = (DescriptionAttribute)
                field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            return (desc != null) ? desc.Description : string.Empty;
        }

        public static string GetHashCodeString(this Enum e)
        {
            return e.GetHashCode().ToString();
        }
    }
}

