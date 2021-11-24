using System;

namespace Kentico.Xperience.Intercom
{
    internal static class DateTimeHelper
    {
        private static readonly DateTime unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts UNIX time stamp to local time.
        /// </summary>
        /// <param name="timeStamp">Seconds since UNIX epoch</param>
        /// <returns>DateTime in local time.</returns>
        public static DateTime UnixTimeStampToDateTime(double timeStamp)
        {
            var dt = unixStartTime.AddSeconds(timeStamp);

            return dt.ToLocalTime();
        }
    }
}
