using GameFramework;
using System.Diagnostics;
using System.Text;

/// <summary>
/// 日志工具集。
/// </summary>
public static class Log
{
    /// <summary>
    /// 打印调试级别日志，用于记录调试类日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void Debug(object message)
    {
        GameFrameworkLog.Debug(message);
    }
    /// <summary>
    /// 打印调试级别日志，用于记录调试类日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void Debug(string message)
    {
        GameFrameworkLog.Debug(message);
    }
    /// <summary>
    /// 打印调试级别日志，用于记录调试类日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void Debug(params object[] param)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < param.Length; i++)
        {
            sb.Append(param[i]);
            sb.Append(' ');
        }
        GameFrameworkLog.Debug(sb);
    }
    /// <summary>
    /// 打印调试级别日志，用于记录调试类日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_DEBUG_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    public static void Debug(string format, params object[] param)
    {
        GameFrameworkLog.Debug(string.Format(format, param));
    }
    /// <summary>
    /// 打印信息级别日志，用于记录程序正常运行日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(object message)
    {
        GameFrameworkLog.Info(message);
    }
    /// <summary>
    /// 打印信息级别日志，用于记录程序正常运行日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string message)
    {
        GameFrameworkLog.Info(message);
    }

    /// <summary>
    /// 打印信息级别日志，用于记录程序正常运行日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(params object[] param)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < param.Length; i++)
        {
            sb.Append(param[i]);
            sb.Append(' ');
        }
        GameFrameworkLog.Info(sb);
    }
    /// <summary>
    /// 打印信息级别日志，用于记录程序正常运行日志信息。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_INFO_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    public static void Info(string format, params object[] param)
    {
        GameFrameworkLog.Info(string.Format(format, param));
    }

    /// <summary>
    /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_WARNING_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(object message)
    {
        GameFrameworkLog.Warning(message);
    }

    /// <summary>
    /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_WARNING_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string message)
    {
        GameFrameworkLog.Warning(message);
    }
    /// <summary>
    /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_WARNING_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(params object[] param)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < param.Length; i++)
        {
            sb.Append(param[i]);
            sb.Append(' ');
        }
        GameFrameworkLog.Warning(sb);
    }
    /// <summary>
    /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_WARNING_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_WARNING_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    public static void Warning(string format, params object[] message)
    {
        GameFrameworkLog.Warning(string.Format(format, message));
    }

    /// <summary>
    /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_ERROR_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(object message)
    {
        GameFrameworkLog.Error(message);
    }

    /// <summary>
    /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_ERROR_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string message)
    {
        GameFrameworkLog.Error(message);
    }
    /// <summary>
    /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_ERROR_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(params object[] param)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < param.Length; i++)
        {
            sb.Append(param[i]);
            sb.Append(' ');
        }
        GameFrameworkLog.Error(sb);
    }
    /// <summary>
    /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_ERROR_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_ERROR_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    public static void Error(string format, params object[] message)
    {
        GameFrameworkLog.Error(string.Format(format, message));
    }
    /// <summary>
    /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_FATAL_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_FATAL_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    [Conditional("ENABLE_FATAL_AND_ABOVE_LOG")]
    public static void Fatal(params object[] param)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < param.Length; i++)
        {
            sb.Append(param[i]);
            sb.Append(' ');
        }
        GameFrameworkLog.Fatal(sb);
    }

    /// <summary>
    /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_FATAL_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_FATAL_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    [Conditional("ENABLE_FATAL_AND_ABOVE_LOG")]
    public static void Fatal(object message)
    {
        GameFrameworkLog.Fatal(message);
    }
    /// <summary>
    /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_FATAL_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_FATAL_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    [Conditional("ENABLE_FATAL_AND_ABOVE_LOG")]
    public static void Fatal(string message)
    {
        GameFrameworkLog.Fatal(message);
    }
    /// <summary>
    /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架。
    /// </summary>
    /// <param name="message">日志内容。</param>
    /// <remarks>仅在带有 ENABLE_LOG、ENABLE_FATAL_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效。</remarks>
    [Conditional("ENABLE_LOG")]
    [Conditional("ENABLE_FATAL_LOG")]
    [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
    [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
    [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
    [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
    [Conditional("ENABLE_FATAL_AND_ABOVE_LOG")]
    public static void Fatal(string format, params object[] message)
    {
        GameFrameworkLog.Fatal(string.Format(format, message));
    }


    /// <summary>
    /// 断言严重错误级别日志，建议在发生严重错误。
    /// </summary>
    /// <param name="condition">条件。</param>
    [Conditional("ENABLE_LOG")]
    public static void Assert(bool condition)
    {
        if (!condition)
        {
            string message = string.Format("{0}\n{1}", "Assert Failed", System.Environment.StackTrace);
            Fatal(message);
        }
    }

    /// <summary>
    /// 断言严重错误级别日志，建议在发生严重错误。
    /// </summary>
    /// <param name="condition">条件。</param>
    /// <param name="retStr">断言输出字符串。</param>
    [Conditional("ENABLE_LOG")]
    public static void Assert(bool condition, string retStr)
    {
        if (!condition)
        {
            string message = string.Format("{0}\n{1}", "Assert Failed" + retStr, System.Environment.StackTrace);
            Fatal(message);
        }
    }
}

