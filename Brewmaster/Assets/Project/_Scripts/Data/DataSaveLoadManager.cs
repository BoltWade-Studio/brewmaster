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
		}

		void Start()
		{
			Utility.Socket.OnEvent("loadCallback", this.gameObject.name, nameof(LoadCallback), LoadCallback);
			Load();
			GameplayManager.Instance.OnEndDay += Save;
			GameplayManager.Instance.OnNextDay += Save;
			GameplayManager.Instance.OnEndDay += () =>
			{
				Debug.Log("End day");
			};
		}

		void OnDestroy()
		{
			GameplayManager.Instance.OnEndDay -= Save;
			GameplayManager.Instance.OnNextDay -= Save;
		}

		private void Save()
		{
			Dictionary<string, object> data = new Dictionary<string, object>
			{
				{"day", TimeManager.Instance.CurrentDay},
				{"money", MoneyManager.Instance.CurrentTotalMoney},
				{"Target", MoneyManager.Instance.CurrentTarget},
				{"TableData", TableData}
			};

			Debug.Log("Save request");
			string json = JsonConvert.SerializeObject(data);
			_json = json;
			Utility.Socket.EmitEvent("saveDataRequest", json);
		}
		private async void Load()
		{
			Utility.Socket.EmitEvent("loadDataRequest");
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
			}
			_isLoaded = true;
		}
	}
}
