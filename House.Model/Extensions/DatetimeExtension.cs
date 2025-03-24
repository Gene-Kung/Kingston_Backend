using System;
using System.Collections.Generic;
using System.Text;

namespace House.Model.Extension
{
    public static class DatetimeExtension
    {
        public static double ToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// 轉換Unit time
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long ToUnixTimestampLong(this DateTime datetime)
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            return (long)(datetime - UnixEpoch).TotalSeconds;
        }

        public static int GetBuildingAge(this DateTime date)
        {
            var today = DateTime.Today;

            int years = today.Year - date.Year;

            if (today.Month < date.Month || (today.Month == date.Month && today.Day < date.Day))
            {
                years--;
            }

            return years;
        }
    }
}
