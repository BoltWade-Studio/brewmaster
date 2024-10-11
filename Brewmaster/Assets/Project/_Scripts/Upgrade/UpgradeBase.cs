using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Game
{
    public abstract class UpgradeBase : MonoBehaviour
    {
        [HideInInspector] public Table _table;
        [HideInInspector] public int Price;

        [SerializeField] private int _defaultPrice = 50;

        [SerializeField] protected UpgradeUI _upgradeUI;
        [SerializeField] protected UpgradeAction _upgradeAction;
        protected int _upgradeTime = 1;
        protected bool _canUpgrade;
        protected bool _isGettingData;

        protected Dictionary<string, object> _transactionJsonDataDic = new Dictionary<string, object>();

        #region Unity functions
        protected void Awake()
        {
            HideUI();
            ChildAwake();
            Price = _defaultPrice;
        }
        protected virtual void ChildAwake() { }
        protected void OnEnable()
        {
            _upgradeUI.OnUpgradeButtonClick += OnUpgradeButtonClickHandler;
            GameEvent.Instance.OnNextDay += HideUI;
            GameEvent.Instance.OnStorePhase += OnStorePhraseHandler;
            GameEvent.Instance.OnStorePhase += OnStorePhaseHandler;
            GameEvent.Instance.OnStorePhaseEnd += OnStorePhaseEndHandler;
            ChildOnEnable();
        }
        protected void Start()
        {
            Debug.Log("UpgradeBase Started");
            if (UpgradeManager.Instance)
            {
                UpgradeManager.Instance.AddUpgradeBase(this);
            }

            ChildStart();
        }

        protected virtual void ChildStart() { }
        protected void OnDisable()
        {
            _upgradeUI.OnUpgradeButtonClick -= OnUpgradeButtonClickHandler;
            GameEvent.Instance.OnNextDay -= HideUI;
            GameEvent.Instance.OnStorePhase -= OnStorePhraseHandler;
            GameEvent.Instance.OnStorePhase -= OnStorePhaseHandler;
            GameEvent.Instance.OnStorePhaseEnd -= OnStorePhaseEndHandler;
            ChildOnDisable();
        }

        private void OnDestroy()
        {
            //       Debug.Log("UpgradeBase OnDestroy");
            //       if (UpgradeManager.Instance)
            // {
            // 	UpgradeManager.Instance.RemoveUpgradeBase(this);
            // }
        }

        protected virtual void ChildOnDisable() { }
        protected virtual void ChildOnEnable() { }
        #endregion

        #region Abstract functions
        protected abstract void Save();
        protected abstract void Load();
        protected abstract string GetId();
        protected abstract bool CheckAllUpgradeComplete();
        /// <summary>
        /// This function is inherited by child to b/c it is set by child
        /// </summary>
        protected abstract void UpdateUpgradeTime();
        #endregion

        private void OnUpgradeButtonClickHandler()
        {
            PerformUpgrade();
        }

        #region Upgrade

        protected virtual void PerformUpgrade()
        {
            if (_transactionJsonDataDic.ContainsKey("CanUpgradeTable"))
                _transactionJsonDataDic.Remove("CanUpgradeTable");
        }
        protected async void CanUpgradeTableCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            _transactionJsonDataDic.Add("CanUpgradeTable", data);

            _isGettingData = false;
        }

        protected abstract UniTask<bool> Execute();

        protected async UniTask TransactionUpgrade()
        {
            bool isPlayerConfirm = true;
            Debug.Log("AddStool to table: " + _table.TableIndex + " with availableSeat: " + _table.AvailableSeatNumber);
            if (Application.isEditor == false)
            {
                // Send transaction to blockchain and wait for result
                LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for player confirmation");
                isPlayerConfirm = await Execute();
            }

            if (isPlayerConfirm == true)
            {
                // Transaction is successful, load data
                LoadingUIManager.Instance.ChangeLoadingMessage("Getting player data");
                await DataSaveLoadManager.Instance.LoadData(); // Load data from blockchain an update in server
                Upgrade(); // Upgrade in local only
                LoadingUIManager.Instance.Hide();
            }
            else
            {
                // Transaction is failed, hide loading and notify
                LoadingUIManager.Instance.Hide();
                NotifyManager.Instance.Show("Transaction failed or user abort");
            }
        }
        #endregion

        protected virtual void OnStorePhaseHandler() { }

        public virtual void Upgrade()
        {
            _upgradeAction.Invoke();
            _upgradeUI.UpdateMoneyText();
        }

        /// <summary>
        /// Update price base on own setting of the child upgradeTime
        /// </summary>
        public virtual void UpdatePrice()
        {
            _upgradeUI.gameObject.SetActive(true);
            _upgradeUI.UpdateMoneyText();
        }

        public void OnStorePhaseEndHandler()
        {
            HideUI();
        }

        public void OnStorePhraseHandler()
        {
            if (CheckAllUpgradeComplete() == false)
                ShowUI();
        }

        public void HideUI()
        {
            _upgradeUI.gameObject.SetActive(false);
        }
        private void ShowUI()
        {
            UpdatePrice();
            _upgradeUI.gameObject.SetActive(true);
            _upgradeUI.UpdateMoneyText();
        }
    }
}
