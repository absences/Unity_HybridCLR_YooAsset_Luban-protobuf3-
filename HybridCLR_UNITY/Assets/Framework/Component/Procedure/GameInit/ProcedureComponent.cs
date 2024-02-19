using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameInit
{
    //游戏初始化流程组件
    public class ProcedureComponent : BaseGameComponent
    {
        private IProcedureManager m_ProcedureManager = null;

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (m_ProcedureManager == null)
                {
                    return null;
                }
                return m_ProcedureManager.CurrentProcedure;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_ProcedureManager = new ProcedureManager();
        }

        // private string[] m_AvailableProcedureTypeNames = null;
        public Type[] procedureTypes = new Type[]
        {
           typeof(ProcedureInitPackage),

           typeof(ProcedurePreload),

           typeof(ProcedureInitResources),

           typeof(ProcedureUpdateVersion),
           typeof(ProcedureDelivery),
           typeof(ProcedureUpdateManifest),
           typeof(ProcedureCreateDownloader),
           typeof(ProcedureDownloadFile),
           typeof(ProcedureDownloadOver),
           typeof(ProcedureClearCache),
        };

        [SerializeField]
        private string m_EntranceProcedureTypeName = null;

        private ProcedureBase m_EntranceProcedure = null;

        void Start()
        {
            ProcedureBase[] procedures = new ProcedureBase[procedureTypes.Length];
            for (int i = 0; i < procedureTypes.Length; i++)
            {
                procedures[i] = Activator.CreateInstance(procedureTypes[i]) as ProcedureBase;
            
                if (m_EntranceProcedureTypeName == procedures[i].GetType().Name)
                {
                    m_EntranceProcedure = procedures[i];
                }
            }
            m_ProcedureManager.Initialize(GameEnter.FSM.FsmManager, procedures);

            UniTask.Create(async () =>
            {

                await UniTask.WaitForEndOfFrame(this);

                //初始化游戏入口
                m_ProcedureManager.StartProcedure(m_EntranceProcedure.GetType());
            });
        }
    }
}