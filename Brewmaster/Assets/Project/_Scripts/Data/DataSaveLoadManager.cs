using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			Day = 1;
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
			Debug.Log("LoadData");
			await GetDataAsync();

			// Create new pub if scale is 0
			if (PlayerData.PlayerScale.Count() == 0)
			{
				bool isCreated = await CreateNewPub();
				if (isCreated)
				{
					LoadingUIManager.Instance.ChangeLoadingMessage("Getting player data");
					await GetDataAsync();
					PlayGame();
				}
			}
			else
			{
				Debug.Log("PlayGame");
				PlayGame();
			}
		}

		private void PlayGame()
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

		private async UniTask GetDataAsync()
		{
			string jsonData = await TransactionManager.Instance.GetPlayerPub();

			Debug.Log("LoadData: " + jsonData);
			PlayerData.PlayerDataClass = JsonConvert.DeserializeObject<PlayerDataClass>(jsonData);
		}

		private async UniTask<bool> CreateNewPub()
		{
			bool isCreatedPub = await TransactionManager.Instance.CreatePub();
			if (isCreatedPub == true)
			{
				LoadingUIManager.Instance.Show("Wait for contract update");
				await UniTask.WaitForSeconds(30f);
			}
			else
			{
				LoadingUIManager.Instance.Hide();
				string message = "The user aborted the action, please create a pub to continue playing the game";
				PopupManager.Instance.ShowYesNoPopup("Create new pub", message, async () =>
				{
					await CreateNewPub();
				}, null);
			}
			return isCreatedPub;
		}
	}
}
