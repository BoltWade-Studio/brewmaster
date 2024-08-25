using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class LanguageToggle : MonoBehaviour
    {
        [SerializeField] private Language _language;
        [SerializeField] private StringLocalizationDataSO _stringData;

        public void ChangeLanguage()
        {
            _stringData.OnLocalizationChange?.Invoke(_language);
        }
    }
}
