using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
public static class UIEditorTool
{
    private static List<Object> AllList;

    public static void AddComponent()
    {
        GameObject window = Selection.activeGameObject;
        if (window == null)
            return;

        UIInterface face = window.GetComponent<UIInterface>();
        if (face == null)
        {
            face = window.AddComponent<UIInterface>();
        }
        AllList = new List<Object>();

        AddToList(face.transform);

        face.mChildObj = new Object[AllList.Count];
        for (int i = 0; i < AllList.Count; i++)
        {
            face.mChildObj[i] = AllList[i];
        }
        EditorUtility.SetDirty(window);

    }

    static void AddToList(Transform transform)
    {
        foreach (Transform item in transform)
        {
            var obj = GetObj(item.gameObject);
            if (obj != null)
            {
                AllList.Add(obj);
            }

            if (item.childCount > 0 && item.GetComponent<UIInterface>() == null)
                AddToList(item);
        }
    }

    [MenuItem("GameObject/UI/UI代码生成", false, 2)]

    public static void CSGen()
    {
        string path = Application.dataPath.Replace("Assets", "Logs/");


        GameObject window = Selection.activeGameObject;
        if (window == null)
            return;

        UIInterface face = window.GetComponent<UIInterface>();
        if (face == null)
        {
            Debug.Log("没有UIInterface");
            return;
        }

        var list = face.mChildObj;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using UnityGameFramework.Runtime;");
        sb.AppendLine("using TMPro;");
        sb.AppendLine("using UnityEngine.UI;\n");
        sb.AppendLine("namespace HotfixMain");
        sb.AppendLine("{");

        sb.AppendLine($"    internal class {face.gameObject.name} : UIFormLogic");
        sb.AppendLine("    {");

        sb.AppendLine("        #region UI Members");


        foreach (Object item in list)
        {
            string Goanme = string.Empty;
            if (item is GameObject go)
            {
                Goanme = go.name;
            }
            else if (item is Component com)
            {
                Goanme = com.transform.gameObject.name;
            }
            // string start = Goanme.Remove(3);

            string name = "m" + upperFirst(Goanme);

            string type = ReMoveNameSp(item.GetType().ToString());

            sb.AppendLine("        private " + type + " " + name + ";");
        }
        sb.AppendLine("        #endregion\n");

        sb.AppendLine("        protected override void OnInit(object userData)");

        sb.AppendLine("        {");

        int index = 0;
        foreach (var obj in list)
        {
            string Goanme = string.Empty;
            GameObject go = null;
            if (obj is GameObject)
            {
                go = obj as GameObject;
                Goanme = go.name;
            }
            else if (obj is Component com)
            {
                Goanme = com.transform.gameObject.name;
            }

            // string start = Goanme.Remove(3);
            string name = "m" + upperFirst(Goanme);

            if (go != null)
            {
                sb.Append("            ");
                sb.AppendLine(name + "=getGameobject" + "(" + index + ");");
            }

            else if (obj is Component com)
            {
                sb.Append("              ");
                sb.AppendLine(name + "=getComponent<" + ReMoveNameSp(com.GetType().ToString()) + ">(" + index + ");");
            }

            index++;
        }
        sb.AppendLine("        }");
        sb.AppendLine("        protected override void OnOpen(object userData)");
        sb.AppendLine("        {\n");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        string strFileName = path + window.name + "_face.cs";
        File.WriteAllText(strFileName, sb.ToString());
        OpenFile(strFileName);
    }
    static void OpenFile(string path)
    {

        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Normal;


        startInfo.FileName = Application.temporaryCachePath + "/../../../Programs/Microsoft VS Code/Code.exe";

        startInfo.Arguments = path;
        process.StartInfo = startInfo;

        process.Start();

    }
    static string ReMoveNameSp(string fullname)
    {
        if (fullname.Contains("."))
        {
            string[] sp = fullname.Split('.');
            return sp[sp.Length - 1];
        }
        return fullname;
    }

    /// <summary>
    /// 设置变量名称首字母大写
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private static string upperFirst(string s)
    {
        s = s.Replace(" ", "").Replace("(", "").Replace(")", "");
        return Regex.Replace(s, @"\b[a-z]\w+", delegate (Match match)
        {
            string v = match.ToString();
            return char.ToUpper(v[0]) + v.Substring(1);
        });
    }



    public static UnityEngine.Object GetObj(GameObject tgo)
    {
        var gname = tgo.name.Trim();
        int length = gname.Length;
        if (length <= 3)
            return null;

        string n = gname.Remove(3);
        UnityEngine.Object obj = null;
        switch (n)
        {
            case "txt":
                obj = tgo.GetComponent<TextMeshProUGUI>();
                break;
            case "btn":
                obj = tgo.GetComponent<Button>();
                break;
            case "obj":
                obj = tgo;
                break;
            case "tra":
                obj = tgo.transform;
                break;
            case "img":
                obj = tgo.GetComponent<Image>();
                break;
            case "ins":
                obj = tgo.GetComponent<UIInterface>();
                break;
            case "sdr":
                obj = tgo.GetComponent<Slider>();
                break;
            case "tog":
                obj = tgo.GetComponent<Toggle>();
                break;
            case "inp":
                obj = tgo.GetComponent<InputField>();
                break;
            case "cvg":
                obj = tgo.GetComponent<CanvasGroup>();
                break;
            case "tmp":
                obj = tgo.GetComponent<TMPro.TextMeshProUGUI>();
                break;
            default:
                // Log.Error("未能处理物体:", tgo.name);
                break;
        }
        return obj;
    }
}


