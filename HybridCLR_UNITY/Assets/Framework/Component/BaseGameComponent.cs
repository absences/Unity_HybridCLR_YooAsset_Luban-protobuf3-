using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameComponent : MonoBehaviour
{
    /// <summary>
    /// 游戏框架组件初始化。
    /// </summary>
    protected virtual void Awake()
    {
        GameEnter.RegisterComponent(this);
    }
}
