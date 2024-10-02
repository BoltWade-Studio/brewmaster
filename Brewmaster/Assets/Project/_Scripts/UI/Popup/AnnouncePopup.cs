using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public class AnnouncePopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText, _messageText, _btnText;
        [SerializeField] private CustomButton _btn;

        public void Show(string title, string message, string btnText, Action btnAction)
        {
            _titleText.text = title;
            _messageText.text = message;
            _btnText.text = btnText;
            _btn.OnClick = btnAction;
            _btn.OnClick += Hide;

            this.gameObject.SetActive(true);
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Flash);
        }

        private void Hide()
        {
            this.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InExpo).OnComplete(() => this.gameObject.SetActive(false));
        }
    }
}
