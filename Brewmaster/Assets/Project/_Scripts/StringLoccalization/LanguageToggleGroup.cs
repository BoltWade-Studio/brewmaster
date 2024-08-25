using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LanguageToggleGroup : MonoBehaviour
    {
        [SerializeField] private Toggle _vietnameseToggle, _englishToggle;
        [SerializeField] private StringLocalizationDataSO _stringData;

        void Awake()
        {
            if(_stringData.IsEnglish)
            {
                _englishToggle.SetIsOnWithoutNotify(true);
                _vietnameseToggle.SetIsOnWithoutNotify(false);
            }
            else
            {
                _englishToggle.SetIsOnWithoutNotify(false);
                _vietnameseToggle.SetIsOnWithoutNotify(true);
            }
        }
    }
}
