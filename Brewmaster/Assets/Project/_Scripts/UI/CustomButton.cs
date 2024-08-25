using NOOD.Sound;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;
using NOOD;

namespace Game
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private string _textId;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        private Button _button;
        public Action OnClick;
        public Action OnHover;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => 
            {
                OnClick?.Invoke();
            });
        }
        private void OnEnable()
        {
            Show();
            UpdateText();
            StringLocalization.OnLocalizationChange += OnLocalizationChangeHandler;
        }
        private void OnDisable()
        {
            NoodyCustomCode.UnSubscribeFromStatic(typeof(StringLocalization), this);
            this.transform.DOKill();
        }

        #region Events functions
        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.PlaySound(SoundEnum.BtnClick, this.transform.position);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.PlaySound(SoundEnum.BtnHover, this.transform.position, 0.5f);
            OnHover?.Invoke();
        }
        public void OnLocalizationChangeHandler()
        {
            UpdateText();
        }
        private void UpdateText()
        {
            if(_textMeshProUGUI)
            {
                _textMeshProUGUI.text = _textId.GetText();
            }
        }    
        #endregion

        #region Show hide
        public void Show()
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.5f);
        }
        public void Hide()
        {
        }
        #endregion
    }
}
