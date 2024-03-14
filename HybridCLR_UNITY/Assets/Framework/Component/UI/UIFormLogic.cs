using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 界面逻辑基类。
    /// </summary>
    public abstract class UIFormLogic
    {
        private bool m_Available = false;
        private bool m_Visible = false;
        private UIForm m_UIForm = null;
        private UIInterface m_UIInterface = null;
        private Transform m_CachedTransform = null;
        private int m_OriginalLayer = 0;

        private List<HandleBase> assetHandles;
        private Dictionary<Object, ViewRefData> viewRefs;
        /// <summary>
        /// 获取界面。
        /// </summary>
        public UIForm UIForm
        {
            get
            {
                return m_UIForm;
            }
        }

        /// <summary>
        /// 获取或设置界面名称。
        /// </summary>
        public string Name
        {
            get
            {
                return UIForm.gameObject.name;
            }
            set
            {
                UIForm.gameObject.name = value;
            }
        }

        /// <summary>
        /// 获取界面是否可用。
        /// </summary>
        public bool Available
        {
            get
            {
                return m_Available;
            }
        }

        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Available && m_Visible;
            }
            set
            {
                if (!m_Available)
                {
                    Log.Warning("UI form '{0}' is not available.", Name);
                    return;
                }

                if (m_Visible == value)
                {
                    return;
                }

                m_Visible = value;
                InternalSetVisible(value);
            }
        }

        /// <summary>
        /// 获取已缓存的 Transform。
        /// </summary>
        public Transform CachedTransform
        {
            get
            {
                return m_CachedTransform;
            }
        }
        /// <summary>
        /// 界面初始化。uiform调用
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        protected internal void onInit(UIForm form, object userData)
        {
            if (m_CachedTransform == null)
            {
                m_CachedTransform = form.transform;
            }
            m_UIForm = form;
            m_UIInterface = form.GetComponent<UIInterface>();
            m_OriginalLayer = form.gameObject.layer;
            assetHandles = ObjectPool.AcquireList<HandleBase>();
            viewRefs = ObjectPool.AcquireDictionary<Object, ViewRefData>();
            OnInit(userData);
        }
        /// <summary>
        /// 界面初始化 子类重写
        /// </summary>
        /// <param name="userData"></param>
        protected internal virtual void OnInit(object userData)
        {

        }
        /// <summary>
        /// 界面回收。
        /// </summary>
        protected internal virtual void OnRecycle()
        {
        }

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        protected internal void onOpen(object userData)
        {
            m_Available = true;
            Visible = true;
            OnOpen(userData);
        }
        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        protected internal virtual void OnOpen(object userData)
        {

        }
        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        protected internal void onClose(bool isShutdown, object userData)
        {
            UIForm.gameObject.SetLayerRecursively(m_OriginalLayer);
            Visible = false;
            m_Available = false;
            OnClose(isShutdown, userData);

            foreach (var handle in assetHandles)
            {
                switch (handle)
                {
                    case AssetHandle assetHandle:
                        assetHandle.Release();
                        break;
                    case RawFileHandle rawFileHandle:
                        rawFileHandle.Release();
                        break;
                    default:
                        throw new GameFrameworkException(Utility.Text.Format("asset handle  '{0}' can't release", handle));

                }
            }
            ObjectPool.Release(assetHandles);
            assetHandles = null;

            foreach (var item in viewRefs.Values)
            {
                ReferencePool.Release(item);    
            }
            viewRefs = null;
        }

        /// <summary>
        /// 界面关闭。 
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        protected internal virtual void OnClose(bool isShutdown, object userData)
        {

        }
        /// <summary>
        /// 界面暂停。
        /// </summary>
        protected internal virtual void OnPause()
        {
            Visible = false;
        }

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        protected internal virtual void OnResume()
        {
            Visible = true;
        }

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        protected internal virtual void OnCover()
        {
        }

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        protected internal virtual void OnReveal()
        {
        }

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        protected internal virtual void OnRefocus(object userData)
        {
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        protected internal virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
        }

        /// <summary>
        /// 设置界面的可见性。
        /// </summary>
        /// <param name="visible">界面的可见性。</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            UIForm.gameObject.SetActive(visible);
        }

        #region Method

        public T getComponent<T>(int id) where T : Component
        {
            return m_UIInterface. getComponent<T>(id);
        }

        public GameObject getGameobject(int id)
        {
            return m_UIInterface.getGameobject(id);
        }

        public void Close()
        {
            GameEnter.UI.CloseUIForm(UIForm);
        }


        void AddImageRef(Image image, string data)
        {
            if (!viewRefs.TryGetValue(image, out var refs))
            {
                refs = ViewRefData.Create();
                viewRefs.Add(image, refs);
            }
           
            refs.imagelocation = data;
            

        }
        bool CheckImgRef(Image image, string location)
        {
            if (viewRefs.TryGetValue(image, out var refs))
            {
                return refs.imagelocation == location;
            }
            return false;
        }

        protected void SetSprite(Image image, string location, System.Action<Sprite> onSet = null)
        {
            if (!m_Available)
                return;

            AddImageRef(image, location);
            GameEnter.Resource.LoadAssetAsync<Sprite>(location).Completed +=
                (handle) =>
                {
                    if (handle.Status == EOperationStatus.Succeed)
                    {
                        if (m_Available && CheckImgRef(image, location))//界面可用 且引用相同
                        {
                            var sprite = handle.GetAssetObject<Sprite>();
                            assetHandles.Add(handle);
                            image.sprite = sprite;
                            onSet?.Invoke(sprite);
                        }
                        else
                        {
                            Log.Warning(this + " is not available");
                            handle.Release();
                        }
                    }

                };
        }

        #endregion
    }
}
