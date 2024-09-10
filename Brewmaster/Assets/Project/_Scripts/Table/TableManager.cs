using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Extension;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public class InitializedGameDataDto
    {
        public List<SeatListDto> tableList = new List<SeatListDto>();
        public List<Vector3> tablePositionList = new List<Vector3>();
        public Vector3 customerSpawnPosition;
        public Vector3 playerPosition;
    }

    public class TableManager : MonoBehaviorInstance<TableManager>
    {
        public Action<Table> OnTableUpgrade;
        [SerializeField] private List<Table> _tableList = new List<Table>();
        [SerializeField] private bool _unlockAllSeats;
        private int _currentTableIndex = 0;
        private TableData _tableData;

        #region Unity Functions
        void Start()
        {
            _tableData = DataSaveLoadManager.Instance.TableData;
            string[] tablePosStrings = new string[_tableList.Count];
            string[] totalSeatArray = new string[_tableList.Count];
            for (int i = 0; i < _tableList.Count; i++)
            {
                string[] tableSeatArray = new string[_tableList[i].AllSeats.Count];
                if (i >= _tableData.SeatNumberList.Count)
                {
                    _tableData.SeatNumberList.Add(1);
                }
                _tableList[i].AvailableSeatNumber = _tableData.SeatNumberList[i];
                tablePosStrings[i] = JsonConvert.SerializeObject(_tableList[i].transform.position);

                for (int j = 0; j < _tableList[i].AllSeats.Count; j++)
                {
                    tableSeatArray[j] = JsonConvert.SerializeObject(_tableList[i].AllSeats[j].position);
                }
                totalSeatArray[i] = JsonConvert.SerializeObject(tableSeatArray).RemoveUnWantChar();
            }

            // Update table pos to server
            string tablePosStr = JsonConvert.SerializeObject(tablePosStrings).RemoveUnWantChar();
            Utility.Socket.EmitEvent(SocketEnum.updateTablePositions.ToString(), tablePosStr);

            // Update seat pos to server
            string seatJson = JsonConvert.SerializeObject(totalSeatArray).RemoveUnWantChar();
            Utility.Socket.EmitEvent(SocketEnum.updateSeatPositions.ToString(), seatJson);

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
    }

}
