using System;
using DG.Tweening;
using Newtonsoft.Json;
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

		void OnEnable()
		{
			_claimBtn.onClick.AddListener(OnClaimBtnClick);
			_logOutBtn.onClick.AddListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.AddListener(OnShareToTwitterBtnClick);
			GameplayManager.Instance.OnEndDay += GameplayManager_OnEndDay;
			GameplayManager.Instance.OnNextDay += GameplayManager_OnNextDay;
		}

		void OnDisable()
		{
			_claimBtn.onClick.RemoveListener(OnClaimBtnClick);
			_logOutBtn.onClick.RemoveListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.RemoveListener(OnShareToTwitterBtnClick);
			GameplayManager.Instance.OnEndDay -= GameplayManager_OnEndDay;
			GameplayManager.Instance.OnNextDay -= GameplayManager_OnNextDay;
		}
		void Start()
		{
			UpdateUI();
			_shareToTwitterBtn.interactable = false;
		}

		void Update()
		{
			if (!_ingamePointText.text.Equals(PlayerData.TotalPoint.ToString()))
				_ingamePointText.text = PlayerData.TotalPoint.ToString();
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
		private void OnShareToTwitterBtnClick()
		{
			string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { TwitterMessage.GetTwitterMessage() } });
			JsSocketConnect.EmitEvent(SocketEnum.shareToTwitterRequest.ToString(), json);
			UIManager.Instance.ShowTwitterInputPanel();
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
