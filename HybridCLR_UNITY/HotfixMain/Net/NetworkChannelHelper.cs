using GameFramework;
using GameFramework.Network;
using Google.Protobuf;
using pbnet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityGameFramework.Runtime;

namespace HotfixMain
{
    public class NetworkChannelHelper : INetworkChannelHelper
    {
        private INetworkChannel _networkChannel;



        private C2G_HeartNeat _heartMsg;
        public void Initialize(INetworkChannel networkChannel)
        {
            _networkChannel = networkChannel;
            _heartMsg = new C2G_HeartNeat();

            GameEnter.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            GameEnter.Event.Subscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            GameEnter.Event.Subscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            GameEnter.Event.Subscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            GameEnter.Event.Subscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
        }

        public bool SendHeartBeat()
        {
            if (_networkChannel.Connected)
            {
                _networkChannel.Send(C2G_HeartNeat.ID, _heartMsg);
                return true;
            }
            return false;
        }
        public void Send(int msgID, IMessage msg)
        {
            if (_networkChannel.Connected)
            {
                _networkChannel.Send(msgID, msg);
            }
        }

        public void Shutdown()
        {
            GameEnter.Event.Unsubscribe(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            GameEnter.Event.Unsubscribe(NetworkClosedEventArgs.EventId, OnNetworkClosed);
            GameEnter.Event.Unsubscribe(NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
            GameEnter.Event.Unsubscribe(NetworkErrorEventArgs.EventId, OnNetworkError);
            GameEnter.Event.Unsubscribe(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

            _networkChannel = null;
        }


        private void OnNetworkConnected(object sender, BaseEventArgs e)
        {
            var ne = (NetworkConnectedEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' connected", ne.NetworkChannel.Name);

            //_networkChannel.Send(new C2S100000().BuildPacket());
        }

        private void OnNetworkClosed(object sender, BaseEventArgs e)
        {
            var ne = (NetworkClosedEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' closed.", ne.NetworkChannel.Name);
        }

        private void OnNetworkMissHeartBeat(object sender, BaseEventArgs e)
        {
            var ne = (NetworkMissHeartBeatEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' miss heart beat '{1}' times.", ne.NetworkChannel.Name, ne.MissCount.ToString());

            if (ne.MissCount < 2)
            {
                return;
            }

            ne.NetworkChannel.Close();
        }

        private void OnNetworkError(object sender, BaseEventArgs e)
        {
            var ne = (NetworkErrorEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }

            Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", ne.NetworkChannel.Name, ne.ErrorCode.ToString(), ne.ErrorMessage);

            ne.NetworkChannel.Close();
        }

        private void OnNetworkCustomError(object sender, BaseEventArgs e)
        {
            var ne = (NetworkCustomErrorEventArgs)e;
            if (ne.NetworkChannel != _networkChannel)
            {
                return;
            }
        }
        public const int MSG_HEAD_LEN = 4;
        public IMessage DeserializePacket(byte[] array, int offset, int count, out object customErrorData)
        {
            customErrorData = null;
            if (count < MSG_HEAD_LEN)
                throw new Exception("Msg Len Err, Missing msg head");

            int MsgID = Telepathy.Utils.BytesToIntBigEndian(array, offset);

            if (MsgID == RespFullyLst.ID)
            {
                var resps = RespFullyLst.Parser.ParseFrom(array, offset + MSG_HEAD_LEN, count - offset - MSG_HEAD_LEN);

                foreach (var resp in resps.Resps)
                {
                    if (resp.TypeSign == RespHeartBeat.ID)
                    {
                        customErrorData = RespHeartBeat.Parser.ParseFrom(resp.Data);
                    }
                    else if(resp.TypeSign== Msg_G2C_JoinSuccess.ID)
                    {
                        Log.Info(resp.TypeSign);
                    }
                }
                return resps;
            }
            //else if (MsgID == )customErrorMsg
            //{

            //}

            return null;
        }
    }
}