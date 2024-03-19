using Cysharp.Threading.Tasks;
using GameFramework;
using pbnet;
using System.Net;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace HotfixMain
{
    public class HotFixActivity
    {

        public static void Init()
        {
            GameEnter.UI.CloseUIForm<UILoadForm>();

            GameEnter.UI.OpenUIForm(nameof(TestForm), "Base");

            GameEnter.Network.Init();
            GameEnter.Event.Subscribe(NetworkConnectedEventArgs.EventId, OnConnectServer);

            network = new NetworkChannelHelper();
            var channel = GameEnter.Network.CreateNetworkChannel("Game",
                GameFramework.Network.ServiceType.Kcp, network);
            channel.Connect(IPAddress.Parse("192.168.1.92"), 11431);
           // channel.Connect("192.168.1.92", 11431, null);


        }
        private static NetworkChannelHelper network;
        private static void OnConnectServer(object sender, BaseEventArgs e)
        {
            Log.Info("connected server");
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {

                ReqRegInfo msg = new ReqRegInfo();
                msg.Name = "absence";
                network.Send(ReqRegInfo.ID, msg);
            }
        }
        public static void ShutDown()
        {

        }
    }
}
