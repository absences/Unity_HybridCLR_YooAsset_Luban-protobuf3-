using GameFramework;
using GameMain;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class GameEnter : MonoBehaviour
{
    private void Awake()
    {
        Utility.Text.SetTextHelper(new DefaultTextHelper());

        GameFrameworkLog.SetLogHelper(new DinLogHelper());

        DontDestroyOnLoad(gameObject);
    }

    #region GameSpeed
    [SerializeField]
    private float m_GameSpeed = 1f;

    /// <summary>
    /// 获取或设置游戏速度。
    /// </summary>
    public float GameSpeed
    {
        get
        {
            return m_GameSpeed;
        }
        set
        {
            Time.timeScale = m_GameSpeed = value >= 0f ? value : 0f;
        }
    }
    #endregion

    #region Component
    private static readonly GameFrameworkLinkedList<BaseGameComponent> s_GameFrameworkComponents = new GameFrameworkLinkedList<BaseGameComponent>();

    public new static T GetComponent<T>() where T : BaseGameComponent
    {
        var type = typeof(T);
        LinkedListNode<BaseGameComponent> current = s_GameFrameworkComponents.First;
        while (current != null)
        {
            if (current.Value.GetType() == type)
            {
                return current.Value as T;
            }

            current = current.Next;
        }
        return null;
    }

    /// <summary>
    /// 注册游戏框架组件。
    /// </summary>
    /// <param name="gameFrameworkComponent">要注册的游戏框架组件。</param>
    internal static void RegisterComponent(BaseGameComponent gameFrameworkComponent)
    {
        s_GameFrameworkComponents.AddLast(gameFrameworkComponent);

        switch (gameFrameworkComponent)
        {
            case ResourceComponent resourceComponent:
                Resource = resourceComponent;
                break;
            case FsmComponent fsmComponent:
                FSM = fsmComponent;
                break;
            case TimerComponent timerComponent:
                Timer = timerComponent;
                break;
            case ConfigComponent configComponent:
                Config = configComponent;
                break;
            case HotFixComponent hotFixComponent:
                HotFix = hotFixComponent;
                break;
            case ObjectPoolComponent objectPoolComponent:
                ObjectPool = objectPoolComponent;
                break;
            case UIComponent uiComponent:
                UI = uiComponent;
                break;
        }
    }

    public static ResourceComponent Resource { private set; get; }

    public static FsmComponent FSM { private set; get; }

    public static TimerComponent Timer { private set; get; }

    public static ConfigComponent Config { private set; get; }

    public static HotFixComponent HotFix { private set; get; }

    public static ObjectPoolComponent ObjectPool { private set; get; }

    public static UIComponent UI { private set; get; }
    #endregion
}
