using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameFramework;
using UnityEngine;

public static class GameUtil
{
    /// <summary>
    /// 时间转int
    /// </summary>
    /// <param name="now"></param>
    /// <returns></returns>
    public static int ConvertDatetimeToInt(DateTime now)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (int)(now - startTime).TotalSeconds;
    }
    public static long ConvertDatetimeToLong(DateTime now)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (int)(now - startTime).TotalMilliseconds;
    }
    public static bool CatchAnyParamNil(params object[] param)
    {
        if (param == null)
        {
            Log.Warning(string.Format("第[{0}]个参数为空", 0));
            return false;
        }
        for (int i = 0; i < param.Length; i++)
        {
            var p = param[i];
            if (p == null)
            {
                Log.Warning(string.Format("第[{0}]个参数为空", i));
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// 长度对应单位
    /// </summary>
    /// <param name="byteLength"></param>
    /// <returns></returns>
    public static string GetByteLengthString(long byteLength)
    {
        if (byteLength < 1024L) // 2 ^ 10
        {
            return Utility.Text.Format("{0} Bytes", byteLength.ToString());
        }

        if (byteLength < 1048576L) // 2 ^ 20
        {
            return Utility.Text.Format("{0} KB", (byteLength / 1024f).ToString("F2"));
        }

        if (byteLength < 1073741824L) // 2 ^ 30
        {
            return Utility.Text.Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
        }

        if (byteLength < 1099511627776L) // 2 ^ 40
        {
            return Utility.Text.Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
        }

        if (byteLength < 1125899906842624L) // 2 ^ 50
        {
            return Utility.Text.Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
        }

        if (byteLength < 1152921504606846976L) // 2 ^ 60
        {
            return Utility.Text.Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
        }

        return Utility.Text.Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
    }
    public static string GetLengthString(long length)
    {
        if (length < 1024)
        {
            return $"{length.ToString()} Bytes";
        }

        if (length < 1024 * 1024)
        {
            return $"{(length / 1024f):F2} KB";
        }

        return length < 1024 * 1024 * 1024 ? $"{(length / 1024f / 1024f):F2} MB" : $"{(length / 1024f / 1024f / 1024f):F2} GB";
    }
    /// <summary>
    /// 获取平台名称
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformName()
    {
#if UNITY_ANDROID
        return "Android";
#elif UNITY_IOS
        return "IOS";
#else
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                return "Windows64";
            case RuntimePlatform.WindowsPlayer:
                return "Windows64";
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "MacOS";

            case RuntimePlatform.IPhonePlayer:
                return "IOS";

            case RuntimePlatform.Android:
                return "Android";
            default:
                throw new System.NotSupportedException(string.Format("Platform '{0}' is not supported.",
                    Application.platform.ToString()));
        }
#endif
    }

    /// <summary>
    /// 获取或增加组件。
    /// </summary>
    /// <typeparam name="T">要获取或增加的组件。</typeparam>
    /// <param name="gameObject">目标对象。</param>
    /// <returns>获取或增加的组件。</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }
    public static bool GetComponent<T>(this GameObject gameObject, ref T component) where T : Component
    {
        if (component == null)
            component = gameObject.GetComponent<T>();
        return component != null;
    }

    public static string LogObj(object obj)
    {
        sb.Clear();
        
        sb.Append(obj.GetType());
        
        sb.AppendLine();
        DumpMsg(sb, obj, 1);
        RemoveComma();
        return sb.ToString();
    }

    const int SPACE_NUM = 2;
    private static void DumpMsg(StringBuilder sb, object obj, int spaceCount)
    {
        var space = "".PadLeft(spaceCount * SPACE_NUM, ' ');
        var parentSpace = "".PadLeft((spaceCount - 1) * SPACE_NUM, ' ');
        sb.Append(parentSpace).AppendLine("{");
        FieldInfo[] fieldInfos = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var f in fieldInfos)
        {
            //int nameLength = f.Name.Length;
            string fieldName = f.Name;//.Remove(nameLength - 1, 1);

            object fieldValue;
            try
            {
                fieldValue = f.GetValue(obj);
            }
            catch
            {
                fieldValue = "Parse Err";
            }

            sb.Append(space).Append('\"').Append(fieldName).Append("\":");
            if (ProcessValueType(fieldValue, concat: ",\n"))
                continue;

            if (fieldValue is ICollection collection)//����
            {
                sb.AppendLine("[");


                if (collection is IDictionary dictionary)//mapfield
                {
                    foreach (var item in dictionary.Keys)
                    {
                        if (ProcessValueType(item, spaceCount + 1, "="))
                        {
                        }
                        else
                            DumpMsg(sb, item, spaceCount + 2);

                        if (ProcessValueType(dictionary[item], spaceCount + 1))
                            continue;
                        else
                            DumpMsg(sb, dictionary[item], spaceCount + 1);
                    }
                }
                else
                {
                    foreach (var item in collection)
                    {
                        if (ProcessValueType(item, spaceCount + 1, ",\n"))
                            continue;
                        else
                        {
                            //if (item is ByteString bs)
                            //{
                            //    sb.Append("  ");
                            //    var array = bs.ToByteArray();
                            //    for (int i = 0; i < array.Length; i++)
                            //    {
                            //        sb.Append(array[i]);
                            //        if (i != array.Length - 1)
                            //            sb.Append(',');
                            //    }
                            //    sb.AppendLine();
                            //}
                            //else
                            {
                                DumpMsg(sb, item, spaceCount + 2);
                            }
                        }

                    }
                }

                if (collection.Count > 0)
                    RemoveComma();
                sb.Append(space).Append("],\n");
                continue;
            }


            //if (fieldValue is IMessage)
            //{
            //    DumpMsg(sb, fieldValue, spaceCount + 1);
            //}
        }
        if (fieldInfos.Length > 0)
            RemoveComma();
        sb.Append(parentSpace).Append("},\n");
    }
    static StringBuilder sb = new StringBuilder(512);
    /// <summary>
    /// 解析值类型
    /// </summary>
    /// <param name="fieldValue"></param>
    /// <param name="spaceCount"></param>
    /// <returns></returns>
    private static bool ProcessValueType(object fieldValue, int spaceCount = -1, string concat = "")
    {
        var stIdx = sb.Length;
        if (fieldValue == null)
        {
            sb.Append("null");
        }
        else
        {
            if (fieldValue is string)
                sb.Append('\"').Append(fieldValue).Append("\"");
            else if (fieldValue is bool)
                sb.Append(fieldValue.ToString().ToLower());
            else if (fieldValue.GetType().IsValueType)
                sb.Append(fieldValue.ToString());
            else
                return false;
        }
        if (spaceCount != -1)
            sb.Insert(stIdx, "".PadLeft(spaceCount * SPACE_NUM, ' '));
        sb.Append(concat);
        return true;
    }

    static void RemoveComma()
    {
        sb.Remove(sb.Length - 2, 1);
    }
}
