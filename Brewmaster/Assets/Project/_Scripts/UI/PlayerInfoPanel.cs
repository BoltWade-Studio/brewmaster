using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyTransition;
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
			UpdateUI();
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
			Utility.Socket.OnEvent(SocketEnum.logoutCallback.ToString(), this.gameObject.name, nameof(LogOutCallback), LogOutCallback);
		}

		private void OnLoadDataSuccessHandler()
		{
			UpdateUI();
		}

		public void UpdateUI()
		{
			UpdateClaimBtn();
			UpdatePoint();
		}

		private void UpdatePoint()
		{
			// Player info always get info from PlayerData because this will show player current info, not in day info
			_playerAddressText.text = PlayerData.PlayerAddress;
			_treasuryText.text = PlayerData.PlayerTreasury.ToString();
			_pointText.text = PlayerData.PlayerPoint.ToString();
		}

		private void UpdateClaimBtn()
		{
			_claimBtn.interactable = !ConnectWalletManager.Instance.IsAnonymous;
			_logOutBtn.interactable = !ConnectWalletManager.Instance.IsAnonymous;
		}
		private void OnLogOutBtnClick()
		{
			PopupManager.Instance.ShowYesNoPopup("Log out", "Are you sure you want to log out?", () =>
			{
				Utility.Socket.EmitEvent(SocketEnum.logout.ToString());
			}, null);
		}
		private async void OnClaimBtnClick()
		{
			await TransactionManager.Instance.Claim();
		}
		private void OnShareToTwitterBtnClick()
		{
			TwitterShareManager.Instance.OpenTwitterNewTab();
		}

		private async void LogOutCallback(string data)
		{
			Debug.Log("LogOutCallback: " + data);
			await UniTask.SwitchToMainThread();
			if (Application.isEditor == false)
				JSInteropManager.DisconnectWallet();
			TransitionManager.Instance().Transition("MainMenu", UIManager.Instance.TransitionSetting, 0);
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
