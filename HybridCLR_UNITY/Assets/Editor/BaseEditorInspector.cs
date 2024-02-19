using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseEditorInspector : UnityEditor.Editor
{
    private bool m_IsCompiling = false;
    public override void OnInspectorGUI()
    {
        if (m_IsCompiling && !EditorApplication.isCompiling)
        {
            m_IsCompiling = false;
            OnCompileComplete();
        }
        else if (!m_IsCompiling && EditorApplication.isCompiling)
        {
            m_IsCompiling = true;
            OnCompileStart();
        }
    }
    /// <summary>
    /// 编译开始事件。
    /// </summary>
    protected virtual void OnCompileStart()
    {
    }

    /// <summary>
    /// 编译完成事件。
    /// </summary>
    protected virtual void OnCompileComplete()
    {
    }
}
