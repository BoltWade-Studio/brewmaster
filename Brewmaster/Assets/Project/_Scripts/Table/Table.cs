using System.Collections.Generic;
using UnityEngine;
using Game.Extension;
using NOOD;
using System.Linq;

namespace Game
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private List<Transform> _availableSeats = new List<Transform>();

        public int AvailableSeatNumber = 1;
        private Stack<Transform> _lockSeatList = new Stack<Transform>();
        private Dictionary<Customer, Transform> _unAvailableSeatDic = new Dictionary<Customer, Transform>();
        private bool _unlockAllSeats;

        #region Unity Functions
        private void Start()
        {
            while(_availableSeats.Count > AvailableSeatNumber && _unlockAllSeats == false)
            {
                // Deactivate all seats but 1
                Transform seat = _availableSeats.Last();
                seat.gameObject.SetActive(false);
                _lockSeatList.Push(seat);
                _availableSeats.Remove(seat);
            }
            TableManager.Instance.OnCustomComplete += OnCustomerCompleteHandler;
        }
        private void OnDestroy()
        {
            NoodyCustomCode.UnSubscribeAllEvent<TableManager>(this);
            NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
            NoodyCustomCode.UnSubscribeAllEvent<UIManager>(this);
        }
        #endregion

        #region Upgrade Functions    
        public Vector3 GetUpgradePosition()
        {
            return _lockSeatList.ToArray()[0].position.ToVector3XZ();
        }
        #endregion

        #region Event functions
        #endregion

        #region In game functions
        public void SetIsUnlockAllSeats(bool value)
        {
            _unlockAllSeats = value;
        }
        #endregion

        public void UnlockSeat()
        {
            Transform seat = _lockSeatList.Pop();
            seat.gameObject.SetActive(true);
            _availableSeats.Add(seat);
            TableManager.Instance.OnTableUpgrade?.Invoke(this);
        }
        public void OnCustomerCompleteHandler(Customer customer)
        {
            if(_unAvailableSeatDic.ContainsKey(customer))
            {
                _availableSeats.Add(_unAvailableSeatDic[customer]);
                _unAvailableSeatDic.Remove(customer);
            }
        }
        public bool IsAvailable()
        {
            return _availableSeats.Count > 0;
        }

        public Transform GetSeatForCustomer(Customer customer)
        {
            if (IsAvailable() == false) return null;
            Transform seat = _availableSeats[Random.Range(0, _availableSeats.Count - 1)];
            _availableSeats.Remove(seat);
            _unAvailableSeatDic.Add(customer, seat);
            return seat;
        }
    }

}
