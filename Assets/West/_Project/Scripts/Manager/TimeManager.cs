using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public DateTime UtcNowLocal()
    {
        return DateTime.UtcNow.ToLocalTime();
    }

    public long UtcNowTimeStamp()
    {
        return DateTimeToTimeStamp(UtcNow());
    }

    public long UtcNowTimeStampLocal()
    {
        return DateTimeToTimeStamp(UtcNowLocal());
    }

    public long DefaultTimeStamp()
    {
        return DateTimeToTimeStamp(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    public DayOfWeek UtcDayOfWeek()
    {
        return UtcNow().DayOfWeek;
    }

    public DayOfWeek UtcDayOfWeekLocal()
    {
        return UtcNowLocal().DayOfWeek;
    }

    public DateTime Tommorrow()
    {
        var now = UtcNow();
        return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
    }

    public DateTime TommorrowLocal()
    {
        var now = UtcNowLocal();
        return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
    }

    public DateTime AddMinute(double minute)
    {
        var now = UtcNow();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc).AddMinutes(minute);
    }

    public DateTime AddMinuteLocal(double minute)
    {
        var now = UtcNowLocal();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc).AddMinutes(minute);
    }

    public DateTime AddSeconds(double seconds)
    {
        var now = UtcNow();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc).AddSeconds(seconds);
    }

    public DateTime AddSecondsLocal(double seconds)
    {
        var now = UtcNowLocal();
        return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc).AddSeconds(seconds);
    }

    public long TommorrowTimeStamp()
    {
        return DateTimeToTimeStamp(Tommorrow());
    }

    public long TommorrowTimeStampLocal()
    {
        return DateTimeToTimeStamp(TommorrowLocal());
    }

    public long AddMinuteTimeStamp(double minute)
    {
        return DateTimeToTimeStamp(AddMinute(minute));
    }

    public long AddMinuteTimeStampLocal(double minute)
    {
        return DateTimeToTimeStamp(AddMinuteLocal(minute));
    }

    public long AddSecondsTimeStamp(double seconds)
    {
        return DateTimeToTimeStamp(AddSeconds(seconds));
    }

    public long AddSecondsTimeStampLocal(double seconds)
    {
        return DateTimeToTimeStamp(AddSecondsLocal(seconds));
    }

    public DateTime NextMonday()
    {
        var now = UtcNow();
        for (var i = 1; i <= 7; i++)
        {
            var cur = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(i);
            if (cur.DayOfWeek == DayOfWeek.Monday) return cur;
        }

        return now;
    }

    public DateTime NextMondayLocal()
    {
        var now = UtcNowLocal();
        for (var i = 1; i <= 7; i++)
        {
            var cur = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(i);
            if (cur.DayOfWeek == DayOfWeek.Monday) return cur;
        }

        return now;
    }

    public long NextMondayTimeStamp()
    {
        return DateTimeToTimeStamp(NextMonday());
    }

    public long NextMondayTimeStampLocal()
    {
        return DateTimeToTimeStamp(NextMondayLocal());
    }

    public TimeSpan GetTimeSpan(long targetTimeStamp)
    {
        return TimeStampToDateTime(targetTimeStamp) - UtcNow();
    }

    public TimeSpan GetTimeSpanLocal(long targetTimeStamp)
    {
        return TimeStampToDateTimeLocal(targetTimeStamp) - UtcNowLocal();
    }

    public TimeSpan GetTimeSpan(long baseTimeStamp, long targetTimeStamp)
    {
        return TimeStampToDateTime(targetTimeStamp) - TimeStampToDateTime(baseTimeStamp);
    }

    public TimeSpan GetTimeSpanLocal(long baseTimeStamp, long targetTimeStamp)
    {
        return TimeStampToDateTimeLocal(targetTimeStamp) - TimeStampToDateTimeLocal(baseTimeStamp);
    }

    public TimeSpan GetTimeSpanFromNow(long targetTimeStamp)
    {
        return UtcNow() - TimeStampToDateTime(targetTimeStamp);
    }

    public TimeSpan GetTimeSpanFromNowLocal(long targetTimeStamp)
    {
        return UtcNowLocal() - TimeStampToDateTimeLocal(targetTimeStamp);
    }

    public TimeSpan GetTimeSpanFromTarget(long targetTimeStamp)
    {
        return TimeStampToDateTime(targetTimeStamp) - UtcNow();
    }

    public TimeSpan GetTimeSpanFromTargetLocal(long targetTimeStamp)
    {
        return TimeStampToDateTimeLocal(targetTimeStamp) - UtcNowLocal();
    }

    public long GetLeftTimestampFromNow(long targetTimestamp)
    {
        return DateTimeToTimeStamp(UtcNow()) - targetTimestamp;
    }

    public long GetLeftTimestampFromNowLocal(long targetTimestamp)
    {
        return DateTimeToTimeStamp(UtcNowLocal()) - targetTimestamp;
    }

    public DateTime GetLeftDateTimeFromNow(long targetTimestamp)
    {
        var leftTimestamp = GetLeftTimestampFromNow(targetTimestamp);
        return TimeStampToDateTime(leftTimestamp);
    }

    public DateTime GetLeftDateTimeFromNowLocal(long targetTimestamp)
    {
        var leftTimestamp = GetLeftTimestampFromNowLocal(targetTimestamp);
        return TimeStampToDateTimeLocal(leftTimestamp);
    }

    public long DateTimeToTimeStamp(DateTime value)
    {
        return ((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    public DateTime TimeStampToDateTime(long value)
    {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(value);

        return dt;
    }

    public DateTime TimeStampToDateTimeLocal(long value)
    {
        var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddSeconds(value);

        return dt.ToLocalTime();
    }

    // 스펙 데이터의 datetime 데이터 문자열을 DateTime 형식으로 변환
    public DateTime ChangeDateStringToDateTime(string dateTimeString)
    {
        return DateTime.Parse(dateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    // 스펙 데이터의 datetime 데이터 문자열을 long 형식으로 변환
    public long ChangeDateStringToTimeStamp(string dateTimeString)
    {
        var targetDateTime = DateTime.Parse(dateTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);

        return DateTimeToTimeStamp(targetDateTime);
    }

    public int DateNumber =>
        DateTime.UtcNow.Year * 10000 + DateTime.UtcNow.Month * 100 +
        DateTime.UtcNow.Day;

    public bool IsValidTimeNow(long startTimeStamp, long endTimeStamp)
    {
        return startTimeStamp > UtcNowTimeStamp() && UtcNowTimeStamp() < endTimeStamp;
    }

    public bool IsValidTimeNowLocal(long startTimeStamp, long endTimeStamp)
    {
        return startTimeStamp > UtcNowTimeStampLocal() && UtcNowTimeStampLocal() < endTimeStamp;
    }
}