using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using UnityEngine;
using Debug = Game.DevelopDebug;

namespace Game
{
	public class DataSaveLoadManager : MonoBehaviorInstance<DataSaveLoadManager>
	{
		public int Day;
		public TableData TableData = null;

		protected override void ChildAwake()
		{
			base.ChildAwake();
			Day = 0;
		}

		void Start()
		{
			GameEvent.Instance.OnClaimSuccess += ClaimSuccessHandler;
		}

		void OnDisable()
		{
			GameEvent.Instance.OnClaimSuccess -= ClaimSuccessHandler;
		}

		private async void ClaimSuccessHandler()
		{
			await LoadData();
		}

		/// <summary>
		/// This will replace current data with the data in blockchain
		/// </summary>
		/// <returns></returns>
		public async UniTask LoadData()
		{
			string jsonData = await TransactionManager.Instance.GetPlayerPub();

			Debug.Log("LoadData: " + jsonData);
			PlayerData.PlayerDataClass = JsonConvert.DeserializeObject<PlayerDataClass>(jsonData);

			// Create new pub if scale is 0
			if (PlayerData.PlayerScale.Count() == 0)
			{
				if (Application.isEditor == false)
				{
					// Create new pub with sendTransaction
					await TransactionManager.Instance.CreatePub();
					LoadingUIManager.Instance.Show("Updating contract");
					await UniTask.WaitForSeconds(30f);
					LoadingUIManager.Instance.ChangeLoadingMessage("Getting data");
					await LoadData();
				}
			}
			else
			{
				// Play game
				LoadingUIManager.Instance.Hide();
				TableData = new TableData();
				for (int i = 0; i < PlayerData.PlayerScale.Count(); i++)
				{
					TableData.SeatNumberList.Add(PlayerData.PlayerScale[i].Stools);
				}

				GameEvent.Instance.OnLoadDataSuccess?.Invoke();
			}
		}
	}
}
