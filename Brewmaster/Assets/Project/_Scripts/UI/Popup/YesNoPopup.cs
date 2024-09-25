using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class YesNoPopup : MonoBehaviour
    {
        [SerializeField] private CustomButton _yesBtn, _noBtn;
        [SerializeField] private TextMeshProUGUI _titleText, _messageText;

        public void Show(string title, string message, Action yesAction, Action noAction)
        {
            this.gameObject.SetActive(true);
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Flash);

            _titleText.text = title;
            _messageText.text = message;

            _yesBtn.OnClick = yesAction;
            _noBtn.OnClick = noAction;
            _yesBtn.OnClick += Hide;
            _noBtn.OnClick += Hide;
        }

        private void Hide()
        {
            this.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InExpo).OnComplete(() => this.gameObject.SetActive(false));
        }
    }
}
