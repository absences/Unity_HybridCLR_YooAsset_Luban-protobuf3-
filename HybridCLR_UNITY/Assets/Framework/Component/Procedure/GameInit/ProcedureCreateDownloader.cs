using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameMain;
using System;
using UnityEngine;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework;

namespace GameInit
{
    public class ProcedureCreateDownloader : ProcedureBase
    {
        private readonly ProcedureOwner _procedureOwner;    
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("创建补丁下载器");

            CreateDownloader(procedureOwner).Forget();
        }
        async UniTask CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            var resource = GameEnter.Resource;

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            downloader = resource.CreateResourceDownloader(GameEnter.Resource.assetPackageName, downloadingMaxNum, failedTryAgain);

            var d1 = resource.CreateResourceDownloader(GameEnter.Resource.rawfilePackageName, downloadingMaxNum, failedTryAgain);

            downloader.Combine(d1);
             
            if (downloader.TotalDownloadCount == 0)
            {
                Log.Info("Not found any download files !");
                ChangeState<ProcedureDownloadOver>(procedureOwner);
            }
            else
            {
                //A total of 10 files were found that need to be downloaded
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
                    , () => { StartDownFile(); },
                    UnityEngine.Application.Quit);

                Log.Info($"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB");

                //StartDownFile(procedureOwner);
            }
        }
        ResourceDownloaderOperation downloader;
        void StartDownFile()
        {
            BeginDownload().Forget();
        }
        async UniTask BeginDownload()
        {
            form = GameEnter.UI.GetUIForm<UILoadForm>();

            form.ShowUpdate("正在准备下载");
            // 注册下载回调
            downloader.OnDownloadErrorCallback = OnDownloadErrorCallback;
            downloader.OnDownloadProgressCallback = OnDownloadProgressCallback;
            downloader.BeginDownload();
            await downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                return;

            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }
        private UILoadForm form;
        private void OnDownloadErrorCallback(string fileName, string error)
        {
            if (form != null)
            {
                form.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton
                 , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
            }

        }
        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {

           // string currentSizeMb = (currentDownloadBytes / 1048576f).ToString("f1");
            //string totalSizeMb = (totalDownloadBytes / 1048576f).ToString("f1");

            string descriptionText = Utility.Text.Format("正在更新，已更新{0}，总更新{1}，已更新大小{2}，总更新大小{3}，更新进度{4}，当前网速{5}/s",
                currentDownloadCount.ToString(),
                totalDownloadCount.ToString(),
                GameUtil.GetByteLengthString(currentDownloadBytes),
                GameUtil.GetByteLengthString(totalDownloadBytes),
              downloader.Progress,
               GameUtil.GetByteLengthString((int)downloader.CurrentSpeed));


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
}