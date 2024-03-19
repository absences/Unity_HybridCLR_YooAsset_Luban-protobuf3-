using Google.Protobuf;
using pbnet;
using Telepathy;

namespace GameServer
{
    internal class LoginController : BaseController
    {

        public LoginController(Server server) : base(server)
        {

        }
        public override void DefaultHandle(Client client, int MsgID, IMessage info)
        {
            switch (MsgID)
            {
                case ReqRegInfo.ID:
                    ReqRegInfo msg = info as ReqRegInfo;
                    client.SetUserData(msg);
                    G2C_LoginSuccess g2C_LoginSuccess = new G2C_LoginSuccess();
                    g2C_LoginSuccess.Info = new RespPlayerInfo() {
                     RoleID = msg.RoleID,
                    };

                    Send( client, G2C_LoginSuccess.ID, g2C_LoginSuccess);
                    break;
            }
        }

        public override void Update(float dt)
        {

        }
    }
}
