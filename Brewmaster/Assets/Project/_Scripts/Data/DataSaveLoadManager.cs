using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using UnityEngine;

namespace Game
{
	public class DataSaveLoadManager : MonoBehaviorInstance<DataSaveLoadManager>
	{
		public int Day;
		public TableData TableData;

		protected override void ChildAwake()
		{
			base.ChildAwake();
			Day = 0;
			TableData = new TableData();
		}

		public async UniTask LoadData()
		{
			string jsonData = await TransactionManager.Instance.GetPlayerPub();

			PlayerData.PlayerDataClass = JsonConvert.DeserializeObject<PlayerDataClass>(jsonData);

			// Create new pub if scale is 0
			if (PlayerData.PlayerScale.Count() == 0)
			{
				if (Application.isEditor == false)
				{
					await TransactionManager.Instance.CreatePub();
					jsonData = await TransactionManager.Instance.GetPlayerPub();
					PlayerData.PlayerDataClass = JsonConvert.DeserializeObject<PlayerDataClass>(jsonData);
				}
			}

			// Play game
			TableData = new TableData();
			for (int i = 0; i < PlayerData.PlayerScale.Count(); i++)
			{
				TableData.SeatNumberList.Add(PlayerData.PlayerScale[i].Stools);
			}
		}
	}
}
