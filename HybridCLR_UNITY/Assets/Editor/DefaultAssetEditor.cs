
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class DefaultAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var path = AssetDatabase.GetAssetPath(target);
        GUI.enabled = true;

        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Path:" + path);

            string[] tempPath = path.Split('/');

            string folderName = tempPath[tempPath.Length - 1];
            //if (folderName == "_Sprite_")
            //{
               
            //}
            //else
            {
                if (GUILayout.Button("打开文件夹目录"))
                {
                    var targetPath = Application.dataPath.Replace("Assets", path);
                    Application.OpenURL(targetPath);
                }
            }
            GUILayout.EndVertical();
        }
    }

    const string spritesName = "/Res/UI/sprites(need split rgb & alpha)";
    const string atlasName = "/Res/UI/atlas(need split rgb & alpha)/";

    
}


[InitializeOnLoad]
public static class AssetInspectorGUI
{
    static AssetInspectorGUI()
    {
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }
    static void OnPostHeaderGUI(Editor editor)
    {
        if (editor.targets.Length == 1)
        {
            DrawSelectEntriesButton(editor.targets[0]);  
        }
    }

    static void DrawSelectEntriesButton(Object obj)
    {
        GUILayout.BeginVertical();
        if (obj is AssetImporter ai )
        {
             EditorGUILayout.TextField("Value", ai.assetPath);
        }
        GUILayout.EndVertical();
    }

    
}