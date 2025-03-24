using System;
using System.Collections.Generic;
using System.Text;

namespace House.Model.Extension
{
    public static class DoubleExtension
    {
        public static DateTime ToDateTime(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
    }
}
