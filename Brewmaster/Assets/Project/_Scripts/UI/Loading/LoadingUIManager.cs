using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NOOD;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LoadingUIManager : MonoBehaviorInstance<LoadingUIManager>
    {
        [SerializeField] private LoadingUI _loadingUIPref;
        [SerializeField] private Transform _uiHolder;
        private LoadingUI _loadingUI;
        private float _time;
        private string _message;
        private bool _isDisconnectPopup;

        protected override void ChildAwake()
        {
            base.ChildAwake();
        }

        void Start()
        {
            if (_uiHolder == null)
                _uiHolder = UIManager.Instance.transform;
        }

        void Update()
        {
            _time += Time.deltaTime;
            if (_loadingUI)
                _loadingUI.SetLoadingText(_message + (int)_time);
            if (_time >= 100 && !_isDisconnectPopup)
            {
                _isDisconnectPopup = true;
                PopupManager.Instance.ShowAnnouncePopup("Server error", "Server disconnected, reload?", "Reload", () =>
                {
                    JsReloadWindow.ReloadWindow();
                });
            }
        }

        public void ChangeLoadingMessage(string message)
        {
            _message = message + "...";
        }
        public void Show(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                _message = "Loading...";
            }
            else
            {
                _message = message + "...";
            }
            _time = 0;
            _isDisconnectPopup = false;

            if (_loadingUI == null || _loadingUI.gameObject == null)
                _loadingUI = Instantiate(_loadingUIPref, _uiHolder);
            _loadingUI.gameObject.SetActive(true);

            // Set UI as top
            _loadingUI.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            if (_loadingUI != null && _loadingUI.gameObject != null)
            {
                _loadingUI.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    _loadingUI.gameObject.SetActive(false);
                });
            }
        }
    }
}
