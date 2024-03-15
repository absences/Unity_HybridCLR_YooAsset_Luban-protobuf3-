using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameMain;
using System;
using UnityEngine;
using GameFramework;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("下载完成!!!");
            //save version
            var form = GameEnter.UI.GetUIForm<UILoadForm>();

            form.ShowUpdate("下载完毕");
            form.OnUpdateEnd();

            ChangeState<PreDownLoadContent>(procedureOwner);
        }
    }

    public class PreDownLoadContent : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _procedureOwner = procedureOwner;


            //获取预下载版本

            DownLoad().Forget();
        }

        async UniTask DownLoad()
        {
            var res = GameEnter.Resource;
            await DownLoad(res.assetPackageName);
            await DownLoad(res.rawfilePackageName);
        }
        async UniTask DownLoad(string package)
        {
            var res = GameEnter.Resource;

            var handle = res.UpdatePrePackageVersionAsync(package);
            await handle;

            var version = handle.PrePackageVersion;

            if(!string.IsNullOrEmpty(version))
            {
               var operation = res.PreDownloadContentAsync(package, version);

                await operation;
                if (operation.Status != EOperationStatus.Succeed)
                {
                    Log.Warning("更新错误");
                    return;
                }
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                downloader = operation.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

                if (downloader.TotalDownloadCount > 0)
                {
                    Log.Info($"Found total {downloader.TotalDownloadCount} files that need download ！");

                    // 发现新更新文件后，挂起流程系统
                    // 注意：开发者需要在下载前检测磁盘空间不足
                    int totalDownloadCount = downloader.TotalDownloadCount;
                    long totalDownloadBytes = downloader.TotalDownloadBytes;

                    float sizeMb = totalDownloadBytes / 1048576f;
                    sizeMb = Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                    string totalSizeMb = sizeMb.ToString("f1");

                    var form = GameEnter.UI.GetUIForm<UILoadForm>();
                    if (form != null)
                        form.ShowMessageBox(
                            $"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB", MessageShowType.TwoButton
                        , () => { StartDownFile(downloader); },
                        UnityEngine.Application.Quit);

                    Log.Info($"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB");
                }else
                {
                    ChangeState<ProcedureUpdateEnd>(_procedureOwner); 
                }
            }

        }
        ResourceDownloaderOperation downloader;
        void StartDownFile(ResourceDownloaderOperation downloader)
        {
            BeginDownload(downloader).Forget();
        }
        async UniTask BeginDownload(ResourceDownloaderOperation downloader)
        {
            var form = GameEnter.UI.GetUIForm<UILoadForm>();

            form.ShowUpdate("正在准备下载");
            // 注册下载回调
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;

            ChangeState<ProcedureUpdateEnd>(_procedureOwner);
        }

        private void OnDownloadErrorCallback(string fileName, string error)
        {
            var form = GameEnter.UI.GetUIForm<UILoadForm>();

            if (form != null)
            {
                form.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton
                 , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
            }

            //  UILoadTip.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton,
            //       LoadStyle.StyleEnum.Style_Default
            //     , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
        }
        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {

            string currentSizeMb = (currentDownloadBytes / 1048576f).ToString("f1");
            string totalSizeMb = (totalDownloadBytes / 1048576f).ToString("f1");

            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"{currentDownloadCount}/{totalDownloadCount} {currentSizeMb}MB/{totalSizeMb}MB");

            string descriptionText = Utility.Text.Format("正在更新，已更新{0}，总更新{1}，已更新大小{2}，总更新大小{3}，更新进度{4}，当前网速{5}/s",
                currentDownloadCount.ToString(),
                totalDownloadCount.ToString(),
                GameUtil.GetByteLengthString(currentDownloadBytes),
                GameUtil.GetByteLengthString(totalDownloadBytes),
              downloader.Progress,
               GameUtil.GetByteLengthString((int)downloader.CurrentSpeed));

            var form = GameEnter.UI.GetUIForm<UILoadForm>();

            form.ShowUpdate(descriptionText);

            form.OnUpdate(currentDownloadBytes * 1f / totalDownloadBytes);

            int needTime = 0;
            if (downloader.CurrentSpeed > 0)
            {
                needTime = (int)((totalDownloadBytes - currentDownloadBytes) / downloader.CurrentSpeed);
            }

            string updateProgress = Utility.Text.Format("剩余时间 {0}({1}/s)",
                new TimeSpan(0, 0, needTime).ToString(@"mm\:ss"), GameUtil.GetLengthString((int)downloader.CurrentSpeed));

            Log.Info(updateProgress);
        }
    }

    public class ProcedureUpdateEnd : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _procedureOwner = procedureOwner;
            OnUpdateEnd().Forget();
        }
        async UniTask OnUpdateEnd()
        {
            var res = GameEnter.Resource;

            Log.Info("保存版本文件");
            res.SaveVersion(res.assetPackageName);
            res.SaveVersion(res.rawfilePackageName);

            Log.Info("清理未使用的缓存文件！");

            await res.ClearUnusedCacheFilesAsync(res.assetPackageName);
            await res.ClearUnusedCacheFilesAsync(res.rawfilePackageName);

            res.assetPackage.package.ForceUnloadAllAssets();
            res.rawfilePackage.package.ForceUnloadAllAssets();

            await res.InitGameAsset();

            //app版本水印改变后会删除manifest文件夹

            //刷新文本
            var form = GameEnter.UI.GetUIForm<UILoadForm>();
            if (form != null)
                form.ReLoadTmp();


            ChangeState<ProcedurePreload>(_procedureOwner);
        }
    }

}