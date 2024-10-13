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
			foreach (var upgradeBase in _upgradeBaseList)
			{
				upgradeBase.UpdatePrice();
			}
		}

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
