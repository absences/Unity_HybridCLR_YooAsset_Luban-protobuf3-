using UnityEngine;

public class UIInterface : MonoBehaviour
{
    public Object[] mChildObj;
    Object getComponent(int index)
    {
        if (index >= mChildObj.Length || index < 0)
        {
            return null;
        }
        else
        {
            return mChildObj[index];
        }
    }
    public T getComponent<T>(int id) where T : Component
    {
        var o = getComponent(id);
        var t = o as T;
        return t;
    }

    public GameObject getGameobject(int id)
    {
        var obj = getComponent(id);

        return obj as GameObject;
    }
}
