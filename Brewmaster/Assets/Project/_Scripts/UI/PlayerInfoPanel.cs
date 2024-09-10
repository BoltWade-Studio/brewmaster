using System;
using DG.Tweening;
using Newtonsoft.Json;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game
{
	public class PlayerInfoPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _playerAddressText, _ingamePointText, _sahPointText;
		[SerializeField] private Button _claimBtn, _logOutBtn, _shareToTwitterBtn;
		[SerializeField] private GameObject _blockXImage;

		void OnEnable()
		{
			_claimBtn.onClick.AddListener(OnClaimBtnClick);
			_logOutBtn.onClick.AddListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.AddListener(OnShareToTwitterBtnClick);
			GameplayManager.Instance.OnEndDay += GameplayManager_OnEndDay;
			GameplayManager.Instance.OnNextDay += GameplayManager_OnNextDay;
			ActiveXButton(!ConnectWalletManager.Instance.isAnonymous);
		}

		void OnDisable()
		{
			_claimBtn.onClick.RemoveListener(OnClaimBtnClick);
			_logOutBtn.onClick.RemoveListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.RemoveListener(OnShareToTwitterBtnClick);
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
		}
		void Start()
		{
			UpdateUI();
			_shareToTwitterBtn.interactable = false;
		}

		void Update()
		{
			if (!_ingamePointText.text.Equals(PlayerData.PlayerTreasury.ToString()))
				_ingamePointText.text = PlayerData.PlayerTreasury.ToString();
			if (!_playerAddressText.text.Equals(PlayerData.PlayerAddress))
				_playerAddressText.text = PlayerData.PlayerAddress;
		}

		private void GameplayManager_OnEndDay()
		{
			_shareToTwitterBtn.interactable = true;
		}

		private void GameplayManager_OnNextDay()
		{
			_shareToTwitterBtn.interactable = false;
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
		private void OnLogOutBtnClick()
		{
		}
		private void OnClaimBtnClick()
		{
			ConnectWalletManager.Instance.Claim();
		}
		private void OnShareToTwitterBtnClick()
		{
			TwitterShareManager.Instance.ShareToTwitter();
		}

		private void ActiveXButton(bool isActive)
		{
			_shareToTwitterBtn.interactable = isActive;
			_blockXImage.SetActive(!isActive);
		}

		public void Open()
		{
			this.gameObject.SetActive(true);
			this.transform.DOScale(1, 0.25f).SetEase(Ease.OutFlash);
		}
		public void Close()
		{
			this.transform.DOScale(0, 0.5f).SetEase(Ease.InFlash).onComplete = () => this.gameObject.SetActive(false);
		}
	}
}
