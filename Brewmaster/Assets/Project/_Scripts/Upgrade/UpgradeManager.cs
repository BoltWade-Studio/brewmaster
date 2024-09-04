using System;
using System.Collections.Generic;
using NOOD;
using UnityEngine;

namespace Game
{
    public class UpgradeManager : MonoBehaviorInstance<UpgradeManager>
    {
        private List<UpgradeBase> _upgradeBaseList = new List<UpgradeBase>();
        private bool _showNotiNotEnoughMoney = false;

        private void Start()
        {
	        Utility.Socket.OnEvent("upgradeTableCallback", this.gameObject.name, nameof(UpgradeCallback), UpgradeCallback);
        }

        private void Update()
        {
	        if (_showNotiNotEnoughMoney)
	        {
		        NotifyManager.Instance.Show("Do not enough money");
		        _showNotiNotEnoughMoney = false;
	        }
        }

        protected override void ChildAwake()
        {
            _upgradeBaseList = new List<UpgradeBase>();
        }

        public void UpgradeCallback(string data)
        {
	        Debug.Log("UpgradeCallback: " + data);

	        if (data == "fail")
	        {
		        _showNotiNotEnoughMoney = true;
		        return;
	        }

	        foreach (var _upgradeBase in _upgradeBaseList)
	        {
		        if (_upgradeBase._table.TableIndex == int.Parse(data))
		        {
			        Debug.Log("Found Table to Upgrade!");
			        _upgradeBase.toUpgrade = true;
		        }
	        }
        }

        public void AddUpgradeBase(UpgradeBase upgradeBase)
		{
			_upgradeBaseList.Add(upgradeBase);
		}
    }

}
