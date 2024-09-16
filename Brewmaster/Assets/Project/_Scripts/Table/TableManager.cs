using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        private bool _isTableUpdateComplete, _isSeatUpdateComplete;

        #region Unity Functions
        protected override void ChildAwake()
        {
            base.ChildAwake();
            foreach (Table table in _tableList)
            {
                table.SetIsUnlockAllSeats(_unlockAllSeats);
            }
        }
        void Start()
        {
            Utility.Socket.OnEvent(SocketEnum.updateTablePositionCallback.ToString(), this.gameObject.name, nameof(UpdateTablePositionCallback), UpdateTablePositionCallback);
            Utility.Socket.OnEvent(SocketEnum.updateSeatPositionsCallback.ToString(), this.gameObject.name, nameof(UpdateSeatPositionCallback), UpdateSeatPositionCallback);

            UpdatePositionsToServer();

            Debug.Log("AAA Update table manger start");

            try
            {
                // if (GameplayManager.Instance.IsMainMenu == false)
                // {
                //     _tableData = DataSaveLoadManager.Instance.TableData;
                // }
                Debug.Log("client table count: " + _tableList.Count);
                Debug.Log("server table count: " + PlayerData.PlayerScale.Count());
                for (int i = 0; i < _tableList.Count; i++)
                {
                    if (GameplayManager.Instance.IsMainMenu == false)
                    {
                        Debug.Log("index: " + i + " table index: " + PlayerData.PlayerScale[i].TableIndex);
                        _tableList[i].TableIndex = PlayerData.PlayerScale[i].TableIndex;
                        _tableList[i].AvailableSeatNumber = PlayerData.PlayerScale[i].Stools;
                        _tableList[i].LoadSeat();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("AAA error: " + e.Message);
            }

            OnTableUpgrade += OnTableUpgradeHandler;
        }
        void OnEnable()
        {
            Player.OnPlayerChangePosition += OnPlayerChangePositionHandler;
            // GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler;
        }
        void OnDisable()
        {
            Player.OnPlayerChangePosition -= OnPlayerChangePositionHandler;
            // GameEvent.Instance.OnLoadDataSuccess -= OnLoadDataSuccessHandler;
        }
        void OnDestroy()
        {
            OnTableUpgrade -= OnTableUpgradeHandler;
        }
        #endregion

        #region Update pos to server
        private void UpdatePositionsToServer()
        {
            Debug.Log("AAA Update position to server");
            string[] tablePosStrings = new string[_tableList.Count];
            string[] totalSeatPosArray = new string[_tableList.Count];
            for (int i = 0; i < _tableList.Count; i++)
            {
                tablePosStrings[i] = JsonConvert.SerializeObject(_tableList[i].transform.position);

                // Get Seats positions
                string[] tableSeatArray = new string[_tableList[i].AllSeats.Count];
                for (int j = 0; j < _tableList[i].AllSeats.Count; j++)
                {
                    tableSeatArray[j] = JsonConvert.SerializeObject(_tableList[i].AllSeats[j].position);
                }
                totalSeatPosArray[i] = JsonConvert.SerializeObject(tableSeatArray).RemoveUnWantChar();
            }


            // Update table pos to server
            string tablePosStr = JsonConvert.SerializeObject(tablePosStrings).RemoveUnWantChar();
            Utility.Socket.EmitEvent(SocketEnum.updateTablePositions.ToString(), tablePosStr);

            // Update seat pos to server
            string seatJson = JsonConvert.SerializeObject(totalSeatPosArray).RemoveUnWantChar();
            Utility.Socket.EmitEvent(SocketEnum.updateSeatPositions.ToString(), seatJson);
        }
        private async void UpdateTablePositionCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("Update table position callback: ");
            _isTableUpdateComplete = true;
            if (_isSeatUpdateComplete)
            {
                Debug.Log("OnComplete");
                GameEvent.Instance.OnUpdatePosComplete?.Invoke();
            }
        }
        private async void UpdateSeatPositionCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("Update seat position callback: ");
            _isSeatUpdateComplete = true;
            if (_isTableUpdateComplete)
            {
                Debug.Log("OnComplete");
                GameEvent.Instance.OnUpdatePosComplete?.Invoke();
            }
        }
        #endregion

        // private void OnLoadDataSuccessHandler()
        // {
        //     if (GameplayManager.Instance.IsMainMenu == false)
        //     {
        //         _tableData = DataSaveLoadManager.Instance.TableData;
        //     }
        //     for (int i = 0; i < _tableList.Count; i++)
        //     {
        //         if (GameplayManager.Instance.IsMainMenu == false)
        //         {
        //             _tableList[i].TableIndex = PlayerData.PlayerScale[i].TableIndex;
        //             _tableList[i].AvailableSeatNumber = PlayerData.PlayerScale[i].Stools;
        //             _tableList[i].LoadSeat();
        //         }
        //     }
        // }
        private void OnTableUpgradeHandler(Table table)
        {
            // int index = _tableList.IndexOf(table);
            // _tableData.SeatNumberList[index] += 1;
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
