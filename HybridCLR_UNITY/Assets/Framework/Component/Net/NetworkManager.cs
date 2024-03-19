using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

internal class NetworkManager : INetworkManager
{
    /// <summary>
    /// 获取网络频道数量。
    /// </summary>
    public int NetworkChannelCount
    {
        get
        {
            return m_NetworkChannels.Count;
        }
    }

    private readonly Dictionary<string, NetworkChannelBase> m_NetworkChannels;
    private EventHandler<NetworkConnectedEventArgs> m_NetworkConnectedEventHandler;
    private EventHandler<NetworkClosedEventArgs> m_NetworkClosedEventHandler;
    private EventHandler<NetworkMissHeartBeatEventArgs> m_NetworkMissHeartBeatEventHandler;
    private EventHandler<NetworkErrorEventArgs> m_NetworkErrorEventHandler;
    private EventHandler<NetworkCustomErrorEventArgs> m_NetworkCustomErrorEventHandler;


    public NetworkManager()
    {
        m_NetworkChannels = new Dictionary<string, NetworkChannelBase>();
    }

    public INetworkChannel CreateNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper)
    {
        if (networkChannelHelper == null)
        {
            throw new GameFrameworkException("Network channel helper is invalid.");
        }

        if (HasNetworkChannel(name))
        {
            throw new GameFrameworkException(Utility.Text.Format("Already exist network channel '{0}'.", name ?? string.Empty));
        }

        NetworkChannelBase networkChannel = null;

        switch (serviceType)
        {
            case ServiceType.Tcp:
                networkChannel = new TcpNetworkChannel(name, networkChannelHelper);
                break;
            case ServiceType.Kcp:
                networkChannel = new KcpNetworkChannel(name, 50001, networkChannelHelper);
                break;
        }
        networkChannel.NetworkChannelConnected += OnNetworkChannelConnected;
        networkChannel.NetworkChannelClosed += OnNetworkChannelClosed;
        networkChannel.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
        networkChannel.NetworkChannelError += OnNetworkChannelError;
        networkChannel.NetworkChannelCustomError += OnNetworkChannelCustomError;

        m_NetworkChannels.Add(name, networkChannel);

        return networkChannel;
    }

    public bool DestroyNetworkChannel(string name)
    {
        NetworkChannelBase networkChannel = null;
        if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out networkChannel))
        {
            networkChannel.NetworkChannelConnected -= OnNetworkChannelConnected;
            networkChannel.NetworkChannelClosed -= OnNetworkChannelClosed;
            networkChannel.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
            networkChannel.NetworkChannelError -= OnNetworkChannelError;
            networkChannel.NetworkChannelCustomError -= OnNetworkChannelCustomError;
            networkChannel.Shutdown();
            return m_NetworkChannels.Remove(name);
        }

        return false;
    }

    public INetworkChannel[] GetAllNetworkChannels()
    {
        int index = 0;
        INetworkChannel[] results = new INetworkChannel[m_NetworkChannels.Count];
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            results[index++] = networkChannel.Value;
        }

        return results;
    }

    public void GetAllNetworkChannels(List<INetworkChannel> results)
    {
        if (results == null)
        {
            throw new GameFrameworkException("Results is invalid.");
        }

        results.Clear();
        foreach (KeyValuePair<string, NetworkChannelBase> networkChannel in m_NetworkChannels)
        {
            results.Add(networkChannel.Value);
        }
    }

    public INetworkChannel GetNetworkChannel(string name)
    {
        if (m_NetworkChannels.TryGetValue(name ?? string.Empty, out NetworkChannelBase networkChannel))
        {
            return networkChannel;
        }

        return null;
    }

    public bool HasNetworkChannel(string name)
    {
        return m_NetworkChannels.ContainsKey(name ?? string.Empty);
    }
    /// <summary>
    /// 网络管理器轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        foreach (var networkChannel in m_NetworkChannels.Values)
        {
            networkChannel.Update(elapseSeconds, realElapseSeconds);
        }
    }

    public void ShutDown()
    {
        foreach (var networkChannelBase in m_NetworkChannels.Values)
        {
            networkChannelBase.NetworkChannelConnected -= OnNetworkChannelConnected;
            networkChannelBase.NetworkChannelClosed -= OnNetworkChannelClosed;
            networkChannelBase.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
            networkChannelBase.NetworkChannelError -= OnNetworkChannelError;
            networkChannelBase.NetworkChannelCustomError -= OnNetworkChannelCustomError;
            //networkChannelBase.Shutdown();
        }

        m_NetworkChannels.Clear();
    }
    /// <summary>
    /// 网络连接成功事件。
    /// </summary>
    public event EventHandler<NetworkConnectedEventArgs> NetworkConnected
    {
        add
        {
            m_NetworkConnectedEventHandler += value;
        }
        remove
        {
            m_NetworkConnectedEventHandler -= value;
        }
    }

    /// <summary>
    /// 网络连接关闭事件。
    /// </summary>
    public event EventHandler<NetworkClosedEventArgs> NetworkClosed
    {
        add
        {
            m_NetworkClosedEventHandler += value;
        }
        remove
        {
            m_NetworkClosedEventHandler -= value;
        }
    }

    /// <summary>
    /// 网络心跳包丢失事件。
    /// </summary>
    public event EventHandler<NetworkMissHeartBeatEventArgs> NetworkMissHeartBeat
    {
        add
        {
            m_NetworkMissHeartBeatEventHandler += value;
        }
        remove
        {
            m_NetworkMissHeartBeatEventHandler -= value;
        }
    }

    /// <summary>
    /// 网络错误事件。
    /// </summary>
    public event EventHandler<NetworkErrorEventArgs> NetworkError
    {
        add
        {
            m_NetworkErrorEventHandler += value;
        }
        remove
        {
            m_NetworkErrorEventHandler -= value;
        }
    }

    /// <summary>
    /// 用户自定义网络错误事件。
    /// </summary>
    public event EventHandler<NetworkCustomErrorEventArgs> NetworkCustomError
    {
        add
        {
            m_NetworkCustomErrorEventHandler += value;
        }
        remove
        {
            m_NetworkCustomErrorEventHandler -= value;
        }
    }
    private void OnNetworkChannelConnected(NetworkChannelBase networkChannel, object userData)
    {

        if (m_NetworkConnectedEventHandler != null)
        {
            NetworkConnectedEventArgs networkConnectedEventArgs = NetworkConnectedEventArgs.Create(networkChannel, userData);
            m_NetworkConnectedEventHandler(this, networkConnectedEventArgs);
            ReferencePool.Release(networkConnectedEventArgs);
        }
    }

    private void OnNetworkChannelClosed(NetworkChannelBase networkChannel)
    {
        if (m_NetworkClosedEventHandler != null)
        {
            NetworkClosedEventArgs networkClosedEventArgs = NetworkClosedEventArgs.Create(networkChannel);
            m_NetworkClosedEventHandler(this, networkClosedEventArgs);
            ReferencePool.Release(networkClosedEventArgs);
        }
    }

    private void OnNetworkChannelMissHeartBeat(NetworkChannelBase networkChannel, int missHeartBeatCount)
    {
        if (m_NetworkMissHeartBeatEventHandler != null)
        {
            NetworkMissHeartBeatEventArgs networkMissHeartBeatEventArgs = NetworkMissHeartBeatEventArgs.Create(networkChannel, missHeartBeatCount);
            m_NetworkMissHeartBeatEventHandler(this, networkMissHeartBeatEventArgs);
            ReferencePool.Release(networkMissHeartBeatEventArgs);
        }
    }

    private void OnNetworkChannelError(NetworkChannelBase networkChannel, NetworkErrorCode errorCode, SocketError socketErrorCode, string errorMessage)
    {
        if (m_NetworkErrorEventHandler != null)
        {
            NetworkErrorEventArgs networkErrorEventArgs = NetworkErrorEventArgs.Create(networkChannel, errorCode, socketErrorCode, errorMessage);
            m_NetworkErrorEventHandler(this, networkErrorEventArgs);
            ReferencePool.Release(networkErrorEventArgs);
        }
    }

    private void OnNetworkChannelCustomError(NetworkChannelBase networkChannel, object customErrorData)
    {
        if (m_NetworkCustomErrorEventHandler != null)
        {
            NetworkCustomErrorEventArgs networkCustomErrorEventArgs = NetworkCustomErrorEventArgs.Create(networkChannel, customErrorData);
            m_NetworkCustomErrorEventHandler(this, networkCustomErrorEventArgs);
            ReferencePool.Release(networkCustomErrorEventArgs);
        }
    }
}
