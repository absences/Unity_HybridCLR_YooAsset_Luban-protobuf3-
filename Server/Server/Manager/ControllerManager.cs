using GameServer.Manager;
using Google.Protobuf;
using pbnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telepathy;

namespace GameServer
{
    /// <summary>
    /// 客户端行为总入口
    /// </summary>
    class ControllerManager
    {
        static Dictionary<int, MessageParser> ParseDic = new Dictionary<int, MessageParser>()
        {
            //    [ReqRegInfo.ID] = ReqRegInfo.Parser,
            //    [C2G_JoinRoom.ID] = C2G_JoinRoom.Parser,
            //    [C2G_LoadingProgress.ID] = C2G_LoadingProgress.Parser,
            //    [Msg_PlayerInput.ID] = Msg_PlayerInput.Parser,
            //    [Msg_HashCode.ID] = Msg_HashCode.Parser,
            //    [C2G_PlayerPing.ID] = C2G_PlayerPing.Parser,
            //    [C2G_ReqMissFrame.ID] = C2G_ReqMissFrame.Parser,

            [C2G_HeartNeat.ID] = C2G_HeartNeat.Parser,
          
        };
        public const int MSG_HEAD_LEN = 4;

        //Models
        private LoginController loginCtrl;
        //private RoomController roomCtrl;
        //public  ClientController clientCtrl;

        private Server Server;
        public ControllerManager(Server server)
        {
            Server = server;
            // clientCtrl=new ClientController (server);
            loginCtrl = new LoginController(server);
          //  roomCtrl = new RoomController(server);
        }

        //void HandleRequest(int connectID, int MsgID, IMessage msg)
        //{
        //    var client= clientCtrl.GetUserByConnectID(connectID);
        //    if (client==null)
        //    {
        //        return;
        //    }

        //    if (MsgID == ReqRegInfo.ID)
        //    {
        //        loginCtrl.DefaultHandle(client, MsgID, msg);

        //        var room = roomCtrl.GetRoomByRoleID(client.ID);
        //        if (room != null)//玩家重回房间
        //        {
        //            room.AddClient(client);
        //            client.SetGameRoom(room);
        //        }
        //    }
        //    else
        //    {
        //        roomCtrl.DefaultHandle(client, MsgID, msg);
        //    }
        //}
        RespHeartBeat heartBeat = new RespHeartBeat();
        public void OnData(int connectID, ArraySegment<byte> data)
        {
            if (data.Count < MSG_HEAD_LEN)
                throw new Exception("Msg Len Err, Missing msg head");
            int MsgID = Utils.BytesToIntBigEndian(data.Array, data.Offset);

            if (!ParseDic.TryGetValue(MsgID, out MessageParser Parser))
            {
                Log.Warning("未解析ID" + MsgID);
                return;
            }

            var info = Parser.ParseFrom(data.Array, data.Offset + MSG_HEAD_LEN, data.Count - MSG_HEAD_LEN - data.Offset);

            Console.WriteLine(MsgID+"_"+ info);
            //HandleRequest(connectID, MsgID, info);

            if (MsgID == C2G_HeartNeat.ID)
            {
                loginCtrl.Send(connectID, RespHeartBeat.ID, heartBeat);
            }
            else
            {
                Msg_G2C_JoinSuccess msg = new Msg_G2C_JoinSuccess();

                loginCtrl.Send(connectID, Msg_G2C_JoinSuccess.ID, msg);
            }


        }
        public void DoUpdate(float dt)
        {
           // roomCtrl.Update(dt);
        }
    }
}
