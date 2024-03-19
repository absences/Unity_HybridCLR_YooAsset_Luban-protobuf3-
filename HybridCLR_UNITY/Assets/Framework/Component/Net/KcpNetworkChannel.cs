
using Cysharp.Threading.Tasks;
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Threading.Tasks;
using Telepathy;

namespace GameFramework.Network
{
    public class KcpNetworkChannel : NetworkChannelBase, IKcpCallback
    {
        public override ServiceType ServiceType => base.ServiceType;


        UdpClient client;

        public override bool Connected => true;
        public IPEndPoint EndPoint { get; set; }

        public SimpleSegManager.Kcp kcp { private set; get; }
        public KcpNetworkChannel(string name, int clientPort, INetworkChannelHelper networkChannelHelper)
            : base(name, networkChannelHelper)
        {
            client = new UdpClient(clientPort);

            m_HeartBeatInterval = 0;//kcp 不需要心跳

        }

        public override void Connect(IPAddress ipAddress, int port, object userData)
        {
            kcp = new SimpleSegManager.Kcp(2001, this);//频道号

            this.EndPoint = new IPEndPoint(ipAddress, port);

            BeginRecv();

            StartRecv();
        }

        public const int MSG_HEAD_LEN = 4;

        async void StartRecv()
        {
            while (true)
            {
                var result = await ReceiveAsync();

                ProcessPacket(result, 0, result.Length);
            }
        }
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            var s = buffer.Memory.Span.Slice(0, avalidLength).ToArray();
            client.SendAsync(s, s.Length, EndPoint);
            buffer.Dispose();
        }

        private async void BeginRecv()
        {
            var res = await client.ReceiveAsync();
            EndPoint = res.RemoteEndPoint;
            kcp.Input(res.Buffer);
            BeginRecv();
        }

        public override void Send<T>(int msgID, T packet)
        {
            m_SentPacketCount++;
            var bytes = Encode(msgID, packet);
            kcp.Send(bytes);
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);

            kcp.Update(DateTimeOffset.UtcNow);
        }
        public async UniTask<byte[]> ReceiveAsync()
        {
            var (buffer, avalidLength) = kcp.TryRecv();
            while (buffer == null)
            {
                await Task.Delay(10);
                (buffer, avalidLength) = kcp.TryRecv();
            }

            var s = buffer.Memory.Span.Slice(0, avalidLength).ToArray();
            return s;
        }
    }
}