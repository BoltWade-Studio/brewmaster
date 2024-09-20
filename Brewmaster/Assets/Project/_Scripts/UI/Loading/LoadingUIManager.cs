using System.Collections;
using System.Collections.Generic;
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

            if (_loadingUI == null || _loadingUI.gameObject == null)
                _loadingUI = Instantiate(_loadingUIPref, _uiHolder);
            _loadingUI.gameObject.SetActive(true);

            // Set UI as top
            _loadingUI.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _loadingUI?.gameObject?.SetActive(false);
        }
    }
}
