using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
	        Utility.Socket.OnEvent("updateUpgradePrice", this.gameObject.name, nameof(UpdateUpgradePrice), UpdateUpgradePrice);
	        Utility.Socket.EmitEvent("updateUpgradePrice");
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

        public void UpdateUpgradePrice(string data)
        {
	        // Debug.Log("UpdateUpgradePrice: " + data);

	        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

	        foreach (var _upgradeBase in _upgradeBaseList)
	        {
		        if (_upgradeBase._table.TableIndex == int.Parse(dict["i"]))
		        {
			        // Debug.Log("Found Table to Update Price! " + dict["message"]);
			        _upgradeBase.Price = int.Parse(dict["message"]);
			        _upgradeBase.toUpdatePrice = true;
		        }
	        }
        }

        public void UpgradeCallback(string data)
        {
	        // Debug.Log("UpgradeCallback: " + data);

	        if (data == "fail")
	        {
		        _showNotiNotEnoughMoney = true;
		        return;
	        }

	        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

	        foreach (var _upgradeBase in _upgradeBaseList)
	        {
		        if (_upgradeBase._table.TableIndex == int.Parse(dict["tableIndex"]))
		        {
			        // Debug.Log("Found Table to Upgrade!");
			        _upgradeBase.Price = int.Parse(dict["message"]);
			        _upgradeBase.toUpgrade = true;
		        }
	        }
        }

        public void AddUpgradeBase(UpgradeBase upgradeBase)
		{
			// Debug.Log("AddUpgradeBase: " + upgradeBase._table.TableIndex);
			_upgradeBaseList.Add(upgradeBase);
		}
    }

}
