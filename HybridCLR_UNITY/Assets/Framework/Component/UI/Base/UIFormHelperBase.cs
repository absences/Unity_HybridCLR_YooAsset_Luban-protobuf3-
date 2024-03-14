﻿using GameFramework.UI;
using UnityEngine;
using YooAsset;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 界面辅助器基类。
    /// </summary>
    public abstract class UIFormHelperBase : MonoBehaviour, IUIFormHelper
    {
        /// <summary>
        /// 实例化界面。
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源。</param>
        /// <returns>实例化后的界面。</returns>
        public abstract GameObject InstantiateUIForm(HandleBase uiFormAsset);

        /// <summary>
        /// 创建界面。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        public abstract IUIForm CreateUIForm(GameObject uiFormInstance, IUIGroup uiGroup, object userData);

        /// <summary>
        /// 释放界面。
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源。</param>
        /// <param name="uiFormInstance">要释放的界面实例。</param>
        public abstract void ReleaseUIForm(HandleBase uiFormAsset, object uiFormInstance);
    }
}
