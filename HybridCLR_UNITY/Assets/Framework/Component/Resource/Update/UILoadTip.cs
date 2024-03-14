using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace GameMain
{
    public enum MessageShowType
    {
        None = 0,
        OneButton = 1,
        TwoButton = 2,
        ThreeButton = 3,
    }

    public class UILoadTip : MonoBehaviour
    {
        public Button _btn_update;
        public Button _btn_ignore;
        public Button _btn_package;
        public TextMeshProUGUI _label_desc;

        public Action OnOk;
        public Action OnCancle;
        public MessageShowType Showtype = MessageShowType.None;


        private GameObject _gameObject;
        void Start()
        {
            _btn_update.onClick.AddListener(_OnGameUpdate);
            _btn_ignore.onClick.AddListener(_OnGameIgnor);
            _btn_package.onClick.AddListener(_OnInvoke);

            _gameObject = transform.Find("UILoadTip").gameObject;
        }

        public void OnEnter(string data)
        {
            _gameObject.SetActive(true);

            _btn_ignore.gameObject.SetActive(false);
            _btn_package.gameObject.SetActive(false);
            _btn_update.gameObject.SetActive(false);
            switch (Showtype)
            {
                case MessageShowType.OneButton:
                    _btn_update.gameObject.SetActive(true);
                    break;
                case MessageShowType.TwoButton:
                    _btn_update.gameObject.SetActive(true);
                    _btn_ignore.gameObject.SetActive(true);
                    break;
                case MessageShowType.ThreeButton:
                    _btn_ignore.gameObject.SetActive(true);
                    _btn_package.gameObject.SetActive(true);
                    _btn_package.gameObject.SetActive(true);
                    break;
            }

            _label_desc.text = data;
        }

        private void _OnGameUpdate()
        {
            if (OnOk == null)
            {
                _label_desc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                OnOk();
                _OnClose();
            }
        }

        private void _OnGameIgnor()
        {
            if (OnCancle == null)
            {
                _label_desc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                OnCancle();
                _OnClose();
            }
        }

        private void _OnInvoke()
        {
            if (OnOk == null)
            {
                _label_desc.text = "<color=#BA3026>该按钮不应该存在</color>";
            }
            else
            {
                OnOk();
                _OnClose();
            }
        }

        private void _OnClose()
        {
            _gameObject.SetActive(false);  
        }


    }
}