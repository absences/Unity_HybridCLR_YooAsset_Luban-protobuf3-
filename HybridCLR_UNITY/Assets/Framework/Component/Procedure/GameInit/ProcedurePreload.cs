
using Cysharp.Threading.Tasks;
using GameFramework.Network;
using GameFramework.Procedure;
using System.Net;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameInit
{
    public class ProcedurePreload : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            WaitAllManagerInit(procedureOwner).Forget();
        }

        async UniTask WaitAllManagerInit(ProcedureOwner procedureOwner)
        {

            await GameEnter.Config.Init();

            //...
            await GameEnter.HotFix.Init();

            //可以分批次初始化
            //await ...

            //GameEnter.UI.CloseUIForm<UILoadForm>();

            //GameEnter.UI.OpenUIForm("TestForm","Base");





        }
    }
}