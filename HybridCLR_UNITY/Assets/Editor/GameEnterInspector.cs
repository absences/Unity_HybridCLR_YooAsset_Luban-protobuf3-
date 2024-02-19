using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameEnter))]
public class GameEnterInspector : BaseEditorInspector
{

    private SerializedProperty m_GameSpeed = null;


    private void OnEnable()
    {
        m_GameSpeed = serializedObject.FindProperty("m_GameSpeed");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        GameEnter t = (GameEnter)target;


        EditorGUILayout.BeginVertical("box");

        {
            float gameSpeed = EditorGUILayout.Slider("Game Speed", m_GameSpeed.floatValue, 0f, 8f);
            int selectedGameSpeed = GUILayout.SelectionGrid(GetSelectedGameSpeed(gameSpeed), GameSpeedForDisplay, 5);
            if (selectedGameSpeed >= 0)
            {
                gameSpeed = GetGameSpeed(selectedGameSpeed);
            }

            if (gameSpeed != m_GameSpeed.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    t.GameSpeed = gameSpeed;
                }
                else
                {
                    m_GameSpeed.floatValue = gameSpeed;
                }
            }
        }
        EditorGUILayout.EndVertical();
    }
    private static readonly string[] GameSpeedForDisplay = new string[] { "0x", "0.01x", "0.1x", "0.25x", "0.5x", "1x", "1.5x", "2x", "4x", "8x" };

    private static readonly float[] GameSpeed = new float[] { 0f, 0.01f, 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 4f, 8f };
    private int GetSelectedGameSpeed(float gameSpeed)
    {
        for (int i = 0; i < GameSpeed.Length; i++)
        {
            if (gameSpeed == GameSpeed[i])
            {
                return i;
            }
        }

        return -1;
    }
    private float GetGameSpeed(int selectedGameSpeed)
    {
        if (selectedGameSpeed < 0)
        {
            return GameSpeed[0];
        }

        if (selectedGameSpeed >= GameSpeed.Length)
        {
            return GameSpeed[GameSpeed.Length - 1];
        }

        return GameSpeed[selectedGameSpeed];
    }
}


