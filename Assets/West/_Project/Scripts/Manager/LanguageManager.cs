using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.BM.MVPWest;
using System.Text;
using UnityEngine;

public class LanguageManager : Singleton<LanguageManager>
{
    public LanguageType CurrentLanguageType { get; private set; } = LanguageType.NONE;

    // 언어 환경 세팅
    public void InitLanguage()
    {
        var settingLanguage = Preference.LoadPreference(Pref.LANGUAGE, (int)LanguageType.NONE);

        if (settingLanguage == (int)LanguageType.NONE) settingLanguage = (int)GetSystemLanguageType();

        SetGameLanguage((LanguageType)settingLanguage);
    }

    public void SetGameLanguage(LanguageType type)
    {
        CurrentLanguageType = type;

        Preference.SavePreference(Pref.LANGUAGE, (int)type);
    }

    public string GetLanguageText(string tokenKey)
    {
        return SpecDataManager.Instance.GetLanguageText(tokenKey, CurrentLanguageType);
    }

    public string GetTimeText(int targetTimeValue, TimeType type, bool isRemain)
    {
        var formatString = string.Empty;

        switch (type)
        {
            case TimeType.DAY:
                formatString = isRemain ? GetLanguageText("TIME_DAY_REMAIN") : GetLanguageText("TIME_DAY");
                return string.Format(formatString, targetTimeValue);
            case TimeType.HOUR:
                formatString = isRemain ? GetLanguageText("TIME_HOUR_REMAIN") : GetLanguageText("TIME_HOUR");
                return string.Format(formatString, targetTimeValue);
            case TimeType.MINUTE:
                formatString = isRemain ? GetLanguageText("TIME_MINUTE_REMAIN") : GetLanguageText("TIME_MINUTE");
                return string.Format(formatString, targetTimeValue);
            case TimeType.SECOND:
                formatString = isRemain ? GetLanguageText("TIME_SECOND_REMAIN") : GetLanguageText("TIME_SECOND");
                return string.Format(formatString, targetTimeValue);
        }

        return string.Empty;
    }

    public string GetTimeSpanFromNowText(long targetTimestamp)
    {
        var timeSpanData = TimeManager.Instance.GetTimeSpanFromNow(targetTimestamp);

        var timeTextList = new StringBuilder();

        if (timeSpanData.Days > 0) timeTextList.Append(GetTimeText(timeSpanData.Days, TimeType.DAY, false));

        if (timeSpanData.Hours > 0) timeTextList.Append(GetTimeText(timeSpanData.Hours, TimeType.HOUR, false));

        timeTextList.Append(GetTimeText(timeSpanData.Minutes, TimeType.MINUTE, true));

        return timeTextList.ToString();
    }

    public string GetTimeSpanFromTargetText(long targetTimestamp)
    {
        var targetTimeSpan = TimeManager.Instance.GetTimeSpanFromTarget(targetTimestamp);

        var timeTextList = new StringBuilder();

        var hasDays = targetTimeSpan.Days > 0;
        var hasHours = targetTimeSpan.Hours > 0;
        var hasMinutes = targetTimeSpan.Minutes > 0;

        if (hasDays) timeTextList.Append(GetTimeText(targetTimeSpan.Days, TimeType.DAY, false));

        if (hasHours) timeTextList.Append(GetTimeText(targetTimeSpan.Hours, TimeType.HOUR, false));

        if (hasMinutes) timeTextList.Append(GetTimeText(targetTimeSpan.Minutes, TimeType.MINUTE, true));

        if (!hasDays && !hasHours) // 1시간 미만으로 남은 경우 초 단위 표시
            timeTextList.Append(GetTimeText(targetTimeSpan.Seconds, TimeType.SECOND, true));

        return timeTextList.ToString();
    }

    public string GetRemainTimeText(TimeSpan targetTimeSpan)
    {
        var timeTextList = new StringBuilder();

        var hasDays = targetTimeSpan.Days > 0;
        var hasHours = targetTimeSpan.Hours > 0;
        var hasMinutes = targetTimeSpan.Minutes > 0;

        if (hasDays) timeTextList.Append(GetTimeText(targetTimeSpan.Days, TimeType.DAY, false));

        if (hasHours) timeTextList.Append(GetTimeText(targetTimeSpan.Hours, TimeType.HOUR, false));

        if (hasMinutes) timeTextList.Append(GetTimeText(targetTimeSpan.Minutes, TimeType.MINUTE, true));

        if (!hasDays && !hasHours) // 1시간 미만으로 남은 경우 초 단위 표시
            timeTextList.Append(GetTimeText(targetTimeSpan.Days, TimeType.SECOND, true));

        return timeTextList.ToString();
    }

    public LanguageType GetSystemLanguageType()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                return LanguageType.KR;
            default:
                return LanguageType.EN;
        }
    }
}
