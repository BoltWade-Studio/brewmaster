using System;
using NOOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using NOOD.Sound;
using EasyTransition;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Utils;

namespace Game
{
	public class UIManager : MonoBehaviorInstance<UIManager>
	{
		#region Events
		public Action OnStorePhrase;
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

		[Header("End day menu")]
		[SerializeField] private GameObject _endDayMenu;
		[SerializeField] private TextMeshProUGUI _totalMoney;
		// [SerializeField] private TextMeshProUGUI _targetMoney, _profitMoney, _bonusMoney;
		[SerializeField] private TextMeshProUGUI _resultText;
		[SerializeField] private CustomButton _shopBtn, _nextDayBtn, _mainMenuBtn;
		[SerializeField] private float _moneyIncreaseSpeed = 30;
		[SerializeField] private CustomButton _shareToTwitterBtn;
		[SerializeField] private GameObject _blockXImage;

		[Header("Pause game menu")]
		[SerializeField] private GameObject _pauseGameMenu;
		[SerializeField] private TextMeshProUGUI _pMoneyText, _pDayText;
		[SerializeField] private CustomButton _pResumeBtn, _pToMainMenuBtn;
		[SerializeField] private TransitionSettings _transitionSetting;

		[Header("Store")]
		[SerializeField] private GameObject _storeMenu;
		[SerializeField] private CustomButton _confirmButton;

		[Header("Twitter")]
		[SerializeField] private TwitterInputPanel _twitterInputPanel;

		private bool _isStorePhrase;

		#region Unity functions
		protected override void ChildAwake()
		{
			_endDayMenu.SetActive(false);
			_pauseGameMenu.SetActive(false);
			_playerInfo.OnClick += OnPlayerInfoClickHandler;
			_pauseGameBtn.OnClick += OnPauseButtonClickHandler;
			_pResumeBtn.OnClick += OnResumeClickHandler;
			_shopBtn.OnClick += () =>
			{
				ActiveStorePhrase();
				_isStorePhrase = true;
			};
			_pToMainMenuBtn.OnClick += OnMainMenuClickHandler;
			_nextDayBtn.OnClick += () =>
			{
				OnNextDayPressed?.Invoke();
				UpdateDayText();
			};
			_confirmButton.OnClick += () =>
			{
				ShowEndDayPanel(false);
				HideStoreMenu();
				_isStorePhrase = false;
			};
			_mainMenuBtn.OnClick += () =>
			{
				PlayerPrefs.DeleteAll();
				SceneManager.LoadScene("MainMenu");
			};
			HideStoreMenu();
			OnNextDayPressed += HideEndDayPanel;
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
			GameplayManager.Instance.OnNextDay += () =>
			{
				_timeBG.color = _timeOriginalColor;
			};

			GameplayManager.Instance.OnEndDay += OnEndDayHandler;
			GameplayManager.Instance.OnNextDay += OnNextDayHandler;
			TimeManager.OnTimePause += ShowPauseGameMenu;
			TimeManager.OnTimeResume += HidePauseGameMenu;
		}

		void OnEnable()
		{
			if (ConnectWalletManager.Instance.isAnonymous)
			{
				_shareToTwitterBtn.GetComponent<Button>().interactable = false;
				_blockXImage.gameObject.SetActive(true);
			}
			else
			{
				_shareToTwitterBtn.GetComponent<Button>().interactable = true;
				_blockXImage.gameObject.SetActive(false);
			}
			_shareToTwitterBtn.OnClick += OnShareToTwitterBtnPress;
		}

		private void Update()
		{
			if (GameplayManager.Instance.IsEndDay) return;
			UpdateTime();
		}
		void OnDisable()
		{
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
			NoodyCustomCode.UnSubscribeAllEvent<TimeManager>(this);
			NoodyCustomCode.UnSubscribeFromStatic(typeof(TimeManager), this);
			NoodyCustomCode.UnSubscribeAllEvent(_shareToTwitterBtn, this);
		}
		private void OnDestroy()
		{
			OnNextDayPressed -= HideEndDayPanel;
			_shopBtn.OnClick -= ActiveStorePhrase;
			_endDayMenu.transform.DOKill();
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

		#region End game
		private void OnNextDayHandler()
		{
			UpdateInDayMoney();
		}
		private void OnEndDayHandler()
		{
			ShowEndDayPanel(true);
		}
		private void ShowEndDayPanel(bool isUpdate = false)
		{
			if (isUpdate)
			{
				PlayMoneyAnimation(PlayerData.PlayerTreasury);
			}

			_mainMenuBtn.gameObject.SetActive(true);
			_shopBtn.gameObject.SetActive(true);
			_nextDayBtn.gameObject.SetActive(true);

			_ingameMenu.SetActive(false);
			_endDayMenu.SetActive(true);
			_endDayMenu.transform.DOScale(Vector3.one, 0.7f);
			SoundManager.PlaySound(SoundEnum.MoneySound);
			EventSystem.current.SetSelectedGameObject(null);
		}
		private void HideEndDayPanel()
		{
			_ingameMenu.SetActive(true);
			_endDayMenu.transform.DOScale(Vector3.zero, 0.7f).OnComplete(() => _endDayMenu.SetActive(false));
		}
		private async void PlayMoneyAnimation(int money)
		{
			float time = 0;
			float temp = 0;

			_resultText.gameObject.SetActive(false);

			// Show currentTotalMoney
			while (temp != money)
			{
				await UniTask.Yield();
				time += Time.unscaledDeltaTime * _moneyIncreaseSpeed;
				temp = Mathf.Lerp(0, money, time / SoundManager.GetSoundLength(SoundEnum.MoneySound));
				_totalMoney.text = temp.ToString();
			}

			_resultText.gameObject.SetActive(true);

			_resultText.text = "CONGRATULATION";
			_resultText.color = Color.green;
		}
		private void ActiveStorePhrase()
		{
			UpdateInDayMoney();
			HideEndDayPanel();
			ShowStoreMenu();
			OnStorePhrase?.Invoke();
		}
		private void OnShareToTwitterBtnPress()
		{
			TwitterShareManager.Instance.ShareToTwitter();
			ShowTwitterInputPanel();
		}
		public void ShowTwitterInputPanel()
		{
			_twitterInputPanel.Show();
		}
		#endregion

		#region Store Menu
		private void ShowStoreMenu()
		{
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
			_pMoneyText.text = PlayerData.TotalPoint.ToString("0");
		}
		#endregion

		#region ButtonZone
		private void OnPauseButtonClickHandler()
		{
			GameplayManager.Instance.OnPausePressed?.Invoke();
		}
		private void OnMainMenuClickHandler()
		{
			TransitionManager.Instance().Transition("MainMenu", _transitionSetting, 0.3f);
		}
		private void OnResumeClickHandler()
		{
			HidePauseGameMenu();
			GameplayManager.Instance.OnPausePressed?.Invoke();
		}
		#endregion
	}
}
