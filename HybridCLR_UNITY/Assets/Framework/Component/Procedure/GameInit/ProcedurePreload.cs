
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
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

            await (
                    GameEnter.Config.Init()
                    , GameEnter.HotFix.Init()

                );
            //���Է����γ�ʼ��
            //await ...

            //GameEnter.UI.CloseUIForm<UILoadForm>();

            //GameEnter.UI.OpenUIForm("TestForm","Base");





        }
    }
}