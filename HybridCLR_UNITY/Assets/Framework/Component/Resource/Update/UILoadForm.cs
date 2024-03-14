using GameMain;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityGameFramework.Runtime;
using YooAsset;

public class UILoadForm : UIFormLogic
{

    private VideoPlayer videoPlayer;

    private TextMeshProUGUI info;
    protected internal override void OnInit(object userData)
    {
        base.OnInit(userData);

        videoPlayer = CachedTransform.Find("UIGame/vp").GetComponent<VideoPlayer>();
        info = CachedTransform.Find("UIGame/tip").GetComponent<TextMeshProUGUI>();
    }

    private RawFileHandle videoHandle;
    protected internal override void OnOpen(object userData)
    {
        base.OnOpen(userData);

        info.text = "抵制不良游戏，拒绝盗版游戏。注意自我保护，谨防受骗上当。\r\n适度游戏益脑，沉迷游戏伤身。合理安排时间，享受健康生活。";

        videoHandle = GameEnter.Resource.LoadRawFileSync("loading");
        videoPlayer.url = videoHandle.GetRawFilePath();
        videoPlayer.targetCamera = GameEnter.UI.UICamera;
        videoPlayer.isLooping = true;
        videoPlayer.Play();

    }
    protected internal override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);

        videoHandle.Release();
    }
    public void ReLoadTmp()
    {
        var tmps = CachedTransform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in tmps)
        {
            text.UpdateFontAsset();
            text.SetAllDirty();
        }
    }

    /// <summary>
    /// 显示提示框，目前最多支持三个按钮
    /// </summary>
    /// <param name="desc">描述</param>
    /// <param name="showtype">类型（MessageShowType）</param>
    /// <param name="style">StyleEnum</param>
    /// <param name="onOk">点击事件</param>
    /// <param name="onCancel">取消事件</param>
    /// <param name="onPackage">更新事件</param>
    public void ShowMessageBox(string desc, MessageShowType showtype = MessageShowType.OneButton,
        Action onOk = null,
        Action onCancel = null,
        Action onPackage = null)
    {

        var tip = CachedTransform.GetComponent<UILoadTip>();
        if (tip == null)
            return;
        tip.OnOk = onOk;
        tip.OnCancle = onCancel;
        tip.Showtype = showtype;

        tip.OnEnter(desc);
    }

    public void ShowUpdate(string s)
    {
        var tip = CachedTransform.GetComponent<UILoadUpdate>();
        if (tip == null)
            return;


        tip.OnEnter(s);
    }

    public void OnUpdate(float s)
    {
        var tip = CachedTransform.GetComponent<UILoadUpdate>();
        if (tip == null)
            return;


        tip.OnUpdate(s);
    }

    public void OnUpdateEnd()
    {
        var tip = CachedTransform.GetComponent<UILoadUpdate>();
        if (tip == null)
            return;

        tip.OnEnd();
    }
}
