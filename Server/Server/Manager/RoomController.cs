using GameServer.Model;
using Google.Protobuf;
using Lockstep.Util;
using pbnet;
using System.Collections.Generic;
using Telepathy;

namespace GameServer.Manager
{
    public  class RoomController : BaseController
    {
        public RoomController(Server server) : base(server)
        {
        }

        private List<GameRoom> roomList = new List<GameRoom>();

        public GameRoom GetRoomByRoleID(int roleID)
        {
            foreach (GameRoom room in roomList)
            {

                if (room.IsClientInRoom(roleID))
                {
                    return room;    
                }
            }
            return null;
        }

        public override void DefaultHandle(Client client, int MsgID, IMessage info)
        {
            switch (MsgID)
            {
                case C2G_JoinRoom.ID:
                    {
                        if (client.Room == null)
                        {
                            GameRoom room = null;
                            foreach (GameRoom item in roomList)
                            {
                                if (item.state == EGameState.WaitPlayer)
                                {
                                    room = item;
                                    break;
                                }
                            }
                            if (room == null)
                                room = new GameRoom(this);

                            client.SetGameRoom(room);
                            roomList.Add(room);

                            room.AddClient(client);
                        }
                    }
                    break;
                case C2G_PlayerPing.ID:
                    {
                        var msg = info as C2G_PlayerPing;
                        Send(client, Msg_G2C_PlayerPing.ID, new Msg_G2C_PlayerPing
                        {
                            LocalId = msg.LocalId,
                            SendTimestamp = msg.SendTimestamp,
                            TimeSinceServerStart = LTime.realtimeSinceStartupMS - client.Room._gameStartTimestampMs
                        });
                    }

                    break;
                case Msg_PlayerInput.ID:
                    client.Room.C2G_PlayerInput(info as Msg_PlayerInput);
                    break;
                case Msg_HashCode.ID:
                    {
                        var msg = info as Msg_HashCode;
                        client.Room.C2G_HashCode(client, msg);
                    }
                    break;
                case C2G_LoadingProgress.ID:
                    {
                        var msg = info as C2G_LoadingProgress;
                        client.Room.C2G_LoadingProgress(client, msg);
                    }
                    break;
                case C2G_ReqMissFrame.ID:
                    client.Room.C2G_ReqMissFrame(info as C2G_ReqMissFrame);

                    break;
            }
        }

        public override void Update(float dt)
        {
            foreach (var item in roomList)
            {
                item.DoUpdate(dt);
            }
        }
    }
}
