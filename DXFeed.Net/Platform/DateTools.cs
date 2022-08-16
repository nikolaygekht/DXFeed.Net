using System;
using System.Collections.Generic;
using System.Text;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Date/time processing for DXFeed
    /// </summary>
    public static class DateTools
    {
        /// <summary>
        /// How to treat unspecified date (Utc or Local)
        /// </summary>
        public static DateTimeKind UnspecifiedIs { get; set; } = DateTimeKind.Utc;

        private readonly static DateTime gBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert date to dxfeed format
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long ToDXFeed(this DateTime datetime)
        {
            if (datetime.Kind == DateTimeKind.Unspecified)
                datetime = new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, UnspecifiedIs);

            if (datetime.Kind == DateTimeKind.Local)
                datetime = datetime.ToUniversalTime();

            return (long)(datetime - gBase).TotalMilliseconds;
        }

        /// <summary>
        /// Converts DXFeed time to C# time
        /// </summary>
        /// <param name="value"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static DateTime FromDXFeed(this long value, DateTimeKind kind = DateTimeKind.Utc)
        {
            var v = gBase.AddMilliseconds(value);
            if (kind == DateTimeKind.Local)
                v = v.ToLocalTime();
            return v;
        }
    }
}
