using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureDownloadFile : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            _procedureOwner = procedureOwner;
            Log.Info("开始下载更新文件！");
            BeginDownload().Forget();
        }
        async UniTask BeginDownload()
        {
            var downloader = GameEnter.Resource.Downloader;

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
        private void OnDownloadErrorCallback(string fileName, string error)
        {
            //  UILoadTip.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton,
            //       LoadStyle.StyleEnum.Style_Default
            //     , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
        }
        private void OnDownloadProgressCallback(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            var downloader = GameEnter.Resource.Downloader;

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

            // LoadUpdateLogic.Instance.DownProgressAction?.Invoke(downloader.Progress);

            // UILoadMgr.Show(UIDefine.UILoadUpdate, descriptionText);

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