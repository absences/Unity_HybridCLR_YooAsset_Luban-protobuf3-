using System;
using System.Net;
using Telepathy;

namespace GameFramework.Network
{
    public class TcpNetworkChannel : NetworkChannelBase
    {
        private Client client;
        public override bool Connected => client.Connected;

        public override ServiceType ServiceType => ServiceType.Tcp;

        public TcpNetworkChannel(string name, INetworkChannelHelper networkChannelHelper) : base(name, networkChannelHelper)
        {
            client = new Client(DefaultBufferLength);
            m_HeartBeatInterval = DefaultHeartBeatInterval;
            client.OnConnected = OnConnected;
            client.OnDisconnected = OnDisconnected;

            client.OnData = OnData;
        }

        private void OnData(ArraySegment<byte> segment)
        {
            m_ReceivedPacketCount++;

            ProcessPacket(segment.Array, segment.Offset, segment.Count - segment.Offset);
        }

        private void OnDisconnected()
        {
            NetworkChannelClosed?.Invoke(this);
        }

        private void OnConnected()
        {
            NetworkChannelConnected?.Invoke(this, client.UserData);
            m_Active = true;

            m_HeartBeatState.Reset(true);
        }

        public override void Connect(IPAddress ipAddress, int port, object userData)
        {
            switch (ipAddress.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    m_AddressFamily = AddressFamily.IPv4;
                    break;
                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    m_AddressFamily = AddressFamily.IPv6;
                    break;
                default:
                    break;
            }

            client.Connect(ipAddress, port, userData);
        }
        public override void Connect(string ip, int port, object userData)
        {
            client.Connect(ip, port, userData);
        }
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);

            client.Tick(100);
        }
        public override void Shutdown()
        {
            base.Shutdown();
            client.Disconnect();
        }

        public override void Send<T>(int msgID, T packet)
        {
            m_SentPacketCount++;
            client.Send(Encode(msgID, packet));
        }
    }
}