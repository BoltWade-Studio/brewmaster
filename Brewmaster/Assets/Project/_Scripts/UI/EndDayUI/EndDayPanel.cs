using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyTransition;
using Newtonsoft.Json;
using NOOD;
using NOOD.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = Game.DevelopDebug;

namespace Game
{
    public class EndDayPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _endDayMenu;
        [SerializeField] private TwitterInputPanel _twitterInputPanel;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _treasuryText, _pointText, _resultText;
        [SerializeField] private CustomButton _shopBtn, _claimBtn, _nextDayBtn, _mainMenuBtn;
        [SerializeField] private CustomButton _shareToTwitterBtn;
        [SerializeField] private GameObject _blockXImage;
        [SerializeField] private float _moneyIncreaseSpeed = 30;

        private int _tempPoint;
        private bool _gettingData = false;

        private bool _isClaimed;

        #region Unity functions
        void Start()
        {
            Utility.Socket.OnEvent(SocketEnum.getPointBeforeClaimCallback.ToString(), this.gameObject.name, nameof(GetPointBeforeClaimCallback), GetPointBeforeClaimCallback);

            Debug.LogError("End Day Start");
            _shareToTwitterBtn.OnClick += OnShareToTwitterBtnPress;
            _claimBtn.OnClick += OnClaimBtnPress;
            _nextDayBtn.OnClick += OnNextDayBtnPress;
            _mainMenuBtn.OnClick += UIManager.Instance.OnMainMenuClickHandler;
            _shopBtn.OnClick += OnShopBtnPress;

            this.gameObject.SetActive(false);
        }
        void OnEnable()
        {
            GameEvent.Instance.OnClaimSuccess += OnClaimSuccessHandler;
            CheckAnonymous();
            HideTwitterInputPanel();
        }
        void OnDisable()
        {
            GameEvent.Instance.OnClaimSuccess -= OnClaimSuccessHandler;
        }
        #endregion

        #region Event
        private void OnShareToTwitterBtnPress()
        {
            TwitterShareManager.Instance.ActiveShareToXPopup();
        }
        private async void OnClaimBtnPress()
        {
            await TransactionManager.Instance.Claim();
        }
        private void OnShopBtnPress()
        {
            if (_isClaimed == false)
            {
                // Show notification
                NotifyManager.Instance.Show("Please claim first!");
            }
            else
            {
                Hide();
                GameEvent.Instance.OnStorePhase?.Invoke();
            }
        }

        private void OnNextDayBtnPress()
        {
            if (_isClaimed == false)
            {
                string message = "You will lose all today point and treasury if you don't claim, Continue?";
                PopupManager.Instance.ShowYesNoPopup("Are you sure?", message, () =>
                {
                    NextDayButNotClaim();
                }, null);
            }
            else
            {
                NextDay();
            }
        }
        private void OnClaimSuccessHandler()
        {
            // Active shop button
            _claimBtn.gameObject.GetComponent<Button>().interactable = false;
            _isClaimed = true;
        }
        #endregion

        private async void NextDayButNotClaim()
        {
            await DataSaveLoadManager.Instance.LoadData();
            NextDay();
        }
        private async void NextDay()
        {
	        await DataSaveLoadManager.Instance.LoadData();
            Hide();
            GameEvent.Instance.OnNextDay?.Invoke();
        }

        private void CheckAnonymous()
        {
            if (ConnectWalletManager.Instance.IsAnonymous)
            {
                _blockXImage.SetActive(true);
            }
        }
        private async UniTask GetPointBeforeClaim()
        {
            _gettingData = true;
            Utility.Socket.EmitEvent(SocketEnum.getPointBeforeClaim.ToString());
            await UniTask.WaitUntil(() => _gettingData == false);
        }
        private async void GetPointBeforeClaimCallback(string point)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("GetPointBeforeClaimCallback: " + point);
            _tempPoint = JsonConvert.DeserializeObject<int>(point);
            _gettingData = false;
        }

        #region Show hide
        public async void Show(bool isUpdate = false)
        {
            this.gameObject.SetActive(true);
            _canvasGroup.alpha = 0;
            if (isUpdate)
            {
	            if (PlayerData.PlayerTreasury > 0)
	            {
		            _isClaimed = false;
		            _claimBtn.gameObject.GetComponent<Button>().interactable = true;
	            }
	            else
	            {
		            _isClaimed = true;
		            _claimBtn.gameObject.GetComponent<Button>().interactable = false;
	            }
                await GetPointBeforeClaim();
                PlayMoneyAnimation();
            }

            _canvasGroup.DOFade(1, 0.2f);
            _mainMenuBtn.gameObject.SetActive(true);
            _shopBtn.gameObject.SetActive(true);
            _nextDayBtn.gameObject.SetActive(true);

            _endDayMenu.SetActive(true);
            _endDayMenu.transform.DOScale(Vector3.one, 0.3f);
            SoundManager.PlaySound(SoundEnum.MoneySound);
            EventSystem.current.SetSelectedGameObject(null);
        }
        public void Hide()
        {
            _endDayMenu.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => this.gameObject.SetActive(false));
            _canvasGroup.DOFade(0, 0.1f);
        }
        public void ShowTwitterInputPanel()
        {
            _twitterInputPanel.Show();
        }
        private void HideTwitterInputPanel()
        {
            _twitterInputPanel.Hide();
        }
        #endregion

        #region Support
        private async void PlayMoneyAnimation()
        {
            float time = 0;
            float treasuryTemp = 0;
            float pointTemp = 0;

            _resultText.gameObject.SetActive(false);

            float lerpValue = 0;
            // Show currentTotalMoney
            while (lerpValue <= 1)
            {
                await UniTask.Yield();
                time += Time.unscaledDeltaTime * _moneyIncreaseSpeed;
                lerpValue = time / SoundManager.GetSoundLength(SoundEnum.MoneySound);
                treasuryTemp = Mathf.Lerp(0, PlayerData.PlayerTreasury, lerpValue);
                _treasuryText.text = treasuryTemp.ToString();
                pointTemp = Mathf.Lerp(0, _tempPoint, lerpValue);
                _pointText.text = pointTemp.ToString();
            }

            _resultText.gameObject.SetActive(true);

            if (treasuryTemp > 0)
            {
                _resultText.text = "CONGRATULATION";
                _resultText.color = Color.green;
            }
            else
            {
                _resultText.text = "BANKRUPT";
                _resultText.color = Color.red;
            }
        }
        #endregion
    }
}
