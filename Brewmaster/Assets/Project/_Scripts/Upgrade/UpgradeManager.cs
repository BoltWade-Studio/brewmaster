using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
			Utility.Socket.SubscribeEvent(SocketEnum.updateUpgradePriceCallback.ToString(), this.gameObject.name, nameof(UpdateUpgradePrice), UpdateUpgradePrice);

			GameEvent.Instance.OnStorePhase += OnStorePhaseHandler;
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

		private void OnStorePhaseHandler()
		{
			Utility.Socket.EmitEvent(SocketEnum.updateUpgradePrice.ToString());
		}

		public async void UpdateUpgradePrice(string data)
		{
			await UniTask.SwitchToMainThread();
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			Debug.Log("UpdatePriceCallback: " + useData);
			var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(useData.ToString());

			foreach (var _upgradeBase in _upgradeBaseList)
			{
				if (_upgradeBase._table.TableIndex == int.Parse(dict["i"]))
				{
					_upgradeBase.Price = int.Parse(dict["message"]);
					_upgradeBase.UpdatePrice();
				}
			}
		}

		// public async void UpgradeCallback(string data)
		// {
		// 	await UniTask.SwitchToMainThread();

		// 	var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

		// 	foreach (var _upgradeBase in _upgradeBaseList)
		// 	{
		// 		if (_upgradeBase._table.TableIndex == int.Parse(dict["tableIndex"]))
		// 		{
		// 			_upgradeBase.Price = int.Parse(dict["message"]);
		// 			_upgradeBase.Upgrade();
		// 		}
		// 	}
		// }

		public void AddUpgradeBase(UpgradeBase upgradeBase)
		{
			_upgradeBaseList.Add(upgradeBase);
		}

		public void RemoveUpgradeBase(UpgradeBase upgradeBase)
		{
			_upgradeBaseList.Remove(upgradeBase);
		}

		public void ClearSeatUpgrade()
		{
			// Clear all upgrade base that is seat upgrade
			List<UpgradeBase> removeList = new List<UpgradeBase>();
			foreach (var upgradeBase in _upgradeBaseList)
			{
				if (upgradeBase is SeatUpgrade)
				{
					removeList.Add(upgradeBase);
				}
			}

			foreach (var upgradeBase in removeList)
			{
				_upgradeBaseList.Remove(upgradeBase);
			}
		}
	}

}
