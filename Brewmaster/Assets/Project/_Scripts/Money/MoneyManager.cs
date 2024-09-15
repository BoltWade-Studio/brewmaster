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
			Utility.Socket.OnEvent(SocketEnum.updateTreasuryCallback.ToString(), this.gameObject.name, nameof(OnUpdatePlayerTreasury), OnUpdatePlayerTreasury);
		}

		private async void OnUpdatePlayerTreasury(string point)
		{
			await UniTask.SwitchToMainThread();
			PlayerData.PlayerDataClass.Treasury = JsonConvert.DeserializeObject<int>(point);
			UIManager.Instance.UpdateInDayMoney();
		}
	}

}
