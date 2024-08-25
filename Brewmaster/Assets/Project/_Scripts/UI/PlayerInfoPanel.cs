using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PlayerInfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerAddressText, _ingamePointText, _sahPointText;
        [SerializeField] private Button _claimBtn, _logOutBtn;

        void OnEnable()
        {
            _claimBtn.onClick.AddListener(OnClaimBtnClick);
            _logOutBtn.onClick.AddListener(OnLogOutBtnClick);
        }
        void OnDisable()
        {
            _claimBtn.onClick.RemoveListener(OnClaimBtnClick);
            _logOutBtn.onClick.RemoveListener(OnLogOutBtnClick);
        }
        void Start()
        {
            UpdateUI();
        }

        void Update()
        {
            if(!_ingamePointText.text.Equals(PlayerData.InGamePoint.ToString()))
                _ingamePointText.text = PlayerData.InGamePoint.ToString();
            if(!_sahPointText.text.Equals(PlayerData.SahPoint.ToString()))
                _sahPointText.text = PlayerData.SahPoint.ToString();
            if(!_playerAddressText.text.Equals(PlayerData.PlayerAddress))
            _playerAddressText.text = PlayerData.PlayerAddress;
        }

        public void UpdateUI()
        {
            UpdateClaimBtn();
        }

        private void UpdateClaimBtn()
        {
            _claimBtn.interactable = !ConnectWalletManager.Instance.isAnonymous;
            _logOutBtn.interactable = !ConnectWalletManager.Instance.isAnonymous;
        }
        private void UpdatePlayerAddress()
        {
            _playerAddressText.text = PlayerData.PlayerAddress;
        }
        private void OnLogOutBtnClick()
        {
        }
        private void OnClaimBtnClick()
        {
            ConnectWalletManager.Instance.Claim();
        }

        public void Open()
        {
            this.transform.DOScale(1, 0.25f).SetEase(Ease.OutFlash);
        }
        public void Close()
        {
            this.transform.DOScale(0, 0.5f).SetEase(Ease.InFlash).onComplete = () => this.gameObject.SetActive(false);
        }
    }
}
