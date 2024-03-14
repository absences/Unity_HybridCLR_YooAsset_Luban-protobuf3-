using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(UIInterface))]
public class UIInterfaceEditor : Editor
{
   
    SerializedProperty ChildList;

    void OnEnable()
    {
        if (target == null)
            return;
        ChildList = serializedObject.FindProperty("mChildObj");

        DrawOther();
    }
    private readonly Color TitleColor =  new Color(149/255f,208/255f,249/255f,0.5f);
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.BeginVertical("box");
        {
            var tempColor = GUI.backgroundColor;
            GUI.backgroundColor = TitleColor;
            if (GUILayout.Button("查找组件",new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleCenter},
                GUILayout.Height(20),GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth-30)))
            {
                //EditorApplication.ExecuteMenuItem("GameObject/UI/自动添加UIComponent组件");
                UIEditorTool.AddComponent();
     
            }
            if (GUILayout.Button("生成代码", new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter },
             GUILayout.Height(20), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 30)))
            {
                //EditorApplication.ExecuteMenuItem("GameObject/UI/UI代码生成");
                UIEditorTool.CSGen();
            }
            GUI.backgroundColor = tempColor;
            {
                list.elementHeight = 21;
                if(list.drawElementCallback == null)
                    DrawElement();
            }
            list.DoLayoutList();
        }
        GUILayout.EndVertical();
    }

    private ReorderableList list;
    private const int IndexWidth = 25;
    void DrawOther()
    {
        list = new ReorderableList(serializedObject, ChildList, false, false, false, false)
        {
            elementHeight = 16,
            footerHeight = 0,
            drawHeaderCallback = delegate(Rect rect)
            {
               // var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
                //if (GUI.Button(rect, "子组件列表", style))
                //{
                //    IsOpen = !IsOpen;
                //    EditorPrefs.SetBool(IsOpenKey, IsOpen);
                //}
            },
            drawFooterCallback  = rect =>
            {
                rect.width = 0;
            }
        };
        DrawElement();
    }

    void DrawElement()
    {
        list.drawElementCallback = (rect, index, active, focused) =>
        {
            var data = ChildList.GetArrayElementAtIndex(index);
            var indexRect = new Rect(rect) {width = IndexWidth,};
            EditorGUI.LabelField(indexRect, index + ":",
                new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperRight});
            rect.x += IndexWidth + 5;
            rect.width -= IndexWidth + 5;
            rect.height = 16;
            EditorGUI.PropertyField(rect, data, GUIContent.none);
        };
    }
}
