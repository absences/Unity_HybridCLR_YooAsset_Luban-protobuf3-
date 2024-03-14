﻿using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameInit
{
    public class ProcedureUpdateVersion : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;


        protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            _procedureOwner = procedureOwner;
            Log.Info("start update PackageVersion");

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Warning("The device is not connected to the network");
            }
            else
            {
                GetStaticVersion().Forget();
            }
        }
        async UniTask GetStaticVersion()
        {
            var resource = GameEnter.Resource;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            
            var operation = resource.UpdatePackageVersionAsync(GameEnter.Resource.rawfilePackageName);

            await operation;

            if (operation.Status == EOperationStatus.Succeed)
            {
                resource.rawfilePackage.packageVersion = operation.PackageVersion;
               // Log.Info("rawfile newversion")
                operation = resource.UpdatePackageVersionAsync(GameEnter.Resource.assetPackageName);

                await operation;
                if (operation.Status == EOperationStatus.Succeed)
                {
                    resource.assetPackage.packageVersion = operation.PackageVersion;

                    ChangeState<ProcedureDelivery>(_procedureOwner);
                }
                else
                {
                    Log.Error(operation.Error);

                    //UILoadTip.ShowMessageBox($"用户尝试更新静态版本失败！点击确认重试 \n \n <color=#FF0000>原因{operation.Error}</color>", MessageShowType.TwoButton,
                    //   LoadStyle.StyleEnum.Style_Retry
                    //    , () => { ChangeState<ProcedureUpdateVersion>(_procedureOwner); }, UnityEngine.Application.Quit);
                }
            }
            else
            {
                Log.Error(operation.Error);

                //UILoadTip.ShowMessageBox($"用户尝试更新静态版本失败！点击确认重试 \n \n <color=#FF0000>原因{operation.Error}</color>", MessageShowType.TwoButton,
                //   LoadStyle.StyleEnum.Style_Retry
                //    , () => { ChangeState<ProcedureUpdateVersion>(_procedureOwner); }, UnityEngine.Application.Quit);
            }
        }
    }
}