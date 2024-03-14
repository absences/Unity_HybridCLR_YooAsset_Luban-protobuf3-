using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public class UILoadUpdate :MonoBehaviour
    {
        public Button _btn_clear;
        public Scrollbar _obj_progress;
        public TextMeshProUGUI _label_desc;
        public TextMeshProUGUI _label_appid;
        public TextMeshProUGUI _label_resid;

        private GameObject _gameObject;
        void Start()
        {
            _gameObject = transform.Find("UILoadUpdate").gameObject;
            _btn_clear.onClick.AddListener(OnClear);
            _btn_clear.gameObject.SetActive(true);
        }


        public  void OnEnter(string param)
        {
            _label_desc.text = param;

            _gameObject.SetActive(true);

            _label_appid.text = string.Format("{0}", Application.version);
            _label_resid.text = string.Format("asset:{0} raw:{1}",
                GameEnter.Resource.assetPackage.packageVersion,GameEnter.Resource.rawfilePackage.packageVersion);
            RefreshVersion();
        }

        public void OnUpdate(float progress)
        {
            _obj_progress.size = progress;
        }

        void Update()
        {
        }

        private void RefreshVersion()
        {
            _label_appid.text = string.Format("{0}", Application.version);
            _label_resid.text = string.Format("asset:{0} raw:{1}",
                GameEnter.Resource.assetPackage.packageVersion, GameEnter.Resource.rawfilePackage.packageVersion);
        }

        public void OnContinue(GameObject obj)
        {
            // LoadMgr.Instance.StartDownLoad();
        }

        public void OnEnd()
        {
            // _gameObject.SetActive(false);
            RefreshVersion();
        }

        /// <summary>
        /// 清空本地缓存
        /// </summary>
        /// <param name="obj"></param>
        public void OnClear()
        {
            //OnStop(null);

            GetComponent<UILoadForm>().ShowMessageBox("清理缓存？", MessageShowType.TwoButton,
                () =>
                {
                    GameEnter.Resource.ClearSandbox();

                    Application.Quit();
                }, () => { OnContinue(null); });
        }

        /// <summary>
        /// 下载进度完成
        /// </summary>
        /// <param name="type"></param>
        public void DownLoad_Complete_Action(int type)
        {
            Log.Info("DownLoad_Complete");
        }

        /// <summary>
        /// 下载进度更新
        /// </summary>
        /// <param name="progress"></param>
        public void DownLoad_Progress_Action(float progress)
        {
            _obj_progress.gameObject.SetActive(true);

            _obj_progress.size = progress;
        }

        ///// <summary>
        ///// 解压缩完成回调
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="status"></param>
        //public void Unpacked_Complete_Action(bool type, GameStatus status)
        //{
        //    _obj_progress.gameObject.SetActive(true);
        //    _label_desc.text = LoadText.Instance.Label_Load_UnpackComplete;
        //    if (status == GameStatus.AssetLoad)
        //    {
        //    }
        //    else
        //    {
        //        Log.Error("error type");
        //    }
        //}

        ///// <summary>
        ///// 解压缩进度更新
        ///// </summary>
        ///// <param name="progress"></param>
        ///// <param name="status"></param>
        //public virtual void Unpacked_Progress_Action(float progress, GameStatus status)
        //{
        //    _obj_progress.gameObject.SetActive(true);
        //    if (status == GameStatus.First)
        //    {
        //        _label_desc.text = LoadText.Instance.Label_Load_FirstUnpack;
        //    }
        //    else
        //    {
        //        _label_desc.text = LoadText.Instance.Label_Load_Unpacking;
        //    }

        //    _obj_progress.size = progress;
        //}

    }
}