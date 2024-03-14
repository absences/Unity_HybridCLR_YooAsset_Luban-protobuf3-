using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtension 
{
    private static readonly List<Transform> s_CachedTransforms = new List<Transform>();

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }
    /// <summary>
    /// �ݹ�������Ϸ����Ĳ�Ρ�
    /// </summary>
    /// <param name="gameObject"><see cref="GameObject" /> ����</param>
    /// <param name="layer">Ŀ���εı�š�</param>
    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.GetComponentsInChildren(true, s_CachedTransforms);
        for (int i = 0; i < s_CachedTransforms.Count; i++)
        {
            s_CachedTransforms[i].gameObject.layer = layer;
        }

        s_CachedTransforms.Clear();
    }
}
