using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Extension;
using Newtonsoft.Json;
using NOOD;
using NOOD.Data;
using NOOD.NoodCamera;
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

        [Header("Build Tables")]
        [SerializeField] private Transform _startPosition;
        [SerializeField] private float distanceBetweenTables = 10f;
        [SerializeField] private int tableCount = 10;
        [SerializeField] private GameObject _tablePrefab;
        [SerializeField] private GameObject _parent;

        #region Unity Functions
        protected override void ChildAwake()
        {
            base.ChildAwake();
            foreach (Table table in _tableList)
            {
                table.SetIsUnlockAllSeats(_unlockAllSeats);
            }
        }
        async void Start()
        {
            Utility.Socket.SubscribeEvent(SocketEnum.updateTablePositionCallback.ToString(), this.gameObject.name, nameof(UpdateTablePositionCallback), UpdateTablePositionCallback);
            Utility.Socket.SubscribeEvent(SocketEnum.updateSeatPositionsCallback.ToString(), this.gameObject.name, nameof(UpdateSeatPositionCallback), UpdateSeatPositionCallback);

            if (GameplayManager.Instance.IsMainMenu == false)
            {
                LoadTableAndSeat(); // When start game from main menu
                GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler; // When start game from next day
                await UniTask.WaitUntil(() => _tableList.Count == PlayerData.PlayerScale.Count());
            }
            UpdatePositionsToServer();
        }
        void OnEnable()
        {
            Player.OnPlayerChangePosition += OnPlayerChangePositionHandler;
        }
        void OnDisable()
        {
            Player.OnPlayerChangePosition -= OnPlayerChangePositionHandler;
        }
        void OnDestroy()
        {
            GameEvent.Instance.OnLoadDataSuccess -= OnLoadDataSuccessHandler;
        }
        #endregion

        #region Update pos to server
        public void UpdatePositionsToServer()
        {
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

        private void OnLoadDataSuccessHandler()
        {
            LoadTableAndSeat();
            UpdatePositionsToServer();
        }
        private void LoadTableAndSeat()
        {
            BuildTables(PlayerData.PlayerScale.Count());
            for (int i = 0; i < PlayerData.PlayerScale.Count(); i++)
            {
                int index = i;
                Table table = _tableList[index];
                Scale scale = PlayerData.PlayerScale[index];
                if (GameplayManager.Instance.IsMainMenu == false)
                {
                    table.TableIndex = scale.TableIndex;
                    table.AvailableSeatNumber = scale.Stools;
                }
                table.LoadSeat();
            }
            // foreach (Scale scale in PlayerData.PlayerScale)
            // {
            //     Debug.Log("Scale: " + scale);
            // }
        }
        private void OnPlayerChangePositionHandler(int index)
        {
            _currentTableIndex = index;
        }

        public List<Table> GetTableList()
        {
            return _tableList;
        }


        private void BuildTables(int tableCount = 10)
        {
            for (int i = 0; i < _tableList.Count; i++)
            {
                Destroy(_tableList[i].gameObject);
            }
            UpgradeManager.Instance.ClearSeatUpgrade(); // Clear Seat Upgrade to avoid duplicate, although OnDestroy is called when destroy table, but it's too late

            _tableList.Clear();
            Vector3 currentPosition = _startPosition.position;
            for (int i = 0; i < tableCount; i++)
            {
                GameObject table = Instantiate(_tablePrefab, currentPosition, Quaternion.identity, this.transform);
                _tableList.Add(table.GetComponent<Table>());
                table.transform.parent = _parent.transform;
                table.name = "Table_" + i;
                currentPosition += Vector3.back * distanceBetweenTables;
            }

            CameraFollow.Instance.TopBorder = _tableList[2].transform;
        }

        public Vector3 GetNextAddTablePosition()
        {
            Vector3 nextPos = _tableList[^1].transform.position;
            nextPos += Vector3.back * distanceBetweenTables;
            return nextPos;
        }

        public void BuildNewTable()
        {
            GameObject table = Instantiate(_tablePrefab, GetNextAddTablePosition(), Quaternion.identity, this.transform);
            Table newTable = table.GetComponent<Table>();
            newTable.TableIndex = _tableList.Count;
            _tableList.Add(newTable);
            table.transform.parent = _parent.transform;
        }
    }

}
