using GameServer.Manager;

namespace GameServer
{
    internal class Program
    {

        static void Main(string[] args)
        {

            //  RunServer.StartServer(11431);  //tcp

            KcpServer.StartServer(11431); //kcp
        }
    }
}
