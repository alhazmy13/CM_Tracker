using System;

namespace falcon.cmtracker
{

    public class UnixTimestampHelper
    {
        public static long getLatestWeeklyServerResetTimestampInSeconds()
        {            
            DateTimeOffset nowUtc = DateTimeOffset.UtcNow;       

            int daysSinceMonday = (int)nowUtc.DayOfWeek - (int)DayOfWeek.Monday;
            //Adjust if current day is Sunday
            if (daysSinceMonday < 0)
            {                
                daysSinceMonday = 7;
            }
            DateTimeOffset latestMondayUtc = nowUtc.AddDays(-daysSinceMonday);

            TimeSpan utcMinus6Offset = TimeSpan.FromHours(-6);

            /** Set the time to weekly reset (01:30:00) of the latest Monday in UTC-6
             * as documented in https://wiki.guildwars2.com/wiki/Server_reset#Weekly_reset.
             */
            DateTimeOffset latestMondayUtcMinus6 = new DateTimeOffset(
                latestMondayUtc.Year,
                latestMondayUtc.Month,
                latestMondayUtc.Day,
                1, 30, 0, 
                utcMinus6Offset);
            
            return latestMondayUtcMinus6.ToUnixTimeSeconds();
        }
    }
}
