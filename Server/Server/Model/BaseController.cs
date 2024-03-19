using Google.Protobuf;
using pbnet;
using System;
using System.IO;
using Telepathy;

namespace GameServer
{
    public abstract class BaseController
    {
        public const int MaxMessageSize = 16 * 1024;
        static MemoryStream sendBuffer = new MemoryStream(MaxMessageSize);

        public static ArraySegment<byte> Encode(int commandID, IMessage msg)
        {
            sendBuffer.Seek(0, SeekOrigin.Begin);

            var resp = new RespFullyLst();
            Utils.WriteIntBigEndian(RespFullyLst.ID, sendBuffer);
            resp.Resps.Add(new RespRawData()
            {
                TypeSign = commandID,
                Data = msg.ToByteString()
            });

            resp.WriteTo(sendBuffer);

            return new ArraySegment<byte>(sendBuffer.GetBuffer(), 0, (int)sendBuffer.Position);
        }

        protected Server server;
        public BaseController(Server server)
        {
            this.server = server;
        }
        protected bool Send(Client client,int id, IMessage msg)
        {
            return server.Send(client.ConnectID, Encode(id, msg));
        }
        public bool Send(int connectid, int id, IMessage msg)
        {
            return server.Send(connectid, Encode(id,msg));
        }
        public abstract void DefaultHandle(Client client, int MsgID, IMessage info);

        public abstract void Update(float dt);
    }
}
