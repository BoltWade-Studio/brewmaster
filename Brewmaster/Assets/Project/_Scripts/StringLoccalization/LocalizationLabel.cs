using NOOD;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LocalizationLabel : MonoBehaviour
    {
        [SerializeField] private string _textId;
        [SerializeField] private TextMeshProUGUI _label;

        void OnEnable()
        {
            StringLocalization.OnLocalizationChange += OnLanguageChangeHandler;
            UpdateText();
        }
        void OnDisable()
        {
            StringLocalization.OnLocalizationChange -= OnLanguageChangeHandler;
        }

        private void OnLanguageChangeHandler()
        {
            UpdateText();
        }
        public void UpdateText()
        {
            if (_label)
                _label.text = _textId.GetText();
        }
    }
}
