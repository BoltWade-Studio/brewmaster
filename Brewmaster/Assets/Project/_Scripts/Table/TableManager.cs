using System;
using System.Collections;
using System.Collections.Generic;
using NOOD;
using NOOD.Data;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class TableData
	{
		public List<int> SeatNumberList = new List<int>();
	}

	public class TableManager : MonoBehaviorInstance<TableManager>
	{
		public Action<Table> OnTableUpgrade;
		public Action<Customer> OnCustomComplete;
		public Action<Customer> OnCustomerRequestBeer;
		[SerializeField] private List<Table> _tableList = new List<Table>();
		[SerializeField] private bool _unlockAllSeats;
		private int _currentTableIndex = 0;
		private TableData _tableData;

		#region Unity Functions
		void Start()
		{
			if (DataSaveLoadManager.Instance)
				_tableData = DataSaveLoadManager.Instance.TableData;
			for (int i = 0; i < _tableList.Count; i++)
			{
				if (i >= _tableData.SeatNumberList.Count)
				{
					_tableData.SeatNumberList.Add(1);
				}
				_tableList[i].AvailableSeatNumber = _tableData.SeatNumberList[i];
			}

			OnTableUpgrade += OnTableUpgradeHandler;
		}
		void OnEnable()
		{
			Player.OnPlayerChangePosition += OnPlayerChangePositionHandler;
			foreach (Table table in _tableList)
			{
				table.SetIsUnlockAllSeats(_unlockAllSeats);
			}
		}
		void OnDisable()
		{
			Player.OnPlayerChangePosition -= OnPlayerChangePositionHandler;
		}
		void OnDestroy()
		{
			OnTableUpgrade -= OnTableUpgradeHandler;
		}
		#endregion

		private void OnTableUpgradeHandler(Table table)
		{
			int index = _tableList.IndexOf(table);
			_tableData.SeatNumberList[index] += 1;
		}
		private void OnPlayerChangePositionHandler(int index)
		{
			_currentTableIndex = index;
		}

		public List<Table> GetTableList()
		{
			return _tableList;
		}
		public Vector3 GetPlayerTablePosition()
		{
			return _tableList[_currentTableIndex].transform.position;
		}
		public Table GetTable(int index)
		{
			try
			{
				return _tableList[index];
			}
			catch
			{
				Debug.LogError("table index out of range");
				return null;
			}
		}
		public Table GetPlayerTable()
		{
			return _tableList[_currentTableIndex];
		}
		public void CustomerComplete(Customer customer)
		{
			OnCustomComplete?.Invoke(customer);
		}
		public void RequestBeer(Customer customer)
		{
			OnCustomerRequestBeer?.Invoke(customer);
		}
	}

}
