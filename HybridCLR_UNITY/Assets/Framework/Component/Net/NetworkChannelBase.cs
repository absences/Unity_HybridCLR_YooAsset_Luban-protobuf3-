using Google.Protobuf;
using pbnet;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Telepathy;
namespace GameFramework.Network
{
    /// <summary>
    /// 网络频道基类。
    /// </summary>

    public abstract class NetworkChannelBase : INetworkChannel
    {
        public string Name => m_Name;

        public virtual bool Connected => throw new NotImplementedException();

        public virtual ServiceType ServiceType => throw new NotImplementedException();

        public AddressFamily AddressFamily => m_AddressFamily;

        /// <summary>
        /// 获取累计发送的消息包数量。
        /// </summary>
        public int SentPacketCount => m_SentPacketCount;

        /// <summary>
        /// 获取累计已接收的消息包数量。
        /// </summary>
        public int ReceivedPacketCount => m_ReceivedPacketCount;

        public int MissHeartBeatCount => m_HeartBeatState.MissHeartBeatCount;

        public float Ping => m_HeartBeatState.ping;

        protected readonly HeartBeatState m_HeartBeatState;

        protected int m_SentPacketCount;

        protected int m_ReceivedPacketCount;

        protected bool m_ResetHeartBeatElapseSecondsWhenReceivePacket;

        protected AddressFamily m_AddressFamily;


        /// <summary>
        /// 获取心跳等待时长，以秒为单位。
        /// </summary>
        public float HeartBeatElapseSeconds
        {
            get
            {
                return m_HeartBeatState.HeartBeatElapseSeconds;
            }
        }
        protected float m_HeartBeatInterval;
        /// <summary>
        /// 获取或设置心跳间隔时长，以秒为单位。
        /// </summary>
        public float HeartBeatInterval
        {
            get
            {
                return m_HeartBeatInterval;
            }
            set
            {
                m_HeartBeatInterval = value;
            }
        }

        public virtual void Close()
        {
            NetworkChannelClosed?.Invoke(this);
            m_HeartBeatState.Reset(true);
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            Connect(ipAddress, port, null);
        }

        public virtual void Connect(IPAddress ipAddress, int port, object userData)
        {

        }

        public virtual void Connect(string ip, int port, object userData)
        {

        }

        public virtual void Send<T>(int msgID, T packet) where T : IMessage
        {
            
        }

        protected void ProcessPacket(byte[] array, int offset, int count)
        {
            object customErrorData = null;
            var msg = m_NetworkChannelHelper.DeserializePacket(array, offset, count, out customErrorData);
           
            if(customErrorData is RespHeartBeat)
            {
                m_HeartBeatState.Reset(true);
            }
        }
        protected ArraySegment<byte> Encode(int commandID, IMessage msg)
        {
            sendBuffer.Seek(0, SeekOrigin.Begin);
            Utils.WriteIntBigEndian(commandID, sendBuffer);
            msg.WriteTo(sendBuffer);
            return new ArraySegment<byte>(sendBuffer.GetBuffer(), 0, (int)sendBuffer.Position);
        }

        private readonly string m_Name;

        protected readonly INetworkChannelHelper m_NetworkChannelHelper;

        /// <summary>
        /// 心跳间隔
        /// </summary>
        protected const float DefaultHeartBeatInterval = 5;
        /// <summary>
        /// buffer长度
        /// </summary>
        protected const int DefaultBufferLength = 1024 * 64;

        protected bool m_Active;

        public GameFrameworkAction<NetworkChannelBase, object> NetworkChannelConnected;
        public GameFrameworkAction<NetworkChannelBase> NetworkChannelClosed;
        public GameFrameworkAction<NetworkChannelBase, int> NetworkChannelMissHeartBeat;
        public GameFrameworkAction<NetworkChannelBase, NetworkErrorCode, SocketError, string> NetworkChannelError;
        public GameFrameworkAction<NetworkChannelBase, object> NetworkChannelCustomError;

        protected MemoryStream sendBuffer;
        public NetworkChannelBase(string name, INetworkChannelHelper networkChannelHelper)
        {
            m_Name = name;
            m_NetworkChannelHelper = networkChannelHelper;

            m_Active = false;
            m_HeartBeatState = new HeartBeatState();
            
            sendBuffer = new MemoryStream(DefaultBufferLength);
            m_ResetHeartBeatElapseSecondsWhenReceivePacket = false;


            networkChannelHelper.Initialize(this);
        }
        /// <summary>
        /// 网络频道轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (!m_Active)
            {
                return;
            }

            if (m_HeartBeatInterval > 0f)
            {
                bool sendHeartBeat = false;
                int missHeartBeatCount = 0;

                if (!m_Active)
                {
                    return;
                }

                m_HeartBeatState.HeartBeatElapseSeconds += elapseSeconds;
                if (m_HeartBeatState.HeartBeatElapseSeconds >= m_HeartBeatInterval)
                {
                    sendHeartBeat = true;
                    missHeartBeatCount = m_HeartBeatState.MissHeartBeatCount;
                    m_HeartBeatState.HeartBeatElapseSeconds = 0f;
                    m_HeartBeatState.MissHeartBeatCount++;
                }


                if (sendHeartBeat && m_NetworkChannelHelper.SendHeartBeat())
                {
                    if (missHeartBeatCount > 0 && NetworkChannelMissHeartBeat != null)
                    {
                        NetworkChannelMissHeartBeat(this, missHeartBeatCount);
                    }
                }
            }
        }

        /// <summary>
        /// 关闭网络频道。
        /// </summary>
        public virtual void Shutdown()
        {
            Close();
            m_NetworkChannelHelper.Shutdown();
        }

    }
}