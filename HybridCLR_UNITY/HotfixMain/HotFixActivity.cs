using Cysharp.Threading.Tasks;
using GameFramework;
using System.Net;
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

            var channel = GameEnter.Network.CreateNetworkChannel("Game",
                GameFramework.Network.ServiceType.Tcp, new NetworkChannelHelper());
            channel.Connect(IPAddress.Parse("192.168.1.92"), 11431);
           // channel.Connect("192.168.1.92", 11431, null);


        }

        private static void OnConnectServer(object sender, BaseEventArgs e)
        {
            Log.Info("connected server");
        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {


        }
        public static void ShutDown()
        {

        }
    }
}
