using GameFramework.Procedure;
using GameInit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameInit.ProcedureComponent))]
public class ProcedureComponentInspector : BaseEditorInspector
{
    //private SerializedProperty m_ProcedureTypeNames = null;
    private SerializedProperty m_EntranceProcedureTypeName = null;

    //private string[] m_ProcedureTypeNames = null;
    private List<string> m_CurrentAvailableProcedureTypeNames = null;
    private int m_EntranceProcedureIndex = -1;

    private ProcedureComponent procedureComponent = null;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();


        if (string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
        {
            EditorGUILayout.HelpBox("Entrance procedure is invalid.", MessageType.Error);
        }
        else if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("Current Procedure",
                procedureComponent.CurrentProcedure == null ? "None"
                : procedureComponent.CurrentProcedure.GetType().ToString());
        }

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical("box");
       
        for (int i = 0; i < m_CurrentAvailableProcedureTypeNames.Count; i++)
        {
            EditorGUILayout.LabelField(m_CurrentAvailableProcedureTypeNames[i]);
        }
        EditorGUILayout.EndVertical();

        int selectedIndex = EditorGUILayout.Popup("Entrance Procedure", m_EntranceProcedureIndex, m_CurrentAvailableProcedureTypeNames.ToArray());
        if (selectedIndex != m_EntranceProcedureIndex)
        {
            m_EntranceProcedureIndex = selectedIndex;
            m_EntranceProcedureTypeName.stringValue = m_CurrentAvailableProcedureTypeNames[selectedIndex];
        }

        serializedObject.ApplyModifiedProperties();

        Repaint();
    }
    private void OnEnable()
    {
        //m_ProcedureTypeNames = serializedObject.FindProperty("procedures");
        m_EntranceProcedureTypeName = serializedObject.FindProperty("m_EntranceProcedureTypeName");

        m_CurrentAvailableProcedureTypeNames = new List<string>();

        procedureComponent = (ProcedureComponent)target;

        for (int i = 0; i < procedureComponent.procedureTypes.Length; i++)
        {
            var name = procedureComponent.procedureTypes[i].Name;
            m_CurrentAvailableProcedureTypeNames.Add(name);
        }

        if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
        {
            m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
            if (m_EntranceProcedureIndex < 0)
            {
                m_EntranceProcedureTypeName.stringValue = null;
            }
        }
    }

}
