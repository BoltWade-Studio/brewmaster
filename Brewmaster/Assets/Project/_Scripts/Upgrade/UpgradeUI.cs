using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UpgradeUI : MonoBehaviour
    {
        public Action OnUpgradeButtonClick;

        [SerializeField] private TextMeshProUGUI _moneyText;
        [SerializeField] private CustomButton _upgradeButton;
        [SerializeField] private UpgradeBase _upgradeBase;
        
        void Start()
        {
            _upgradeButton.OnClick += () => OnUpgradeButtonClick?.Invoke();
        }
        void OnEnable()
        {
            UpdateMoneyText();
            OnUpgradeButtonClick += UpdateMoneyText;
        }
        void OnDisable()
        {
            OnUpgradeButtonClick -= UpdateMoneyText;
        }

        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }

        public void UpdateMoneyText()
        {
            _moneyText.text = _upgradeBase.Price.ToString();
        }
    }

}
