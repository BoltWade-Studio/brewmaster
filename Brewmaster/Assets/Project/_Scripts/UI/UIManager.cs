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
        [SerializeField] private TextMeshProUGUI _targetMoney, _profitMoney, _bonusMoney;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private CustomButton _shopBtn, _nextDayBtn, _mainMenuBtn;
        [SerializeField] private float _moneyIncreaseSpeed = 30;

        [Header("Pause game menu")]
        [SerializeField] private GameObject _pauseGameMenu;
        [SerializeField] private TextMeshProUGUI _pMoneyText, _pDayText;
        [SerializeField] private CustomButton _pResumeBtn, _pToMainMenuBtn;
        [SerializeField] private TransitionSettings _transitionSetting;

        [Header("Store")]
        [SerializeField] private GameObject _storeMenu;
        [SerializeField] private CustomButton _confirmButton;
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
            if(_playerInfoPanel.isActiveAndEnabled)
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
            _moneyText.text = MoneyManager.Instance.CurrentTotalMoney.ToString();
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
        private async void ShowEndDayPanel(bool isUpdate = false)
        {
            if(isUpdate)
            {
                await PlayMoneyAnimation(MoneyManager.Instance.CurrentTotalMoney);
                MoneyManager.Instance.CommitEndDay();
            }
            if(GameplayManager.Instance.IsWin)
            {
                _mainMenuBtn.gameObject.SetActive(false);
                _shopBtn.gameObject.SetActive(true);
                _nextDayBtn.gameObject.SetActive(true);
            }
            else
            {
                _mainMenuBtn.gameObject.SetActive(true);
                _shopBtn.gameObject.SetActive(false);
                _nextDayBtn.gameObject.SetActive(false);
            }
            SoundManager.PlaySound(SoundEnum.MoneySound);
            _ingameMenu.SetActive(false);
            _endDayMenu.SetActive(true);
            _endDayMenu.transform.DOScale(Vector3.one, 0.7f);
            EventSystem.current.SetSelectedGameObject(null);
        }
        private void HideEndDayPanel()
        {
            _ingameMenu.SetActive(true);
            _endDayMenu.transform.DOScale(Vector3.zero, 0.7f).OnComplete(() => _endDayMenu.SetActive(false));
        }
        private async UniTask PlayMoneyAnimation(int money)
        {
            float time = 0;
            float temp = 0;

            _targetMoney.text = MoneyManager.Instance.CurrentTarget.ToString();
            _profitMoney.text = (MoneyManager.Instance.CurrentTotalMoney - MoneyManager.Instance.CurrentTarget).ToString();
            _bonusMoney.text = MoneyManager.Instance.Bonus.ToString();
            _resultText.gameObject.SetActive(false);

            // Show currentTotalMoney
            while(temp != money)
            {
                await UniTask.Yield();
                time += Time.unscaledDeltaTime * _moneyIncreaseSpeed;
                temp = Mathf.Lerp(0, money, time/SoundManager.GetSoundLength(SoundEnum.MoneySound));
                _totalMoney.text = temp.ToString();
            }

            _resultText.gameObject.SetActive(true);
            if(GameplayManager.Instance.IsWin)
            {
                _resultText.text = "CONGRATULATION";
                _resultText.color = Color.green;
            }
            else
            {
                _resultText.text = "YOU LOSE";
                _resultText.color = Color.red;
            }
        }
        private void ActiveStorePhrase()
        {
            UpdateInDayMoney();
            HideEndDayPanel();
            ShowStoreMenu();
            OnStorePhrase?.Invoke();
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
            if(_isStorePhrase)
                ShowStoreMenu();
        }
        private void UpdatePauseText()
        {
            _pDayText.text = TimeManager.Instance.CurrentDay.ToString("00");
            _pMoneyText.text = MoneyManager.Instance.CurrentTotalMoney.ToString("0");
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
