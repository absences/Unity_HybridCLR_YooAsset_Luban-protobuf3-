
using UnityEngine;

public abstract class Singleton<T> where T : new()//构造函数
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
}


