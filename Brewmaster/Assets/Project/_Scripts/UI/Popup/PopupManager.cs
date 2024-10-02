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
        [SerializeField] private Transform _uiHolder;
        [SerializeField] private AnnouncePopup _announcePopupPref;
        private YesNoPopup _yesNoPopup;
        private AnnouncePopup _announcePopup;

        void Start()
        {
            if (_uiHolder == null)
                _uiHolder = UIManager.Instance.transform;
        }

        public void ShowYesNoPopup(string popupName, string message, Action yesAction, Action noAction)
        {
            if (_yesNoPopup == null)
                _yesNoPopup = Instantiate(_yesNoPopupPref, _uiHolder);

            _yesNoPopup.Show(popupName, message, yesAction, noAction);
        }

        public void ShowAnnouncePopup(string popupName, string message, string btnText, Action btnAction)
        {
            if (_announcePopup == null)
                _announcePopup = Instantiate(_announcePopupPref, _uiHolder);

            _announcePopup.Show(popupName, message, btnText, btnAction);
        }
    }
}
