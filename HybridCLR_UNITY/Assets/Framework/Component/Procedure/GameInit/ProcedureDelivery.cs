using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;
using System.IO;
using UnityEngine.Networking;

using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameInit
{
    public class ProcedureDelivery : ProcedureBase
    {
        private ProcedureOwner _procedureManager;
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _procedureManager = procedureOwner;
            var resource = GameEnter.Resource;
          //  DownLoadFiles(resource.PackageName).Forget();
            //other package...

            ChangeState<ProcedureUpdateManifest>(_procedureManager);
        }
        //async UniTask DownLoadFiles(string packageName)
        //{
        //    var resource = GameEnter.Resource;
        //    var fileName = resource.PackageVersion + "_deliveryFiles";
        //    var sandBoxDeliveryPath = Path.Combine(resource.GetPackageSandboxRootDirectory(packageName), fileName);

        //    var zipFile = fileName + ".zip";

        //    if (!Directory.Exists(sandBoxDeliveryPath))
        //    {
        //        var url = GetMainResUrl(resource, zipFile);
        //        UnityWebRequest request = UnityWebRequest.Get(url);
        //        try
        //        {
        //            await request.SendWebRequest();

        //            if (request.result == UnityWebRequest.Result.Success)
        //            {
        //                UnZipDelivery(request.downloadHandler.data, sandBoxDeliveryPath);
        //            }
        //        }
        //        catch(Exception)
        //        {
        //            Log.Info(request.url, request.error);
        //        }
               
        //        ChangeState<ProcedureUpdateManifest>(_procedureManager);
        //    }
        //    await UniTask.CompletedTask;
        //}

        string GetMainResUrl(ResourceComponent resource, string fileName)
        {
            var VersionType = resource.VersionType;
            var CodeVersion = resource.AppVersion;
            return string.Format("{0}/{1}/{2}_{3}/{4}",
                    resource.ResourceSourceUrl, GameUtil.GetPlatformName(), VersionType, CodeVersion, fileName);
        }

        void UnZipDelivery(byte[] data, string sandBoxDeliveryPath)
        {
            ZipUtility.UnzipFile(data, sandBoxDeliveryPath);//解压自动创建目录
        }
    }
}