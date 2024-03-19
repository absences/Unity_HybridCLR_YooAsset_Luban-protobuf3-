using GameFramework.Network;
using Google.Protobuf;
using System;
using System.IO;
using System.Net;
using Telepathy;

public class TcpNetworkChannel : NetworkChannelBase
{
    private Client client;

    public TcpNetworkChannel(string name, INetworkChannelHelper networkChannelHelper) : base(name, networkChannelHelper)
    {

        client = new Client(DefaultBufferLength);

        client.OnConnected = OnConnected;
        client.OnDisconnected = OnDisconnected;

        client.OnData = OnData;
    }

    private void OnData(ArraySegment<byte> segment)
    {
        m_ReceivedPacketCount++;

        ProcessPacket(segment.Array, segment.Offset, segment.Count - segment.Offset);
    }

    public override bool Connected => client.Connected;

    public override ServiceType ServiceType => ServiceType.Tcp;

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
    ArraySegment<byte> Encode(int commandID, IMessage msg)
    {
        sendBuffer.Seek(0, SeekOrigin.Begin);
        Utils.WriteIntBigEndian(commandID, sendBuffer);
        msg.WriteTo(sendBuffer);
        return new ArraySegment<byte>(sendBuffer.GetBuffer(), 0, (int)sendBuffer.Position);
    }
    public override void Send<T>(int msgID, T packet)
    {
        m_SentPacketCount++;
        client.Send(Encode(msgID, packet));
    }
}
