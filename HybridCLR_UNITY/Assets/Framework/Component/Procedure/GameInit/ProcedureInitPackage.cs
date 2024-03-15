using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureInitPackage : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            InitAssetsPackage(procedureOwner).Forget();
        }

        async UniTask InitAssetsPackage(ProcedureOwner procedureOwner)
        {
            //确保其他模块start初始化了
            await UniTask.Delay(1000);

            var resource = GameEnter.Resource;

            var initSuccess = await resource.InitPackage();
            Log.Info("Init package "+ initSuccess);
            await resource.InitGameAsset();
            Log.Info("init tmp success");
            await GameEnter.UI.OpenUIFormAsync(nameof(UILoadForm), "Base");

            TMPAssetLoader.Initialize(resource.ResourceManager);

            // 编辑器模式。
            if (resource.PlayMode == EPlayMode.EditorSimulateMode)
            {
                Log.Info("Editor resource mode detected.");
                ChangeState<ProcedurePreload>(procedureOwner);
            }
            // 单机模式。
            else if (resource.PlayMode == EPlayMode.OfflinePlayMode)
            {
                Log.Info("Package resource mode detected.");
                ChangeState<ProcedureInitResources>(procedureOwner);
            }
            // 可更新模式。
            else if (resource.PlayMode == EPlayMode.HostPlayMode)
            {
                // 打开启动UI。


               Log.Info("Updatable resource mode detected.");

                ChangeState<ProcedureUpdateVersion>(procedureOwner);
            }
            //else if(resource.PlayMode== EPlayMode.WebPlayMode)//webgl
            //{

            //}

        }
    }
}