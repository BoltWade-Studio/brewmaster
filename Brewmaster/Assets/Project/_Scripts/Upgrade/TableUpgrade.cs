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
		    _upgradeAction = new UpgradeAction(() =>
		    {
			    Debug.Log("Before Add table");
			    TableManager.Instance.BuildNewTable();
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
		    _table.TableIndex = TableManager.Instance.GetTableList().Count - 1;
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
    }
}
