using System;

namespace falcon.cmtracker
{

    public class UnixTimestampHelper
    {
        public static long getLatestWeeklyServerResetTimestampInSeconds()
        {
            TimeSpan utcMinus6Offset = TimeSpan.FromHours(-6);
            DateTimeOffset nowInUtcMinus6 = DateTime.UtcNow.Add(utcMinus6Offset);

            int daysSinceMonday = (int)nowInUtcMinus6.DayOfWeek - (int)DayOfWeek.Monday;
            //Adjust if current day is Sunday
            if (daysSinceMonday < 0)
            {
                daysSinceMonday = 7;
            }
            DateTimeOffset latestMonday = nowInUtcMinus6.AddDays(-daysSinceMonday);

            /** Set the time to weekly reset (01:30:00) of the latest Monday in UTC-6
             * as documented in https://wiki.guildwars2.com/wiki/Server_reset#Weekly_reset.
             */
            DateTimeOffset latestMondayUtcMinus6 = new DateTimeOffset(
                latestMonday.Year,
                latestMonday.Month,
                latestMonday.Day,
                1, 30, 0,
                utcMinus6Offset);

            return latestMondayUtcMinus6.ToUnixTimeSeconds();
        }
    }
}
