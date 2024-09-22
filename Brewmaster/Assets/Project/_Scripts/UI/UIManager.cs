using System;
using Cysharp.Threading.Tasks;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.Serialization;

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
				_endDayPanel.Show(false);
				_isStorePhrase = false;
			};

			HideStoreMenu();
			_timeOriginalColor = _timeBG.color;
			_twitterInputPanel.Hide();
		}

		private void Start()
		{
			UpdateDayText();
			UpdateInDayMoney();
			_timeBG.color = _timeOriginalColor;
			TimeManager.Instance.OnTimeWarning += () =>
			{
				_timeBG.color = Color.red;
			};
			GameEvent.Instance.OnNextDay += () =>
			{
				_timeBG.color = _timeOriginalColor;
			};

			TimeManager.OnTimePause += ShowPauseGameMenu;
			TimeManager.OnTimeResume += HidePauseGameMenu;
			GameEvent.Instance.OnEndDay += OnEndDayHandler;
			GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler;
			GameEvent.Instance.OnStorePhase += OnStorePaseHandler;
		}


		void OnEnable()
		{
			RegisterEvent();
		}

		private void Update()
		{
			if (GameplayManager.Instance.IsEndDay) return;
			UpdateTime();
		}
		void OnDisable()
		{
			UnRegisterEvent();
		}
		private void OnDestroy()
		{
		}
		#endregion

		private void RegisterEvent()
		{
		}
		private void UnRegisterEvent()
		{
			NoodyCustomCode.UnSubscribeAllEvent(GameEvent.Instance, this);
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
			NoodyCustomCode.UnSubscribeAllEvent<TimeManager>(this);
			NoodyCustomCode.UnSubscribeFromStatic(typeof(TimeManager), this);
		}

		#region Event functions
		private void OnStorePaseHandler()
		{
			ShowStoreMenu();
			_isStorePhrase = true;
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
		public void UpdateInDayMoney()
		{
			_moneyText.text = PlayerData.PlayerTreasury.ToString();
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
			Debug.Log("UIManager: OnEndDayHandler");
			_endDayPanel.Show(true);
		}

		private void OnLoadDataSuccessHandler()
		{
			UpdateInDayMoney();
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
			TransitionManager.Instance().Transition("MainMenu", TransitionSetting, 0.3f);
		}
		private void OnResumeClickHandler()
		{
			HidePauseGameMenu();
			GameplayManager.Instance.OnPausePressed?.Invoke();
		}
		#endregion
	}
}
