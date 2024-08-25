using DG.Tweening;
using NOOD;
using TMPro;
using UnityEngine;

namespace Game
{
    public class NotifyManager : MonoBehaviorInstance<NotifyManager>
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private Transform _showPos, _hidePos;
        [SerializeField] private LocalizationLabel _localizationLabel;
        private bool _isShow = false;

        public void Show(string text)
        {
            if (_isShow == true) return;

            _localizationLabel.UpdateText();
            _textMeshProUGUI.text = text;
            this.transform.DOMove(_showPos.position, 0.8f).SetEase(Ease.OutElastic);
            _isShow = true;
            NoodyCustomCode.StartDelayFunction(() =>
            {
                this.transform.DOMove(_hidePos.position, 0.8f).SetEase(Ease.InElastic);
                _isShow = false;
            }, 1f);
        }
    }
}
