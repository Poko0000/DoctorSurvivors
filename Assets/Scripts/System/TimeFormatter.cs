using UnityEngine;

// 共用的時間格式工具,HUD 計時器和結算畫面都用同一套格式
public static class TimeFormatter
{
    // 把秒數轉成 "mm:ss",例如 125.7 → "02:05"
    public static string ToMinutesSeconds(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
        return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }
}
