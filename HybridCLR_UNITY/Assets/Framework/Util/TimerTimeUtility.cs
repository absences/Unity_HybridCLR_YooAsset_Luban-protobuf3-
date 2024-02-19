using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerTimeUtility
{
    private static readonly long Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    /// <summary>
    /// 当前时间戳
    /// </summary>
    /// <returns></returns>
    public static long Now()
    {
        return (DateTime.UtcNow.Ticks - Epoch) / 10000;
    }
}