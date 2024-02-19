﻿using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureCreateDownloader : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("创建补丁下载器");

            CreateDownloader(procedureOwner).Forget();
        }
        async UniTaskVoid CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            var resource = GameEnter.Resource;

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = resource.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

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

                // UILoadTip.ShowMessageBox($"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB", MessageShowType.TwoButton,
                //     LoadStyle.StyleEnum.Style_StartUpdate_Notice
                //    , () => { StartDownFile(procedureOwner: procedureOwner); }, UnityEngine.Application.Quit);

                Log.Info($"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB");

                StartDownFile(procedureOwner: procedureOwner);
            }
        }

        void StartDownFile(ProcedureOwner procedureOwner)
        {
            ChangeState<ProcedureDownloadFile>(procedureOwner);
        }
    }
}