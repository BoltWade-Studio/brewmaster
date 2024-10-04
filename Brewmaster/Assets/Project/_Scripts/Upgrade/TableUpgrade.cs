using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class TableUpgrade : UpgradeBase
    {
	    #region Unity functions
	    protected override void ChildAwake()
	    {
		    _table = gameObject.AddComponent<Table>();
		    _table.TableIndex = TableManager.Instance.GetTableList().Count;
		    _table.name = "Table " + _table.TableIndex;
		    _upgradeAction = new UpgradeAction(async ()  =>
		    {
			    try
			    {
				    // await DataSaveLoadManager.Instance.LoadData();
				    // GameEvent.Instance.OnStorePhaseEnd?.Invoke();
				    // GameEvent.Instance.OnStorePhase?.Invoke();
				    GameEvent.Instance.OnStorePhase?.Invoke();
				    TableManager.Instance.UpdatePositionsToServer();
			    }
			    catch (System.Exception e)
			    {
				    Debug.Log("Invoke store phase Error: " + e);
			    }
			    Debug.Log("Invoke store phase");


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
		    _table.TableIndex = TableManager.Instance.GetTableList().Count - 1;
		    _table.name = "Table " + _table.TableIndex;
		    if (CheckAllUpgradeComplete())
		    {
			    HideUI();
		    }
		    else
		    {
			    MoveToNewPos();
		    }
	    }
	    protected override void OnStorePhaseHandler()
	    {
		    OnUpgradeCompleteHandler();
	    }

	    private void MoveToNewPos()
	    {
		    Vector3 tempPos = TableManager.Instance.GetNextAddTablePosition();
		    Vector3 originalPosition = _upgradeUI.gameObject.transform.position;
		    Vector3 newPos = new Vector3(originalPosition.x, originalPosition.y, tempPos.z);
		    _upgradeUI.SetPosition(newPos);
	    }

        protected override void Save()
        {
	        throw new System.NotImplementedException();
        }

        protected override void Load()
        {
	        _upgradeTime = TableManager.Instance.GetTableList().Count;
        }

        protected override string GetId()
        {
	        string Id = typeof(TableUpgrade).ToString() + _table.TableIndex;
	        return Id;
        }

        protected override bool CheckAllUpgradeComplete()
        {
	        if (_table.TableIndex == 10)
	        {
		        return true;
	        }
	        else return false;
        }

        protected override void UpdateUpgradeTime()
        {
	        throw new System.NotImplementedException();
        }

        protected override UniTask<bool> Execute()
        {
	        return TransactionManager.Instance.AddTable();
        }

        protected override async void PerformUpgrade()
        {
	        LoadingUIManager.Instance.Show("Checking upgrade data");
	        string json = Utility.Socket.StringToSocketJson(_table.TableIndex.ToString());
	        Debug.Log("Adding Table " + _table.TableIndex );

	        _isGettingData = true;
	        if (_transactionJsonDataDic.ContainsKey("CanUpgradeTable"))
		        _transactionJsonDataDic.Remove("CanUpgradeTable");
	        Utility.Socket.SubscribeEvent(SocketEnum.getCanAddTableCallback.ToString(), this.gameObject.name, nameof(CanUpgradeTableCallback), CanUpgradeTableCallback);
	        Utility.Socket.EmitEvent(SocketEnum.getCanAddTable.ToString(), json);
	        await UniTask.WaitUntil(() => _isGettingData == false);
	        await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey("CanUpgradeTable"));

	        bool canUpgrade = _transactionJsonDataDic["CanUpgradeTable"].ToString().ToLower().Equals("true");

	        if (canUpgrade == false)
	        {
		        LoadingUIManager.Instance.Hide();
		        NotifyManager.Instance.Show("Don't have enough money or table is not available to open new table");
		        return;
	        }
	        else
	        {
		        await TransactionUpgrade();
		        // LoadingUIManager.Instance.Hide();
	        }
	        Utility.Socket.UnSubscribeEvent(SocketEnum.getCanAddTableCallback.ToString(), this.gameObject.name, nameof(CanUpgradeTableCallback), CanUpgradeTableCallback);
        }
    }
}
