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

        info.text = "���Ʋ�����Ϸ���ܾ�������Ϸ��ע�����ұ�����������ƭ�ϵ���\r\n�ʶ���Ϸ���ԣ�������Ϸ����������ʱ�䣬���ܽ������";

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
    /// ��ʾ��ʾ��Ŀǰ���֧��������ť
    /// </summary>
    /// <param name="desc">����</param>
    /// <param name="showtype">���ͣ�MessageShowType��</param>
    /// <param name="style">StyleEnum</param>
    /// <param name="onOk">����¼�</param>
    /// <param name="onCancel">ȡ���¼�</param>
    /// <param name="onPackage">�����¼�</param>
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
