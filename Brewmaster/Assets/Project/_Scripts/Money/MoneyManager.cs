using NOOD;
using UnityEngine;

namespace Game
{
    public class MoneyManager : MonoBehaviorInstance<MoneyManager>
    {
        [SerializeField] private int _bonus = 40;
        public int CurrentTarget => _currentTarget;
        public int NextTarget => _currentTarget + _bonus;
        public int CurrentTotalMoney => _totalMoney;
        public int Bonus => _bonus;
        
        private int _totalMoney;
        private int _currentTarget;

        protected override void ChildAwake()
        {
            _currentTarget = 150;
        }

        void Start()
        {
            _currentTarget = DataSaveLoadManager.Instance.Target;
            _totalMoney = DataSaveLoadManager.Instance.Money;
            GameplayManager.Instance.OnNextDay += GameplayManager_OnNextDayHandler;
        }

        void OnDisable()
        {
            NoodyCustomCode.UnSubscribeAllEvent(GameplayManager.Instance, this);
        }

        private void GameplayManager_OnNextDayHandler()
        {
            _currentTarget = NextTarget;
            Debug.Log("Target: " + _currentTarget);
        }

        public void CommitEndDay()
        {
            _totalMoney -= CurrentTarget;
        }

        public bool PayMoney(int amount)
        {
            if (_totalMoney >= amount)
            {
                _totalMoney -= amount;
                UIManager.Instance.UpdateInDayMoney();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// If buying stuff, use PayMoney(int) instead
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveMoney(int amount)
        {
            _totalMoney -= amount;
            UIManager.Instance.UpdateInDayMoney();
        }
        public void AddMoney(int amount)
        {
            _totalMoney += amount;
            UIManager.Instance.UpdateInDayMoney();
        }
    }

}
