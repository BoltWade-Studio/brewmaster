using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ConnectWalletUI : MonoBehaviour
    {
        public Action onArgentXClick;
        public Action onBraavosClick;
        public Action onAnonymousClick;

        [SerializeField] private Button _argentXButton, _braavosButton, _anonymousBtn;

        public void Open()
        {
            _argentXButton.onClick.RemoveAllListeners();
            _braavosButton.onClick.RemoveAllListeners();
            _anonymousBtn.onClick.RemoveAllListeners();
            _argentXButton.onClick.AddListener(() => onArgentXClick?.Invoke());                
            _braavosButton.onClick.AddListener(() => onBraavosClick?.Invoke());
            _anonymousBtn.onClick.AddListener(() => onAnonymousClick?.Invoke());
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(1, 0.25f).SetEase(Ease.OutFlash);
        }
        public void Close()
        {
            this.transform.DOScale(0, 0.25f).SetEase(Ease.OutFlash);
            NoodyCustomCode.StartDelayFunction(() => Destroy(this.gameObject), 0.25f);
        }
    }
}
