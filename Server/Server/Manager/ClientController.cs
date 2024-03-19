using System;
using System.Collections.Generic;
using Telepathy;

namespace GameServer
{
    public class ClientController
    {
        public Server server { private set; get; }
        Dictionary<int, Client> clients = new Dictionary<int, Client>();//key 是connectid
        public ClientController(Server server)
        {
            this.server = server;
        }

        public void AddClient(int connectid)
        {
            Console.WriteLine("client{0} connect", connectid);
            clients.Add(connectid, new Client(connectid));
        }
        public void RemoveClient(int connectid)
        {
            Console.WriteLine("client{0} disconnect", connectid);
            var client = GetUserByConnectID(connectid);

            if (client.Room != null)
            {
                client.Room.QuitRoom(client);
                client.SetGameRoom(null);
            }

            clients.Remove(connectid);

        }
        /// <summary>
        /// 根据连接ID返回user信息
        /// </summary>
        /// <param name="connectID"></param>
        /// <returns></returns>
        public Client GetUserByConnectID(int connectID)
        {
           clients.TryGetValue(connectID, out Client client);
           return client;
        }
    }
}
