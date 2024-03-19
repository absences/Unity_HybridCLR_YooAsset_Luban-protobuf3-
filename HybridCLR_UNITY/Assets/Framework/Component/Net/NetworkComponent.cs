using GameFramework.Network;
using System.ComponentModel;
using UnityEngine;


public class NetworkComponent : BaseGameComponent
{

    private INetworkManager m_NetworkManager = new NetworkManager();
    private EventComponent m_EventComponent = null;


    /// <summary>
    /// 获取网络频道数量。
    /// </summary>
    public int NetworkChannelCount
    {
        get
        {
            return m_NetworkManager.NetworkChannelCount;
        }
    }

    private bool Inited = false;
    public void Init()
    {
        
        Inited = true;
        m_EventComponent = GameEnter.Event;

        m_NetworkManager.NetworkConnected += OnNetworkConnected;
        m_NetworkManager.NetworkClosed += OnNetworkClosed;
        m_NetworkManager.NetworkMissHeartBeat += OnNetworkMissHeartBeat;
        m_NetworkManager.NetworkError += OnNetworkError;
        m_NetworkManager.NetworkCustomError += OnNetworkCustomError;
    }
    void Update()
    {
        if (Inited)
        {

            m_NetworkManager.Update(Time.deltaTime, Time.realtimeSinceStartup);
        }
       
    }
    private void OnDestroy()
    {
        if (Inited)
            m_NetworkManager.ShutDown();
    }
    /// <summary>
    /// 创建网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <param name="serviceType">网络服务类型。</param>
    /// <param name="networkChannelHelper">网络频道辅助器。</param>
    /// <returns>要创建的网络频道。</returns>
    public INetworkChannel CreateNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper)
    {
        return m_NetworkManager.CreateNetworkChannel(name, serviceType, networkChannelHelper);
    }

    public INetworkChannel GetNetworkChannel(string name)
    {
        return m_NetworkManager.GetNetworkChannel(name);
    }
    /// <summary>
    /// 获取所有网络频道。
    /// </summary>
    /// <returns>所有网络频道。</returns>
    public INetworkChannel[] GetAllNetworkChannels()
    {
        return m_NetworkManager.GetAllNetworkChannels();
    }

    /// <summary>
    /// 销毁网络频道。
    /// </summary>
    /// <param name="name">网络频道名称。</param>
    /// <returns>是否销毁网络频道成功。</returns>
    public bool DestroyNetworkChannel(string name)
    {
        return m_NetworkManager.DestroyNetworkChannel(name);
    }

    private void OnNetworkCustomError(object sender, NetworkCustomErrorEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.NetworkCustomErrorEventArgs.Create(e));
    }

    private void OnNetworkError(object sender, NetworkErrorEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.NetworkErrorEventArgs.Create(e));
    }

    private void OnNetworkMissHeartBeat(object sender, NetworkMissHeartBeatEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.NetworkMissHeartBeatEventArgs.Create(e));
    }

    private void OnNetworkClosed(object sender, NetworkClosedEventArgs e)
    {
        m_EventComponent.Fire(this, UnityGameFramework.Runtime.NetworkClosedEventArgs.Create(e));
    }

    private void OnNetworkConnected(object sender, NetworkConnectedEventArgs e)
    {

        m_EventComponent.Fire(this, UnityGameFramework.Runtime.NetworkConnectedEventArgs.Create(e));
    }
}
