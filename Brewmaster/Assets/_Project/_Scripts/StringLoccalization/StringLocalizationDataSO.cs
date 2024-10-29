using System;
using System.Collections;
using System.Collections.Generic;
using NOOD.SerializableDictionary;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class StringPair
    {
        [TextArea(3, 5)]
        public string Vietnamese;
        [TextArea(3, 5)]
        public string English;
    }
    [Serializable]
    public enum Language
    {
        Vietnamese,
        English
    }

    [CreateAssetMenu(fileName = "String Localization Data")]
    public class StringLocalizationDataSO : ScriptableObject
    {
        public Action<Language> OnLocalizationChange;
        public bool IsEnglish;
        [SerializeField] private SerializableDictionary<string, StringPair> _stringDic = new SerializableDictionary<string, StringPair>();

        public Dictionary<string, StringPair> GetDictionary() => _stringDic.Dictionary;
    }
}
