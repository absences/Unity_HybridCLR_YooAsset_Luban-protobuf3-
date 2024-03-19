using System;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class ConnHelper
    {
        public const string CONNECTIONSTRING = "datasource=192.168.3.60;port=3306;database=game01;user=root;pwd=root;";

        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(CONNECTIONSTRING);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("链接数据库的时候实现异常：" + e);
                return null;
            }

        }
        public static void CloseConnection(MySqlConnection conn)
        {
            if (conn != null)
                conn.Close();
            else
            {
                Console.WriteLine("MySqlConnection不能为空");
            }
        }
    }
}
