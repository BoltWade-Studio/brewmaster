using System;
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
        private bool _canUpgrade;
        private bool _isGettingData;

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
            ChildOnEnable();
        }
        protected void Start()
        {
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
            NoodyCustomCode.UnSubscribeAllEvent(GameEvent.Instance, this);
            ChildOnDisable();
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

        private async void PerformUpgrade()
        {
            string json = Utility.Socket.StringToSocketJson(_table.TableIndex.ToString());
            Debug.Log("Upgrading Table " + _table.TableIndex + " with availableSeat: " + _table.AvailableSeatNumber);

            LoadingUIManager.Instance.Show("Checking condition");
            Utility.Socket.OnEvent(SocketEnum.getCanUpgradeTableCallback.ToString(), this.gameObject.name, nameof(CanUpgradeTableCallback), CanUpgradeTableCallback);
            Utility.Socket.EmitEvent("getCanUpgradeTable", json);
            await UniTask.WaitUntil(() => _isGettingData == false);
            Debug.Log("_isGettingData: " + _isGettingData);
            LoadingUIManager.Instance.Hide();
            if (_canUpgrade)
            {
                bool result = await TransactionManager.Instance.AddStool(_table.TableIndex);
                if (result)
                {
                    LoadingUIManager.Instance.Show("Upgrading");
                    Utility.Socket.EmitEvent(SocketEnum.upgradeTable.ToString(), json);
                }
            }
            else
            {
                NotifyManager.Instance.Show("Don't have enough money");
            }
        }

        private async void CanUpgradeTableCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            try
            {
                _canUpgrade = data.Equals("True");

                if (_canUpgrade)
                {
                    bool result = true;
                    Debug.Log("AddStool to table: " + _table.TableIndex + " with availableSeat: " + _table.AvailableSeatNumber);
                    if (Application.isEditor == false)
                    {
                        result = await TransactionManager.Instance.AddStool(_table.TableIndex);
                        LoadingUIManager.Instance.ChangeLoadingMessage("Wait for updating transaction");
                        await UniTask.WaitForSeconds(30f);
                    }
                    if (result == false)
                    {
                        LoadingUIManager.Instance.Hide();
                        NotifyManager.Instance.Show("Player cancel upgrade");
                    }
                    else
                    {
                        Upgrade();
                        await DataSaveLoadManager.Instance.LoadData();
                    }
                }
                else
                {
                    LoadingUIManager.Instance.Hide();
                    NotifyManager.Instance.Show("Don't have enough money");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("CanUpgradeCallback error: " + e.Message);
            }

            LoadingUIManager.Instance.Hide();
        }

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
