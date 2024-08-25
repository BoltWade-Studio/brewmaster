using NOOD.Data;
using UnityEngine;

namespace Game
{
    public class SeatUpgrade : UpgradeBase
    {
        private Table _table;

        protected override void ChildAwake()
        {
            _table = this.GetComponent<Table>();
            _upgradeAction = new UpgradeAction(() =>
            {
                _table.UnlockSeat();
                _table.AvailableSeatNumber++;
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

        public void OnUpgradeCompleteHandler()
        {
            if(CheckAllUpgradeComplete())
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
            Vector3 tempPos = _table.GetUpgradePosition();
            Vector3 newPos = new Vector3(tempPos.x, this.transform.position.y, tempPos.z);
            _upgradeUI.SetPosition(newPos);
        }
        protected override bool CheckAllUpgradeComplete()
        {
            if (_table.AvailableSeatNumber == 4)
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
            DataManager<int>.SaveToPlayerPrefWithGenId(_upgradeTime, GetId());
        }
        protected override void Load()
        {
            _upgradeTime = DataManager<int>.LoadDataFromPlayerPref(GetId(), 1);
        }
        #endregion
    }

}
