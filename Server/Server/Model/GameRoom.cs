using GameServer.Manager;
using Google.Protobuf;
using Lockstep.FakeServer;
using Lockstep.Util;
using pbnet;
using System;
using System.Collections.Generic;
using Telepathy;

namespace GameServer.Model
{
    public enum EGameState
    {
        /// <summary>
        /// 默认
        /// </summary>
        Idle,

        WaitPlayer,
        /// <summary>
        /// 玩家加载中
        /// </summary>
        Loading,
        /// <summary>
        /// 加载结束，等待玩家输入
        /// </summary>
        PartLoaded,
        /// <summary>
        /// gaming
        /// </summary>
        Playing,
        PartFinished,
        FinishAll,
    }
    class GameRole
    {
        public int localID;//房间内的编号，
        public string name;
        public int clientID;//连接编号
    }

    public  class GameRoom//todo 回收房间
    {

        public static int ID;
        //加入一个玩家 state==wait player

        //玩家数量满足，== Loading..等待所有玩家加载完毕，超时1分钟判定准备不成功

        //PartLoaded  都加载完毕，等待客户端输入阶段（由客户端逻辑触发发送玩家输入）

        private RoomController roomController;
        private int roomID;
        public GameRoom (RoomController controller)
        {
            roomController = controller;
            roomID = ID++;
        }
        private void Send(Client client, int id, IMessage msg)
        {
            roomController.Send(client.ConnectID,id,msg);
        }
        public const int MaxPlayerCount = 2;

        public int Tick = 0;
        private float _timeSinceLoaded;
        private float _firstFrameTimeStamp = 0;
        private float _waitTimer = 0;

        public long _gameStartTimestampMs = -1;
        public int _ServerTickDealy = 0;

        public int TickSinceGameStart =>
            (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

        private int[] _playerLoadingProgress;

        public EGameState state = EGameState.Idle;
        public int Seed { get; set; }

        private List<Client> clientList = new List<Client>();

        Dictionary<int, int> roleID_actorID_Dic = new Dictionary<int, int>(MaxPlayerCount);////当前房间正在游戏的 玩家ID与ActorID映射
        public void QuitRoom(Client client)
        {
            if(state== EGameState.WaitPlayer)
            {
                //等待阶段退出了玩家
                
            }
            clientList.Remove(client);

        }
        /// <summary>
        /// 玩家是否在此房间
        /// </summary>
        /// <param name="clientRoleID"></param>
        /// <returns></returns>
        public bool IsClientInRoom(int clientRoleID)
        {
            foreach (var item in roleID_actorID_Dic)
            {
                if (item.Key == clientRoleID)
                    return true;
            }
            return false;
        }
        public void AddClient(Client client)
        {
            Log.Info(roomID+" room add role" );
            if (roleID_actorID_Dic.TryGetValue(client.ID,out int actorID))
            {
                client.roomActorID = actorID;
            }
            else
            {
                client.roomActorID = clientList.Count;
            }
         
            clientList.Add(client);

            //广播此房间的所有玩家信息（展示）

            var join = new Msg_G2C_JoinSuccess
            {
                Info = new RespPlayerInfo()
                {
                    ActorID = client.roomActorID,
                    RoleID = client.ID,
                }
            };

            if (state== EGameState.Idle|| state== EGameState.WaitPlayer)//开始游戏
            {
                Brocast(Msg_G2C_JoinSuccess.ID, join);

                state = EGameState.WaitPlayer;
               
                if (clientList.Count == MaxPlayerCount)
                {
                    Log.Info("game start room id:" + roomID);
                    DoStart();
                }
            }
            else
            {
                //重连加入的
               // SendAllRoomInfo();
               Send(client, Msg_G2C_JoinSuccess.ID, join);

                var info = new Msg_G2C_GameStartInfo();
                info.UserCount = MaxPlayerCount;
                info.GameSeed = Seed;
                foreach (var c in clientList)
                {
                    info.Infos.Add(new RespPlayerInfo
                    {
                        Name = c.ID.ToString(),
                        ActorID = c.roomActorID,
                        RoleID = c.ID
                    });
                }
                Send(client, Msg_G2C_GameStartInfo.ID, info);
            }
        }
      
        void DoStart()
        {
            state = EGameState.Loading;

            _playerLoadingProgress = new int[MaxPlayerCount];

            Seed = LRandom.Range(1, 100000);
            Tick = 0;
            _timeSinceLoaded = 0;
            _firstFrameTimeStamp = 0;

            foreach (var item in clientList)
            {
                roleID_actorID_Dic.Add(item.ID, item.roomActorID);
            }
            SendAllRoomInfo();

            // Log.Info("game start!!!");
        }
        void SendAllRoomInfo()
        {
            var info = new Msg_G2C_GameStartInfo();
            info.UserCount = MaxPlayerCount;
            info.GameSeed = Seed;
            foreach (var client in clientList)
            {
                info.Infos.Add(new RespPlayerInfo
                {
                    Name = client.ID.ToString(),
                    ActorID = client.roomActorID,
                    RoleID = client.ID
                }) ;
            }
            Brocast(Msg_G2C_GameStartInfo.ID, info);
        }
        public void DoUpdate(float deltaTime)
        {
            _timeSinceLoaded += deltaTime;
            _waitTimer += deltaTime;
            if (state != EGameState.Playing)
                return;
            if (_gameStartTimestampMs <= 0)
                return;
            while (Tick < TickSinceGameStart)
            {
                _CheckBorderServerFrame(true);
            }
        }

  
        /// <summary>
        /// 广播frame消息 没有就填补
        /// </summary>
        /// <param name="isForce">true 强制广播此帧消息</param>
        /// <returns></returns>
        private bool _CheckBorderServerFrame(bool isForce = false)
        {
            // Log.Info(isForce + "  " + Tick + "/" + state.ToString());
            if (state != EGameState.Playing)
                return false;
            var frame = GetOrCreateFrame(Tick);
            var inputs = frame.Inputs;

            if (!isForce && inputs.Count < MaxPlayerCount)
            {
                //是否所有的输入  都已经等到
                return false;
            }

            int length = inputs.Count;
            //将所有未到的包 给予默认的输入
            for (int i = length; i < MaxPlayerCount; i++)
            {
                inputs[i] = new PlayerInput()
                {
                    IsMiss = true,
                    Tick = Tick,
                    ActorId = i
                };
            }

            var msg = new Msg_ServerFrames() { Frames = new MutilFrames() };

            int count = Tick < 2 ? Tick + 1 : 3;//将最近的3帧数据发送给客户端

            var frames = new ServerFrame[count];

            for (int i = 0; i < count; i++)
            {
                frames[count - i - 1] = _allHistoryFrames[Tick - i];
            }

            msg.Frames.StartTick = frames[0].Tick;

            msg.Frames.Frames.Add(frames);

            Brocast(Msg_ServerFrames.ID, msg);

            if (_firstFrameTimeStamp <= 0)
            {
                _firstFrameTimeStamp = _timeSinceLoaded;
            }

            if (_gameStartTimestampMs < 0)//收到所有客户端的消息？
            {
                _gameStartTimestampMs =
                    LTime.realtimeSinceStartupMS + NetworkDefine.UPDATE_DELTATIME * _ServerTickDealy;
            }

            Tick++;
            return true;
        }

        //所有需要等待输入到来的Ids
        private List<int> _allNeedWaitInputPlayerIds;
        private List<ServerFrame> _allHistoryFrames = new List<ServerFrame>(); //所有的历史帧

        ServerFrame GetOrCreateFrame(int tick)
        {
            //扩充帧队列
            var frameCount = _allHistoryFrames.Count;
            if (frameCount <= tick)
            {
                var count = tick - _allHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++)
                {
                    _allHistoryFrames.Add(null);
                }
            }
            var frame = _allHistoryFrames[tick];
            if (frame == null)
            {
                frame = new ServerFrame() { Tick = tick };
                _allHistoryFrames[tick] = frame;
            }
            return frame;
        }
        public void C2G_PlayerInput(Msg_PlayerInput msg)
        {
            if (state != EGameState.PartLoaded && state != EGameState.Playing)
                return;

            if (state == EGameState.PartLoaded)
            {
                Log.Info("First input: game start playing");
                state = EGameState.Playing;
            }
            var input = msg.Input;

            if (input.Tick < Tick)//输入帧不是当前帧
            {
                return;
            }

            var frame = GetOrCreateFrame(input.Tick);

            var id = input.ActorId;
            if (!_allNeedWaitInputPlayerIds.Contains(id))//ByteString 内部重写Equals 
            {
                _allNeedWaitInputPlayerIds.Add(id);
            }

            frame.Inputs.Add(id, input);
            _CheckBorderServerFrame(false);
        }
        private Dictionary<int, HashCodeMatcher> _hashCodes = new Dictionary<int, HashCodeMatcher>();

        public void C2G_HashCode(Client client, Msg_HashCode hashInfo)
        {
            var localID = client.roomActorID;

            for (int i = 0; i < hashInfo.HashCodes.Count; i++)
            {
                var code = hashInfo.HashCodes[i];
                var tick = hashInfo.StartTick + i;

                if (_hashCodes.TryGetValue(tick, out HashCodeMatcher matcher1))
                {
                    if (matcher1 == null || matcher1.sendResult[localID])
                    {
                        continue;
                    }

                    if (matcher1.hashCode != code)
                    {
                        OnHashMatchResult(tick, code, false);
                    }

                    matcher1.count = matcher1.count + 1;
                    matcher1.sendResult[localID] = true;
                    if (matcher1.IsMatchered)
                    {
                        OnHashMatchResult(tick, code, true);
                    }
                }
                else
                {
                    var matcher2 = new HashCodeMatcher(MaxPlayerCount);
                    matcher2.count = 1;
                    matcher2.hashCode = code;
                    matcher2.sendResult[localID] = true;
                    _hashCodes.Add(tick, matcher2);
                    if (matcher2.IsMatchered)
                    {
                        OnHashMatchResult(tick, code, true);
                    }
                }
            }

        }

        void OnHashMatchResult(int tick, long hash, bool isMatched)
        {
            if (isMatched)
            {
                _hashCodes[tick] = null;
            }

            if (!isMatched)
            {
                Log.Info($"!!!!!!!!!!!! Hash not match tick{tick} hash{hash} ");
            }
        }
        public const int MaxRepMissFrameCountPerPack = 600;
        public void C2G_ReqMissFrame(C2G_ReqMissFrame msg)
        {

            var nextCheckTick = msg.StartTick;
            // Log($"C2G_ReqMissFrame nextCheckTick id:{player.LocalId}:{nextCheckTick}");

            int count = System.Math.Min(System.Math.Min((Tick - 1), _allHistoryFrames.Count) - nextCheckTick,
                MaxRepMissFrameCountPerPack);

            if (count <= 0) return;

            var resp = new Msg_RepMissFrame();
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = _allHistoryFrames[nextCheckTick + i];
            }

            resp.StartTick = frames[0].Tick;
            resp.Frames.AddRange(frames);
        }
        public void C2G_LoadingProgress(Client client, C2G_LoadingProgress msg)
        {

            if (state != EGameState.Loading)
                return;
            // Log.Info("client " + clientID + " loading" + client.roomActorID + " _" + msg.Progress);
            _playerLoadingProgress[client.roomActorID] = msg.Progress;

            Msg_G2C_LoadingProgress smsg = new Msg_G2C_LoadingProgress();

            smsg.Progress.AddRange(_playerLoadingProgress);

            Brocast(Msg_G2C_LoadingProgress.ID, smsg);

            bool isDone = true;

            for (int i = 0; i < MaxPlayerCount; i++)
            {
                isDone &= _playerLoadingProgress[i] == 100;
            }
            Log.Info(isDone.ToString());
            if (isDone)
            {
                state = EGameState.PartLoaded;
                _allNeedWaitInputPlayerIds = new List<int>();


                Msg_G2C_AllFinishedLoaded msga = new Msg_G2C_AllFinishedLoaded();

                Brocast(Msg_G2C_AllFinishedLoaded.ID, msga);
            }
        }


        void Brocast(int msgid, IMessage msg)
        {
            foreach (var item in clientList)
            {
                Send(item, msgid, msg);
            }
        }
    }
}
