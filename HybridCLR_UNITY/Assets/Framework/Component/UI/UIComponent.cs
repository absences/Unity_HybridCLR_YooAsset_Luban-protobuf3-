using Cysharp.Threading.Tasks;
using GameFramework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public partial class UIComponent : BaseGameComponent
{
    [SerializeField]
    private bool m_EnableOpenUIFormSuccessEvent = true;

    [SerializeField]
    private bool m_EnableOpenUIFormFailureEvent = true;

    [SerializeField]
    private bool m_EnableOpenUIFormUpdateEvent = false;

    [SerializeField]
    private bool m_EnableCloseUIFormCompleteEvent = true;

    [SerializeField]
    private float m_InstanceAutoReleaseInterval = 60f;

    [SerializeField]
    private int m_InstanceCapacity = 16;

    [SerializeField]
    private float m_InstanceExpireTime = 60f;

    [SerializeField]
    private int m_InstancePriority = 0;

    [SerializeField]
    private RectTransform m_InstanceRoot = null;

    [SerializeField]
    private UIGroup[] m_UIGroups = null;

    private IUIManager m_UIManager;

    private EventComponent m_EventComponent = null;

    private readonly List<IUIForm> m_InternalUIFormResults = new List<IUIForm>();

    public Camera UICamera;

    /// <summary>
    /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数。
    /// </summary>
    public float InstanceAutoReleaseInterval
    {
        get
        {
            return m_UIManager.InstanceAutoReleaseInterval;
        }
        set
        {
            m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value;
        }
    }

    /// <summary>
    /// 获取或设置界面实例对象池的容量。
    /// </summary>
    public int InstanceCapacity
    {
        get
        {
            return m_UIManager.InstanceCapacity;
        }
        set
        {
            m_UIManager.InstanceCapacity = m_InstanceCapacity = value;
        }
    }
    /// <summary>
    /// 获取或设置界面实例对象池对象过期秒数。
    /// </summary>
    public float InstanceExpireTime
    {
        get
        {
            return m_UIManager.InstanceExpireTime;
        }
        set
        {
            m_UIManager.InstanceExpireTime = m_InstanceExpireTime = value;
        }
    }
    /// <summary>
    /// 获取界面组数量。
    /// </summary>
    public int UIGroupCount
    {
        get
        {
            return m_UIManager.UIGroupCount;
        }
    }

    /// <summary>
    /// 获取或设置界面实例对象池的优先级。
    /// </summary>
    public int InstancePriority
    {
        get
        {
            return m_UIManager.InstancePriority;
        }
        set
        {
            m_UIManager.InstancePriority = m_InstancePriority = value;
        }
    }
    private void Start()
    {
        m_UIManager = new UIManager();

        if (m_EnableOpenUIFormSuccessEvent)
        {
            m_UIManager.OpenUIFormSuccess += OnOpenUIFormSuccess;
        }

        m_UIManager.OpenUIFormFailure += OnOpenUIFormFailure;

        if (m_EnableOpenUIFormUpdateEvent)
        {
            m_UIManager.OpenUIFormUpdate += OnOpenUIFormUpdate;
        }

        if (m_EnableCloseUIFormCompleteEvent)
        {
            m_UIManager.CloseUIFormComplete += OnCloseUIFormComplete;
        }

        //对象池管理器
        m_UIManager.SetObjectPoolManager(GameEnter.ObjectPool.ObjectPoolManager);
        m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
        m_UIManager.InstanceCapacity = m_InstanceCapacity;
        m_UIManager.InstanceExpireTime = m_InstanceExpireTime;
        m_UIManager.InstancePriority = m_InstancePriority;

        UIFormHelperBase uiFormHelper = new GameObject("UI Form Helper").AddComponent<DefaultUIFormHelper>();

        Transform helpertransform = uiFormHelper.transform;
        helpertransform.SetParent(transform);
        helpertransform.localScale = Vector3.one;

        m_UIManager.SetUIFormHelper(uiFormHelper);


        m_InstanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");

        for (int i = 0; i < m_UIGroups.Length; i++)
        {
            if (!AddUIGroup(m_UIGroups[i].Name, m_UIGroups[i].Depth))
            {
                Log.Warning("Add UI group '{0}' failure.", m_UIGroups[i].Name);
                continue;
            }
        }
    }
    /// <summary>
    /// 是否存在界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>是否存在界面组。</returns>
    public bool HasUIGroup(string uiGroupName)
    {
        return m_UIManager.HasUIGroup(uiGroupName);
    }

    /// <summary>
    /// 获取界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>要获取的界面组。</returns>
    public IUIGroup GetUIGroup(string uiGroupName)
    {
        return m_UIManager.GetUIGroup(uiGroupName);
    }

    /// <summary>
    /// 获取所有界面组。
    /// </summary>
    /// <returns>所有界面组。</returns>
    public IUIGroup[] GetAllUIGroups()
    {
        return m_UIManager.GetAllUIGroups();
    }

    /// <summary>
    /// 获取所有界面组。
    /// </summary>
    /// <param name="results">所有界面组。</param>
    public void GetAllUIGroups(List<IUIGroup> results)
    {
        m_UIManager.GetAllUIGroups(results);
    }

    /// <summary>
    /// 增加界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>是否增加界面组成功。</returns>
    public bool AddUIGroup(string uiGroupName)
    {
        return AddUIGroup(uiGroupName, 0);
    }

    /// <summary>
    /// 增加界面组。
    /// </summary>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="depth">界面组深度。</param>
    /// <returns>是否增加界面组成功。</returns>
    public bool AddUIGroup(string uiGroupName, int depth)
    {
        if (m_UIManager.HasUIGroup(uiGroupName))
        {
            return false;
        }

        UIGroupHelperBase uiGroupHelper = new GameObject(string.Format("UI Group - {0}", uiGroupName)).AddComponent<DefaultUIGroupHelper>();// Helper.CreateHelper(m_UIGroupHelperTypeName, m_CustomUIGroupHelper, UIGroupCount);


        uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
        RectTransform transform = uiGroupHelper.gameObject.AddComponent<RectTransform>();
        transform.SetParent(m_InstanceRoot);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.anchorMin = Vector3.zero;
        transform.anchorMax = Vector3.one;
        transform.sizeDelta = m_InstanceRoot.sizeDelta;

        return m_UIManager.AddUIGroup(uiGroupName, depth, uiGroupHelper);
    }

    /// <summary>
    /// 是否存在界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>是否存在界面。</returns>
    public bool HasUIForm(int serialId)
    {
        return m_UIManager.HasUIForm(serialId);
    }

    /// <summary>
    /// 是否存在界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>是否存在界面。</returns>
    public bool HasUIForm(string uiFormAssetName)
    {
        return m_UIManager.HasUIForm(uiFormAssetName);
    }

    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>要获取的界面。</returns>
    public UIForm GetUIForm(int serialId)
    {
        return (UIForm)m_UIManager.GetUIForm(serialId);
    }

    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>要获取的界面。</returns>
    public UIForm GetUIForm(string uiFormAssetName)
    {
        return (UIForm)m_UIManager.GetUIForm(uiFormAssetName);
    }
    /// <summary>
    /// 获取界面。
    /// </summary>
    public T GetUIForm<T>() where T : UIFormLogic
    {
        var form = m_UIManager.GetUIForm(typeof(T).Name);
      
        if (form != null)
        {
            return (form as UIForm).Logic as T;
        }
        return null;
    }
    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>要获取的界面。</returns>
    public UIForm[] GetUIForms(string uiFormAssetName)
    {
        IUIForm[] uiForms = m_UIManager.GetUIForms(uiFormAssetName);
        UIForm[] uiFormImpls = new UIForm[uiForms.Length];
        for (int i = 0; i < uiForms.Length; i++)
        {
            uiFormImpls[i] = (UIForm)uiForms[i];
        }

        return uiFormImpls;
    }

    /// <summary>
    /// 获取界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="results">要获取的界面。</param>
    public void GetUIForms(string uiFormAssetName, List<UIForm> results)
    {
        if (results == null)
        {
            Log.Error("Results is invalid.");
            return;
        }

        results.Clear();
        m_UIManager.GetUIForms(uiFormAssetName, m_InternalUIFormResults);
        foreach (IUIForm uiForm in m_InternalUIFormResults)
        {
            results.Add((UIForm)uiForm);
        }
    }

    /// <summary>
    /// 获取所有已加载的界面。
    /// </summary>
    /// <returns>所有已加载的界面。</returns>
    public UIForm[] GetAllLoadedUIForms()
    {
        IUIForm[] uiForms = m_UIManager.GetAllLoadedUIForms();
        UIForm[] uiFormImpls = new UIForm[uiForms.Length];
        for (int i = 0; i < uiForms.Length; i++)
        {
            uiFormImpls[i] = (UIForm)uiForms[i];
        }

        return uiFormImpls;
    }

    /// <summary>
    /// 获取所有已加载的界面。
    /// </summary>
    /// <param name="results">所有已加载的界面。</param>
    public void GetAllLoadedUIForms(List<UIForm> results)
    {
        if (results == null)
        {
            Log.Error("Results is invalid.");
            return;
        }

        results.Clear();
        m_UIManager.GetAllLoadedUIForms(m_InternalUIFormResults);
        foreach (IUIForm uiForm in m_InternalUIFormResults)
        {
            results.Add((UIForm)uiForm);
        }
    }

    /// <summary>
    /// 获取所有正在加载界面的序列编号。
    /// </summary>
    /// <returns>所有正在加载界面的序列编号。</returns>
    public int[] GetAllLoadingUIFormSerialIds()
    {
        return m_UIManager.GetAllLoadingUIFormSerialIds();
    }

    /// <summary>
    /// 获取所有正在加载界面的序列编号。
    /// </summary>
    /// <param name="results">所有正在加载界面的序列编号。</param>
    public void GetAllLoadingUIFormSerialIds(List<int> results)
    {
        m_UIManager.GetAllLoadingUIFormSerialIds(results);
    }

    /// <summary>
    /// 是否正在加载界面。
    /// </summary>
    /// <param name="serialId">界面序列编号。</param>
    /// <returns>是否正在加载界面。</returns>
    public bool IsLoadingUIForm(int serialId)
    {
        return m_UIManager.IsLoadingUIForm(serialId);
    }

    /// <summary>
    /// 是否正在加载界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <returns>是否正在加载界面。</returns>
    public bool IsLoadingUIForm(string uiFormAssetName)
    {
        return m_UIManager.IsLoadingUIForm(uiFormAssetName);
    }

    /// <summary>
    /// 是否是合法的界面。
    /// </summary>
    /// <param name="uiForm">界面。</param>
    /// <returns>界面是否合法。</returns>
    public bool IsValidUIForm(UIForm uiForm)
    {
        return m_UIManager.IsValidUIForm(uiForm);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, false, null);
    }
    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, uint priority)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, false, null);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, pauseCoveredUIForm, null);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, false, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, uint priority, bool pauseCoveredUIForm)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, null);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, uint priority, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, priority, false, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData)
    {
        return OpenUIForm(uiFormAssetName, uiGroupName, 0, pauseCoveredUIForm, userData);
    }

    /// <summary>
    /// 打开界面。
    /// </summary>
    /// <param name="uiFormAssetName">界面资源名称。</param>
    /// <param name="uiGroupName">界面组名称。</param>
    /// <param name="priority">加载界面资源的优先级。</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>界面的序列编号。</returns>
    public int OpenUIForm(string uiFormAssetName, string uiGroupName, uint priority, bool pauseCoveredUIForm, object userData)
    {
        return m_UIManager.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
    }

    /// <summary>
    /// 异步返回打开界面
    /// </summary>
    /// <param name="uiFormAssetName"></param>
    /// <param name="uiGroupName"></param>
    /// <param name="priority"></param>
    /// <param name="pauseCoveredUIForm"></param>
    /// <param name="userData"></param>
    /// <returns></returns>
    public UniTask<UIFormLogic> OpenUIFormAsync(string uiFormAssetName, string uiGroupName, uint priority = 0, bool pauseCoveredUIForm = false, object userData = null)
    {
        return m_UIManager.OpenUIFormAsync(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
    }
    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="serialId">要关闭界面的序列编号。</param>
    public void CloseUIForm(int serialId)
    {
        m_UIManager.CloseUIForm(serialId);
    }
    /// <summary>
    /// 关闭界面。
    /// </summary>
    public void CloseUIForm<T>() where T : UIFormLogic
    {
        var uiForm = m_UIManager.GetUIForm(typeof(T).Name);
        if (uiForm != null)
            m_UIManager.CloseUIForm(uiForm);
    }
    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="serialId">要关闭界面的序列编号。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseUIForm(int serialId, object userData)
    {
        m_UIManager.CloseUIForm(serialId, userData);
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="uiForm">要关闭的界面。</param>
    public void CloseUIForm(UIForm uiForm)
    {
        m_UIManager.CloseUIForm(uiForm);
    }

    /// <summary>
    /// 关闭界面。
    /// </summary>
    /// <param name="uiForm">要关闭的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseUIForm(UIForm uiForm, object userData)
    {
        m_UIManager.CloseUIForm(uiForm, userData);
    }

    /// <summary>
    /// 关闭所有已加载的界面。
    /// </summary>
    public void CloseAllLoadedUIForms()
    {
        m_UIManager.CloseAllLoadedUIForms();
    }

    /// <summary>
    /// 关闭所有已加载的界面。
    /// </summary>
    /// <param name="userData">用户自定义数据。</param>
    public void CloseAllLoadedUIForms(object userData)
    {
        m_UIManager.CloseAllLoadedUIForms(userData);
    }

    /// <summary>
    /// 关闭所有正在加载的界面。
    /// </summary>
    public void CloseAllLoadingUIForms()
    {
        m_UIManager.CloseAllLoadingUIForms();
    }

    /// <summary>
    /// 激活界面。
    /// </summary>
    /// <param name="uiForm">要激活的界面。</param>
    public void RefocusUIForm(UIForm uiForm)
    {
        m_UIManager.RefocusUIForm(uiForm);
    }

    /// <summary>
    /// 激活界面。
    /// </summary>
    /// <param name="uiForm">要激活的界面。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void RefocusUIForm(UIForm uiForm, object userData)
    {
        m_UIManager.RefocusUIForm(uiForm, userData);
    }

    /// <summary>
    /// 设置界面是否被加锁。
    /// </summary>
    /// <param name="uiForm">要设置是否被加锁的界面。</param>
    /// <param name="locked">界面是否被加锁。</param>
    public void SetUIFormInstanceLocked(UIForm uiForm, bool locked)
    {
        if (uiForm == null)
        {
            Log.Warning("UI form is invalid.");
            return;
        }

        m_UIManager.SetUIFormInstanceLocked(uiForm.gameObject, locked);
    }

    /// <summary>
    /// 设置界面的优先级。
    /// </summary>
    /// <param name="uiForm">要设置优先级的界面。</param>
    /// <param name="priority">界面优先级。</param>
    public void SetUIFormInstancePriority(UIForm uiForm, int priority)
    {
        if (uiForm == null)
        {
            Log.Warning("UI form is invalid.");
            return;
        }

        m_UIManager.SetUIFormInstancePriority(uiForm.gameObject, priority);
    }

    private void OnOpenUIFormSuccess(object sender, GameFramework.UI.OpenUIFormSuccessEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.OpenUIFormSuccessEventArgs.Create(e));
    }

    private void OnOpenUIFormFailure(object sender, GameFramework.UI.OpenUIFormFailureEventArgs e)
    {
        Log.Warning("Open UI form failure, asset name '{0}', UI group name '{1}', pause covered UI form '{2}', error message '{3}'.",
            e.UIFormAssetName, e.UIGroupName, e.PauseCoveredUIForm.ToString(), e.ErrorMessage);
        if (m_EnableOpenUIFormFailureEvent)
        {
            m_EventComponent.Fire(this, UnityGameFramework.Runtime.OpenUIFormFailureEventArgs.Create(e));
        }
    }

    private void OnOpenUIFormUpdate(object sender, GameFramework.UI.OpenUIFormUpdateEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.OpenUIFormUpdateEventArgs.Create(e));
    }


    private void OnCloseUIFormComplete(object sender, GameFramework.UI.CloseUIFormCompleteEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.CloseUIFormCompleteEventArgs.Create(e));
    }

}
