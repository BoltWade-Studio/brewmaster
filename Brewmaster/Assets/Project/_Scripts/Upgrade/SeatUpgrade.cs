using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NOOD.Data;
using UnityEngine;

namespace Game
{
    public class SeatUpgrade : UpgradeBase
    {
        #region Unity functions
        protected override void ChildAwake()
        {
            _table = this.GetComponent<Table>();
            _upgradeAction = new UpgradeAction(() =>
            {
                Debug.Log("Before Unlock at table: " + _table.TableIndex);
                // _table.UnlockSeat();
                // _table.AvailableSeatNumber++;
                GameEvent.Instance.OnStorePhase?.Invoke();
            });
        }
        protected override void ChildStart()
        {
            Load();
        }
        protected override void ChildOnEnable()
        {
            base.ChildOnEnable();
            _upgradeAction.OnComplete += OnUpgradeCompleteHandler;
        }
        protected override void ChildOnDisable()
        {
            base.ChildOnDisable();
            _upgradeAction.OnComplete -= OnUpgradeCompleteHandler;
        }
        #endregion

        public void OnUpgradeCompleteHandler()
        {
            if (CheckAllUpgradeComplete())
            {
                HideUI();
            }
            else
            {
                MoveToNewPos();
            }
        }
        private void MoveToNewPos()
        {
            Vector3 tempPos = _table.GetNextUpgradePosition();
            Vector3 newPos = new Vector3(tempPos.x, this.transform.position.y, tempPos.z);
            _upgradeUI.SetPosition(newPos);
        }
        protected override bool CheckAllUpgradeComplete()
        {
            if (_table.AvailableSeatNumber == 10)
            {
                return true;
            }
            else return false;
        }
        protected override void OnStorePhaseHandler()
        {
            OnUpgradeCompleteHandler();
        }

        #region Abstract functions override
        protected override void UpdateUpgradeTime()
        {
            _upgradeTime = _table.AvailableSeatNumber;
        }
        protected override string GetId()
        {
            string Id = typeof(SeatUpgrade).ToString() + TableManager.Instance.GetTableList().IndexOf(_table);
            return Id;
        }
        protected override void Save()
        {
            // DataManager<int>.SaveToPlayerPrefWithGenId(_upgradeTime, GetId());
        }
        protected override void Load()
        {
            _upgradeTime = DataManager<int>.LoadDataFromPlayerPref(GetId(), 1);
        }

        protected override UniTask<bool> Execute()
        {
            return TransactionManager.Instance.AddStool(_table.TableIndex);
        }

        protected override async void PerformUpgrade()
        {
            base.PerformUpgrade();
            LoadingUIManager.Instance.Show("Checking upgrade data");
            string json = Utility.Socket.StringToSocketJson(_table.TableIndex.ToString());
            Debug.Log("Upgrading Table " + _table.TableIndex + " with availableSeat: " + _table.AvailableSeatNumber);

            _isGettingData = true;
            if (_transactionJsonDataDic.ContainsKey("CanUpgradeTable"))
                _transactionJsonDataDic.Remove("CanUpgradeTable");
            Utility.Socket.SubscribeEvent(SocketEnum.getCanUpgradeTableCallback.ToString(), this.gameObject.name, nameof(CanUpgradeTableCallback), CanUpgradeTableCallback);
            Utility.Socket.EmitEvent(SocketEnum.getCanUpgradeTable.ToString(), json);
            await UniTask.WaitUntil(() => _isGettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey("CanUpgradeTable"));

            bool canUpgrade = _transactionJsonDataDic["CanUpgradeTable"].ToString().ToLower().Equals("true");

            if (canUpgrade == false)
            {
                LoadingUIManager.Instance.Hide();
                NotifyManager.Instance.Show("Don't have enough money or table is not available to upgrade");
                return;
            }
            else
            {
                await TransactionUpgrade();
                // LoadingUIManager.Instance.Hide();
            }
            Utility.Socket.UnSubscribeEvent(SocketEnum.getCanUpgradeTableCallback.ToString(), this.gameObject.name, nameof(CanUpgradeTableCallback), CanUpgradeTableCallback);
        }

        #endregion
    }

}
