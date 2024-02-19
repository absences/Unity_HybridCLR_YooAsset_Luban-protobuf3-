using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
namespace GameInit
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("下载完成!!!");

            ChangeState<ProcedureClearCache>(procedureOwner);
        }
    }
    public class ProcedureClearCache : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        protected internal override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            Log.Info("清理未使用的缓存文件！");
            _procedureOwner = procedureOwner;
            ClearCache().Forget();
        }
        async UniTask ClearCache()
        {
            var resource = GameEnter.Resource;


            await resource.ClearUnusedCacheFilesAsync();

            ChangeState<ProcedurePreload>(_procedureOwner);
        }
    }

}