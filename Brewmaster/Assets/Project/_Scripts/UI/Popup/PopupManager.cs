using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    public class PopupManager : MonoBehaviorInstance<PopupManager>
    {
        [SerializeField] private YesNoPopup _yesNoPopupPref;
        private Transform _uiHolder;

        void Start()
        {
            _uiHolder = UIManager.Instance.transform;
        }

        public void ShowYesNoPopup(string popupName, string message, Action yesAction, Action noAction)
        {
            Instantiate(_yesNoPopupPref, _uiHolder).Show(popupName, message, yesAction, noAction);
        }
    }
}
