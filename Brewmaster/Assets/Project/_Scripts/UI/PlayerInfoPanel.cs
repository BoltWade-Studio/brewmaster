using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using NOOD;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class PlayerInfoPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _playerAddressText;
		[SerializeField] private TextMeshProUGUI _treasuryText;
		[SerializeField] private TextMeshProUGUI _pointText;
		[SerializeField] private Button _claimBtn, _logOutBtn, _shareToTwitterBtn;
		[SerializeField] private GameObject _blockXImage;

#if UNITY_EDITOR
		[ContextMenu("Reserialize")]
		public void Reserialize()
		{
			var enumerable = new[] { "Assets/Project/_Prefab/UI/PlayerInfoPanel.prefab" }.AsEnumerable();
			AssetDatabase.ForceReserializeAssets(enumerable);
		}
#endif

		void OnEnable()
		{
			_claimBtn.onClick.AddListener(OnClaimBtnClick);
			_logOutBtn.onClick.AddListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.AddListener(OnShareToTwitterBtnClick);
			GameEvent.Instance.OnEndDay += OnEndDayHandler;
			GameEvent.Instance.OnNextDay += OnNextDayHandler;
			ActiveXButton(!ConnectWalletManager.Instance.IsAnonymous);
		}

		void OnDisable()
		{
			_claimBtn.onClick.RemoveListener(OnClaimBtnClick);
			_logOutBtn.onClick.RemoveListener(OnLogOutBtnClick);
			_shareToTwitterBtn.onClick.RemoveListener(OnShareToTwitterBtnClick);
		}
		void Start()
		{
			UpdateUI();
			_shareToTwitterBtn.interactable = false;
			GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler;
		}

		private void OnLoadDataSuccessHandler()
		{
			UpdateUI();
		}

		private void OnEndDayHandler()
		{
			_shareToTwitterBtn.interactable = true;
		}

		private void OnNextDayHandler()
		{
			_shareToTwitterBtn.interactable = false;
		}

		public void UpdateUI()
		{
			UpdateClaimBtn();
			UpdatePoint();
		}

		private void UpdatePoint()
		{
			_treasuryText.text = PlayerData.PlayerTreasury.ToString();
			_playerAddressText.text = PlayerData.PlayerAddress;
			_pointText.text = PlayerData.PlayerPoint.ToString();
		}

		private void UpdateClaimBtn()
		{
			_claimBtn.interactable = !ConnectWalletManager.Instance.IsAnonymous;
			_logOutBtn.interactable = !ConnectWalletManager.Instance.IsAnonymous;
		}
		private void OnLogOutBtnClick()
		{
		}
		private async void OnClaimBtnClick()
		{
			await TransactionManager.Instance.Claim();
		}
		private void OnShareToTwitterBtnClick()
		{
			TwitterShareManager.Instance.OpenTwitterNewTab();
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
