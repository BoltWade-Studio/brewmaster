using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;

namespace Game
{
	public class MoneyManager : MonoBehaviorInstance<MoneyManager>
	{

		void Start()
		{
			Utility.Socket.SubscribeEvent(SocketEnum.updateTreasuryCallback.ToString(), this.gameObject.name, nameof(OnUpdatePlayerTreasury), OnUpdatePlayerTreasury);
			Utility.Socket.SubscribeEvent(SocketEnum.updateInDayTreasuryCallback.ToString(), this.gameObject.name, nameof(OnUpdateInDayPlayerTreasury), OnUpdateInDayPlayerTreasury);
		}

		private async void OnUpdatePlayerTreasury(string point)
		{
			await UniTask.SwitchToMainThread();
			PlayerData.PlayerDataClass.Treasury = JsonConvert.DeserializeObject<int>(point);
			UIManager.Instance.UpdateMoneyUI();
			UIManager.Instance.UpdatePlayerInfoPanel();
		}

		private async void OnUpdateInDayPlayerTreasury(string point)
		{
			await UniTask.SwitchToMainThread();
			PlayerData.InDayTreasury = JsonConvert.DeserializeObject<int>(point);
			UIManager.Instance.UpdateMoneyUI();
		}
	}

}
