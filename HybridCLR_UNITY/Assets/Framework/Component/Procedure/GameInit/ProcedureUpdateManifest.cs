using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;

using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureUpdateManifest : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("更新资源清单！！！");

            UpdateManifest(procedureOwner).Forget();
        }

        private async UniTask UpdateManifest(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            var resource = GameEnter.Resource;

            var operation = resource.UpdatePackageManifestAsync(resource.PackageVersion);

            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                ChangeState<ProcedureCreateDownloader>(procedureOwner);
            }
            else
            {
                Log.Error(operation.Error);

              //  UILoadTip.ShowMessageBox($"用户尝试更新清单失败！点击确认重试 \n \n <color=#FF0000>原因{operation.Error}</color>", MessageShowType.TwoButton,
               //     LoadStyle.StyleEnum.Style_Retry
                //    , () => { ChangeState<ProcedureUpdateManifest>(procedureOwner); }, UnityEngine.Application.Quit);
            }
        }
    }
}