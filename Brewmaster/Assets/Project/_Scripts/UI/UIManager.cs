using System;
using Cysharp.Threading.Tasks;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyTransition;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using Utils;

namespace Game
{
	public class UIManager : MonoBehaviorInstance<UIManager>
	{
		#region Events
		public Action OnNextDayPressed;
		#endregion

		[Header("In game menu")]
		[SerializeField] private GameObject _ingameMenu;
		[SerializeField] private TextMeshProUGUI _moneyText;
		[SerializeField] private TextMeshProUGUI _timeText;
		[SerializeField] private Image _timeBG;
		[SerializeField] private TextMeshProUGUI _dayText;
		[SerializeField] private CustomButton _pauseGameBtn, _playerInfo;
		[SerializeField] private PlayerInfoPanel _playerInfoPanel;
		private Color _timeOriginalColor;

		[Header("EndDay")]
		[SerializeField] private EndDayPanel _endDayPanel;

		[Header("Pause game menu")]
		[SerializeField] private GameObject _pauseGameMenu;
		[SerializeField] private TextMeshProUGUI _pMoneyText, _pDayText;
		[SerializeField] private CustomButton _pResumeBtn, _pToMainMenuBtn;

		[Header("Store")]
		[SerializeField] private GameObject _storeMenu;
		[FormerlySerializedAs("_confirmButton")]
		[SerializeField] private CustomButton _storeConfirmBtn;
		[SerializeField] private CustomButton _storeUpBtn, _storeDownBtn;

		[Header("Twitter")]
		[SerializeField] private TwitterInputPanel _twitterInputPanel;

		[Header("Other")]
		public TransitionSettings TransitionSetting;
		private bool _isStorePhrase;

		#region Unity functions
		protected override void ChildAwake()
		{
			_pauseGameMenu.SetActive(false);
			_playerInfo.OnClick += OnPlayerInfoClickHandler;
			_pauseGameBtn.OnClick += OnPauseButtonClickHandler;
			_pResumeBtn.OnClick += OnResumeClickHandler;
			_pToMainMenuBtn.OnClick += OnMainMenuClickHandler;
			_storeConfirmBtn.OnClick += () =>
			{
				// GameEvent.Instance.OnNextDay?.Invoke();
				HideStoreMenu();
				GameEvent.Instance.OnStorePhaseEnd?.Invoke();
				_endDayPanel.Show(false);
				_isStorePhrase = false;
			};
			_storeUpBtn.OnClick += OnStoreMoveUp;
			_storeDownBtn.OnClick += OnStoreMoveDown;

			HideStoreMenu();
			_timeOriginalColor = _timeBG.color;
			_twitterInputPanel.Hide();
		}

		private void Start()
		{
			UpdateDayText();
			UpdateMoneyUI();
			_timeBG.color = _timeOriginalColor;
			TimeManager.Instance.OnTimeWarning += WarningTimeBg;
			TimeManager.OnTimePause += ShowPauseGameMenu;
			TimeManager.OnTimeResume += HidePauseGameMenu;
			GameEvent.Instance.OnNextDay += ResetTimeBg;
			GameEvent.Instance.OnEndDay += OnEndDayHandler;
			GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler;
			GameEvent.Instance.OnStorePhase += OnStorePaseHandler;
		}

		private void Update()
		{
			if (GameplayManager.Instance.IsEndDay) return;
			UpdateTime();
		}
		private void OnDestroy()
		{
			TimeManager.Instance.OnTimeWarning -= WarningTimeBg;
			TimeManager.OnTimePause -= ShowPauseGameMenu;
			TimeManager.OnTimeResume -= HidePauseGameMenu;
			GameEvent.Instance.OnNextDay -= ResetTimeBg;
			GameEvent.Instance.OnEndDay -= OnEndDayHandler;
			GameEvent.Instance.OnLoadDataSuccess -= OnLoadDataSuccessHandler;
			GameEvent.Instance.OnStorePhase -= OnStorePaseHandler;
		}
		#endregion

		#region Event functions
		private void OnStorePaseHandler()
		{
			ShowStoreMenu();
			_isStorePhrase = true;
			UpdateMoneyUI();
		}
		#endregion

		#region In game
		private void OnPlayerInfoClickHandler()
		{
			if (_playerInfoPanel.isActiveAndEnabled)
			{
				_playerInfoPanel.Close();
			}
			else
			{
				_playerInfoPanel.gameObject.SetActive(true);
				_playerInfoPanel.Open();
			}
		}
		public void UpdatePlayerInfoPanel()
		{
			_playerInfoPanel.UpdateUI();
		}
		public void UpdateDayText()
		{
			_dayText.text = "Day".GetText() + " " + TimeManager.Instance.CurrentDay.ToString("00");
		}
		public void UpdateMoneyUI()
		{
			if (_isStorePhrase)
				_moneyText.text = PlayerData.PlayerTreasury.ToString();
			else
				_moneyText.text = PlayerData.InDayTreasury.ToString();
		}
		public void UpdateTime()
		{
			float hour = TimeManager.Instance.GetHour();
			float minute = TimeManager.Instance.GetMinute();

			_timeText.text = $"{hour.ToString("00")}:{minute.ToString("00")}";
		}
		private void HideIngameMenu()
		{
			_ingameMenu.SetActive(false);
		}
		private void ShowIngameMenu()
		{
			_ingameMenu.SetActive(true);
		}
		#endregion

		#region EndDay
		public void ShowTwitterInputPanel()
		{
			_endDayPanel.ShowTwitterInputPanel();
		}
		public void OnEndDayHandler()
		{
			_endDayPanel.Show(true);
		}

		private void OnLoadDataSuccessHandler()
		{
			UpdateMoneyUI();
		}
		#endregion

		#region Store Menu
		private async void ShowStoreMenu()
		{
			Utility.Socket.EmitEvent(SocketEnum.getPlayerPub.ToString());
			LoadingUIManager.Instance.ChangeLoadingMessage("Getting player money");
			await UniTask.WaitForSeconds(1f);
			_storeMenu.SetActive(true);
		}
		private void HideStoreMenu()
		{
			_storeMenu.SetActive(false);
		}

		private void OnStoreMoveUp()
		{
			string json = JsonConvert.SerializeObject(new ArrayWrapper
			{ array = new string[] { JsonUtility.ToJson(new Vector2(0, 1)) } });
			Utility.Socket.EmitEvent("playerForceMove", json);
		}

		private void OnStoreMoveDown()
		{
			string json = JsonConvert.SerializeObject(new ArrayWrapper
			{ array = new string[] { JsonUtility.ToJson(new Vector2(0, -1)) } });
			Utility.Socket.EmitEvent("playerForceMove", json);
		}
		#endregion

		#region PauseGame
		private void ShowPauseGameMenu()
		{
			_pauseGameMenu.SetActive(true);
			HideIngameMenu();
			HideStoreMenu();
			UpdatePauseText();
		}
		private void HidePauseGameMenu()
		{
			_pauseGameMenu.SetActive(false);
			ShowIngameMenu();
			if (_isStorePhrase)
				ShowStoreMenu();
		}
		private void UpdatePauseText()
		{
			_pDayText.text = TimeManager.Instance.CurrentDay.ToString("00");
			_pMoneyText.text = PlayerData.PlayerPoint.ToString("0");
		}
		#endregion

		#region ButtonZone
		private void OnPauseButtonClickHandler()
		{
			GameplayManager.Instance.OnPausePressed?.Invoke();
		}
		public void OnMainMenuClickHandler()
		{
			TransitionManager.Instance().Transition("MainMenu", TransitionSetting, 0);
		}
		private void OnResumeClickHandler()
		{
			HidePauseGameMenu();
			GameplayManager.Instance.OnPausePressed?.Invoke();
		}
		#endregion

		#region Support
		private void WarningTimeBg()
		{
			_timeBG.color = Color.red;
		}
		private void ResetTimeBg()
		{
			_timeBG.color = _timeOriginalColor;
		}
		#endregion
	}
}
