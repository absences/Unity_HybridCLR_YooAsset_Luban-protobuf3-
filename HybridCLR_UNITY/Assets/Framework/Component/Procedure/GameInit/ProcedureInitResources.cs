using GameFramework.Fsm;
using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameInit
{

    public class ProcedureInitResources : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            //包内资源模式
            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}