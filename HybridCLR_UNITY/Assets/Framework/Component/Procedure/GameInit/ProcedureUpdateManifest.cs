using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;

using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureUpdateManifest : ProcedureBase
    {
        protected internal override async void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("更新资源清单！！！");
            var resource = GameEnter.Resource;

            var status1 = await UpdateManifest(GameEnter.Resource.assetPackageName, resource.assetPackage.packageVersion);
            var status2 = await UpdateManifest(GameEnter.Resource.rawfilePackageName, resource.rawfilePackage.packageVersion);

            if (status1 && status2)
                ChangeState<ProcedureCreateDownloader>(procedureOwner);
        }

        private async UniTask<bool> UpdateManifest(string packageName, string packageVersion)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            var resource = GameEnter.Resource;
            var operation = resource.UpdatePackageManifestAsync(packageName, packageVersion, false);

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                Log.Error(operation.Error);

                //  UILoadTip.ShowMessageBox($"用户尝试更新清单失败！点击确认重试 \n \n <color=#FF0000>原因{operation.Error}</color>", MessageShowType.TwoButton,
                //     LoadStyle.StyleEnum.Style_Retry
                //    , () => { ChangeState<ProcedureUpdateManifest>(procedureOwner); }, UnityEngine.Application.Quit);
            }
            return operation.Status == EOperationStatus.Succeed;
        }
    }
}