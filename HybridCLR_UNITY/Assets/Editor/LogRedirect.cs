using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LogRed
{
    private static Type m_ConsoleWindow;

    [UnityEditor.Callbacks.OnOpenAsset(0)] //1 : 使用OnOpenAsset属性 接管当有资源打开时的操作
    private static bool OnOpenAssetSoundLog(int instanceID)
    {
        var asset = AssetDatabase.GetAssetPath(instanceID);
        if (!asset.Contains("/DinLogHelper.cs"))//MonoScripts文件  这里进行筛选
        {
            return false;
        }

        if (m_ConsoleWindow == null) //反射出ConsoleWindow类
        {
            m_ConsoleWindow = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
        }

        if (EditorWindow.focusedWindow.GetType() != m_ConsoleWindow)
        {
            return false;
        }

        var activeText =
            m_ConsoleWindow?.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);//m_ActiveText包含了当前Log的全部信息
        var consoleWindowFiledInfo =
            m_ConsoleWindow?.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);//ms_ConsoleWindow 是ConsoleWindow的对象字段
        var consoleWindowInstance = consoleWindowFiledInfo?.GetValue(null);//从对象字段中得到这个对象
        var str = activeText?.GetValue(consoleWindowInstance).ToString();//得到Log信息,用于后面解析


        var (path, lineIndex) = GetSubStringInStackStr(str, 5);//解析出对应的.cs文件全路径  和 行号  因为笔者的Log第二个cs文件即正确位置  所以参数传1
        if (lineIndex == -1)
            return false;

        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, lineIndex);//跳转到正确行号
        return true;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stackStr"></param>
    /// <param name="needIndex">第几个at才是输出位置</param>
    /// <returns></returns>
    private static (string, int) GetSubStringInStackStr(string stackStr, int needIndex)
    {
        var lines = stackStr.Split(new string[] { "\n" }, StringSplitOptions.None);

        var tempIndex = 0;
        var count = lines.Length;
        for (int i = 0; i < count; i++)
        {
            string textBeforeFilePath = ") (at ";
            int filePathIndex = lines[i].IndexOf(textBeforeFilePath, StringComparison.Ordinal);

            if (filePathIndex > 0)
            {
                filePathIndex += textBeforeFilePath.Length;
                if (lines[i][filePathIndex] != '<')
                {
                    string filePathPart = lines[i].Substring(filePathIndex);
                    int lineIndex = filePathPart.LastIndexOf(":", StringComparison.Ordinal);
                    if (lineIndex > 0)
                    {
                        int endLineIndex = filePathPart.LastIndexOf(")", StringComparison.Ordinal);
                        if (endLineIndex > 0)
                        {
                            string lineString =
                                filePathPart.Substring(lineIndex + 1, (endLineIndex) - (lineIndex + 1));
                            string filePath = filePathPart.Substring(0, lineIndex);
                            if (tempIndex++ >= needIndex)
                            {
                                return (filePath, int.Parse(lineString));
                            }
                        }
                    }
                }
            }
        }

        return ("", -1);
    }
}
