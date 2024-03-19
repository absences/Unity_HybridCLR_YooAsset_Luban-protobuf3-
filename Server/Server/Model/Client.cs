using GameServer.Model;
using MySql.Data.MySqlClient;
using pbnet;
using System;

namespace GameServer
{
    public class Client
    {
        public int ConnectID { private set; get; }
        public int ID { private set; get; }//uuid >0登录成功

        public GameRoom Room { private set; get; }

        public int roomActorID;
        public Client(int connectID)
        {
            ConnectID = connectID;

            //  mysqlConn = ConnHelper.Connect();
        }
        private MySqlConnection mysqlConn;
        public MySqlConnection MySQLConn
        {
            get { return mysqlConn; }
        }
        /// <summary>
        /// 上号
        /// </summary>
        /// <param name="id"></param>
        public void SetUserData(ReqRegInfo msg)
        {
            ID = msg.RoleID;
            //...
        }
        public void SetGameRoom(GameRoom room)
        {
            Room = room;
        }
    }
}
