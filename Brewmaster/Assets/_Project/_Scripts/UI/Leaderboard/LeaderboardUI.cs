using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LeaderboardUI : MonoBehaviour
    {
        private const string LEADERBOARD_ALL_TIME_DATA_KEY = "LeaderboardAllTime";
        private const string LEADERBOARD_WEEKLY_DATA_KEY = "LeaderboardWeekly";
        [SerializeField] private PlayerRowUI _playerRowPrefab;
        [SerializeField] private Transform _playerRowsContainer;
        [SerializeField] private PlayerRowUI _mainPlayerRow;
        [SerializeField] private Transform _panelTransform, _hideTransform, _showTransform;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private LeaderboardDataController _leaderboardDataController;

        [Header("FilterTabs")]
        [SerializeField] private Button _weeklyBtn;
        [SerializeField] private Button _allTimeBtn;
        private bool _isWeekly = true;
        private List<PlayerRowUI> _playerRows = new List<PlayerRowUI>();
        private float _refreshTime = 0;

        #region Unity Functions
        void Start()
        {
            _playerRowPrefab.gameObject.SetActive(false);
            Hide();

            _closeBtn.onClick.AddListener(Hide);
            _weeklyBtn.onClick.AddListener(() =>
            {
                ChangeFilterWeekly(true);
                _weeklyBtn.interactable = false;
                _allTimeBtn.interactable = true;
            });
            _allTimeBtn.onClick.AddListener(() =>
            {
                ChangeFilterWeekly(false);
                _weeklyBtn.interactable = true;
                _allTimeBtn.interactable = false;
            });
            _leaderboardDataController.GetRemoteData();
            _refreshTime = Time.time;
        }

        void OnEnable()
        {
            _leaderboardDataController.OnGetRemoteDataComplete += OnGetRemoteDataComplete;
        }


        void Update()
        {
            if (Time.time - _refreshTime >= 10)
            {
                _leaderboardDataController.GetRemoteData();
                ChangeFilterWeekly(_isWeekly);
                _refreshTime = Time.time;
            }
        }

        void OnDisable()
        {
            _leaderboardDataController.OnGetRemoteDataComplete -= OnGetRemoteDataComplete;
        }

        void OnDestroy()
        {
            _closeBtn.onClick.RemoveListener(Hide);
            _weeklyBtn.onClick.RemoveAllListeners();
            _allTimeBtn.onClick.RemoveAllListeners();
        }
        #endregion


        private void OnGetRemoteDataComplete(OnGetRemoteDataCompleteArgs args)
        {
            if (args.IsWeekly && _isWeekly)
            {
                ShowWeeklyData();
            }
            else if (args.IsWeekly == false && _isWeekly == false)
            {
                ShowAllTimeData();
            }
        }

        private void ChangeFilterWeekly(bool isWeekly)
        {
            _isWeekly = isWeekly;
            if (_isWeekly)
            {
                ShowWeeklyData();
            }
            else
            {
                ShowAllTimeData();
            }
        }

        private void ShowWeeklyData()
        {
            if (PlayerPrefs.HasKey(LEADERBOARD_WEEKLY_DATA_KEY))
            {
                var data = PlayerPrefs.GetString(LEADERBOARD_WEEKLY_DATA_KEY); // Get local leaderboard data
                ParseLeaderboardData(data);
            }
        }

        private void ShowAllTimeData()
        {
            if (PlayerPrefs.HasKey(LEADERBOARD_ALL_TIME_DATA_KEY))
            {
                var data = PlayerPrefs.GetString(LEADERBOARD_ALL_TIME_DATA_KEY); // Get local leaderboard data
                ParseLeaderboardData(data);
            }
        }


        private void ParseLeaderboardData(string data)
        {
            var leaderboardResponse = JsonConvert.DeserializeObject<LeaderboardResponse>(data);
            if (leaderboardResponse.Success)
            {
                var leaderboardData = leaderboardResponse.Data;
                CreatePlayerRows(leaderboardData.Items);
            }
            SetDataForMainPLayerRow(leaderboardResponse.Data.Items);
        }

        private void SetDataForMainPLayerRow(List<LeaderboardItem> items)
        {
            string playerAddress = "0x0000000000000000000000000000000000000000";
            int playerPoint = 0;
            if (PlayerData.PlayerDataClass != null)
            {
                playerAddress = PlayerData.PlayerAddress;
                playerPoint = PlayerData.PlayerPoint;
            }

            _mainPlayerRow.SetPlayerAddress(playerAddress);
            _mainPlayerRow.SetPlayerScore(PointDisplay.GetPointDisplayText(playerPoint));

            bool isInTop = items.Any(x => x.UserAddress == playerAddress);
            int index = -1;
            Debug.Log("isInTop: " + isInTop);
            if (isInTop)
            {
                index = items.FindIndex(item => item.UserAddress == PlayerData.PlayerAddress);
            }

            _mainPlayerRow.SetPlayerRank(index);
        }

        private void CreatePlayerRows(List<LeaderboardItem> items)
        {
            while (_playerRows.Count != items.Count)
            {
                if (_playerRows.Count < items.Count)
                {
                    var playerRow = Instantiate(_playerRowPrefab, _playerRowsContainer);
                    _playerRows.Add(playerRow);
                    playerRow.gameObject.SetActive(true);
                }
                if (_playerRows.Count > items.Count)
                {
                    PlayerRowUI playerRow = _playerRows[_playerRows.Count - 1];
                    _playerRows.Remove(playerRow);
                    Destroy(playerRow.gameObject);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                var playerRow = _playerRows[i];
                playerRow.SetPlayerRank(i);
                playerRow.SetPlayerAddress(items[i].UserAddress);
                playerRow.SetPlayerScore(PointDisplay.GetPointDisplayText(items[i].Point));
            }
        }

        #region Show Hide
        public void ShowLeaderboard()
        {
            Show();
        }
        private void Show()
        {
            _panelTransform.gameObject.SetActive(true);
            _closeBtn.interactable = true;
            _closeBtn.gameObject.SetActive(true);

            // Using DoTween for animation
            if (_panelTransform != null)
            {
                // Store the initial position
                Vector3 showPos = _showTransform.position;

                // Move the panel off-screen to the right
                _panelTransform.position = _hideTransform.position;

                // Animate the panel sliding in from the right
                _panelTransform.DOMove(showPos, 0.5f).SetEase(Ease.OutFlash);

                // Fade in
                CanvasGroup canvasGroup = _panelTransform.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.DOFade(1, .5f);
                }
                else
                {
                    Debug.LogWarning("CanvasGroup component not found on panel. Fade animation won't play.");
                }
            }
            else
            {
                Debug.LogWarning("_panelTransform is null. Animation won't play.");
            }
        }

        public void Hide()
        {
            // Using DoTween for animation
            if (_panelTransform != null)
            {
                // Store the initial position
                Vector3 hidePos = _hideTransform.position;
                _closeBtn.interactable = false;
                _closeBtn.gameObject.SetActive(false);

                // Animate the panel sliding out to the right
                _panelTransform.DOMove(hidePos, 0.5f).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    // Deactivate the game object after the animation completes
                    _panelTransform.gameObject.SetActive(false);
                });

                // Fade out
                CanvasGroup canvasGroup = _panelTransform.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.DOFade(0, .5f);
                }
                else
                {
                    Debug.LogWarning("CanvasGroup component not found on panel. Fade animation won't play.");
                }
            }
            else
            {
                Debug.LogWarning("_panelTransform is null. Animation won't play.");
                _panelTransform.gameObject.SetActive(false);
            }
        }
        #endregion
    }


}
