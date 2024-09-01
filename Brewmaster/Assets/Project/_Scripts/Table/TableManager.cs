using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Game
{
    [Serializable]
    public class TableData
    {
        public List<int> SeatNumberList = new List<int>();
    }

    [Serializable]
    public class SeatListDto
	{
		public List<Vector3> seatPositionList = new List<Vector3>();
	}

	[Serializable]
	public class TableListDto
	{
		public List<SeatListDto> tableList = new List<SeatListDto>();
		public List<Vector3> tablePositionList = new List<Vector3>();
		public Vector3 customerSpawnPosition;
		public Vector3 playerPosition;
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
            _tableData = DataSaveLoadManager.Instance.TableData;
            // Debug.Log("Table Data: " + JsonUtility.ToJson(_tableData));
            for(int i = 0; i < _tableList.Count; i++)
            {
                if(i >= _tableData.SeatNumberList.Count)
                {
                    _tableData.SeatNumberList.Add(1);
                }
                _tableList[i].AvailableSeatNumber = _tableData.SeatNumberList[i];
            }

            OnTableUpgrade += OnTableUpgradeHandler;

            TableListDto seatPositionList = new TableListDto();
            foreach(Table table in _tableList)
			{
				SeatListDto seatListDto = new SeatListDto();
				foreach(Transform seat in table.GetAllSeats())
				{
					seatListDto.seatPositionList.Add(seat.position);
					if (seatListDto.seatPositionList.Count == table.AvailableSeatNumber)
					{
						break;
					}
				}
				// Debug.Log("Seat List: " + JsonUtility.ToJson(seatListDto));
				seatPositionList.tableList.Add(seatListDto);
				seatPositionList.tablePositionList.Add(table.transform.position);
			}

            seatPositionList.customerSpawnPosition = CustomerSpawner.Instance.transform.position;
            seatPositionList.playerPosition = Player.Instance.transform.position;

            string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { JsonUtility.ToJson(seatPositionList) } });
            // json = Utility.Socket.ToSocketJson(json);
            // Debug.Log("Seat Position List: " + json);

            Utility.Socket.EmitEvent("initGame", json);
        }
        void OnEnable()
        {
            Player.OnPlayerChangePosition += OnPlayerChangePositionHandler;
            foreach(Table table in _tableList)
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
            }catch
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
