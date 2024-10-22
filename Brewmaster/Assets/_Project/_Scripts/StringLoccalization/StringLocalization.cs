using System;
using NOOD;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class StringLocalization 
    {
        public static Action OnLocalizationChange;
        private StringLocalizationDataSO _stringDicData;

        public void Init()
        {
            _stringDicData = Resources.Load<StringLocalizationDataSO>("Data/String Localization Data");
            _stringDicData.OnLocalizationChange += ChangeLanguage;
        }

        private void ChangeLanguage(Language language)
        {
            if(language == Language.Vietnamese)
            {
                _stringDicData.IsEnglish = false;
            }
            if(language == Language.English)
            {
                _stringDicData.IsEnglish = true;
            }
            OnLocalizationChange?.Invoke();
        }

        public string GetString(string ID)
        {
            if (_stringDicData.IsEnglish)
                return _stringDicData.GetDictionary()[ID].English;
            else
                return _stringDicData.GetDictionary()[ID].Vietnamese;
        }
    }

    public static class LocalizationExtension
    {
        private static StringLocalization _stringLocalization;

        public static string GetText(this string id)
        {
            if(_stringLocalization == null)
            {
                _stringLocalization = new StringLocalization();
                _stringLocalization.Init();
            }
            return _stringLocalization.GetString(id);
        }
    }
}
