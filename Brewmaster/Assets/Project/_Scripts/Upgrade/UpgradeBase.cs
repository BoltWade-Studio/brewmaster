using System;
using Newtonsoft.Json;
using NOOD;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Game
{
    public abstract class UpgradeBase : MonoBehaviour
    {
	    public Table _table;
        [HideInInspector] public int Price;
        [HideInInspector] public float PriceMultipler;

        [SerializeField] protected UpgradeUI _upgradeUI;
        [SerializeField] protected UpgradeAction _upgradeAction;
        protected int _upgradeTime = 1;
        public bool toUpgrade = false;

        #region Unity functions
        protected void Awake()
        {
            PriceMultipler = 2f;
            HideUI();
            ChildAwake();
        }
        protected virtual void ChildAwake(){}
        protected void OnEnable()
        {
            _upgradeUI.OnUpgradeButtonClick += OnUpgradeButtonClickHandler;
            if(GameplayManager.Instance)
            {
                GameplayManager.Instance.OnNextDay += HideUI;
            }
            if(UIManager.Instance)
            {
                UIManager.Instance.OnStorePhrase += OnStorePhraseHandler;
                UIManager.Instance.OnStorePhrase += OnStorePhaseHandler;
            }
            ChildOnEnable();
        }
        protected void Start()
        {
            if(GameplayManager.Instance)
            {
                GameplayManager.Instance.OnNextDay += HideUI;
            }
            if(UIManager.Instance)
            {
                UIManager.Instance.OnStorePhrase += OnStorePhraseHandler;
                UIManager.Instance.OnStorePhrase += OnStorePhaseHandler;
            }
            ChildStart();
            UpgradeManager.Instance.AddUpgradeBase(this);
        }

        private void Update()
        {
	        if(toUpgrade)
	        {
		        _upgradeAction.Invoke();
				toUpgrade = false;
			}
        }

        protected virtual void ChildStart(){}
        protected void OnDisable()
        {
            _upgradeUI.OnUpgradeButtonClick -= OnUpgradeButtonClickHandler;
            NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
            NoodyCustomCode.UnSubscribeAllEvent<UIManager>(this);
            ChildOnDisable();
        }
        protected virtual void ChildOnDisable(){}
        protected virtual void ChildOnEnable(){}
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
	        string json = JsonConvert.SerializeObject(new ArrayWrapper
	        {
		        array = new string[]
		        {
			        _table.TableIndex.ToString(),
			        JsonUtility.ToJson(_table.GetUpgradePosition())

		        }
	        });
	        Debug.Log("Upgrading Table " + json);
	        Utility.Socket.EmitEvent("upgradeTable", json);
        }

        protected virtual void OnStorePhaseHandler(){}

        /// <summary>
        /// Update price base on own setting of the child upgradeTime
        /// </summary>
        public virtual void UpdatePrice()
        {
            UpdateUpgradeTime();
            Price = (int) (_upgradeTime * PriceMultipler) + 50;
            Save();
        }

        public void OnStorePhraseHandler()
        {
            if(CheckAllUpgradeComplete() == false)
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
