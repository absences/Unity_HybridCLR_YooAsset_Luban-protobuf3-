using UnityGameFramework.Runtime;
using UnityEngine.UI;
using TMPro;
namespace HotfixMain
{
    internal class TestForm : UIFormLogic
    {
        #region UI Members
        private Image mImgImage;
        private Button mBtnButton;
        private TextMeshProUGUI mTxtinfo;
        private Image mImgImage2;
        #endregion

        protected override void OnInit(object userData)
        {
            mImgImage = getComponent<Image>(0);
            mBtnButton = getComponent<Button>(1);
            mTxtinfo = getComponent<TextMeshProUGUI>(2);
            mImgImage2 = getComponent<Image>(3);
        }
        protected override void OnOpen(object userData)
        {
            mBtnButton.onClick.AddListener(() =>
            {
                Close();
            });
            SetSprite(mImgImage, "chat_ico_emoji_5");
            SetSprite(mImgImage2, "main_campaign_bg_6");

            mTxtinfo.text = "<sprite=\"emoji one\" index=1>";
        }
    }
}
