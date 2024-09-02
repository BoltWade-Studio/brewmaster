using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using UnityEngine;

namespace Game
{
	public class DataSaveLoadManager : MonoBehaviorInstance<DataSaveLoadManager>
	{
		public TableData TableData;
		public int Day;
		public int Money;
		public int Target;
		private bool _isLoaded = false;
		public string _json;

		protected override void ChildAwake()
		{
			Utility.Socket.OnEvent(SocketEnum.loadCallback.ToString(), this.gameObject.name, nameof(LoadCallback), LoadCallback);
			Load();
		}

		void Start()
		{
			GameplayManager.Instance.OnEndDay += Save;
			GameplayManager.Instance.OnNextDay += Save;
		}

		void OnDestroy()
		{
			GameplayManager.Instance.OnEndDay -= Save;
			GameplayManager.Instance.OnNextDay -= Save;
		}

		private void Save()
		{
			// Dictionary<string, object> data = new Dictionary<string, object>
			// {
			// 	{"day", TimeManager.Instance.CurrentDay},
			// 	{"money", MoneyManager.Instance.CurrentTotalMoney},
			// 	{"Target", MoneyManager.Instance.CurrentTarget},
			// 	{"TableData", TableData}
			// };

			// Debug.Log("Save request");
			// string json = JsonConvert.SerializeObject(data);
			// _json = json;
			// Utility.Socket.EmitEvent(SocketEnum.saveDataRequest.ToString(), json);
		}
		private async void Load()
		{
			Utility.Socket.EmitEvent(SocketEnum.loadDataRequest.ToString());
			_isLoaded = false;
			await UniTask.WaitUntil(() => _isLoaded);
		}

		private void LoadCallback(string json)
		{
			if (string.IsNullOrEmpty(json) == false)
			{
				Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
				Day = (int)data["day"];
				Money = (int)data["money"];
				TableData = (TableData)data["TableData"];
				Target = (int)data["Target"];
			}
			else
			{
				Day = 1;
				Money = 0;
				TableData = new TableData();
				Target = 100;
				PlayerData.Reset();
			}
			_isLoaded = true;
		}
	}
}
