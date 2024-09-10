using System.Collections.Generic;
using UnityEngine;
using Game.Extension;
using NOOD;
using System.Linq;
using UnityEngine.Serialization;

namespace Game
{
    public class Table : MonoBehaviour
    {
        // [SerializeField] private List<Transform> 
        [FormerlySerializedAs("_availableSeats")]
        [SerializeField] private List<Transform> _allSeats = new List<Transform>();
        private List<Transform> _availableSeatList = new List<Transform>();

        private Stack<Transform> _lockSeatList = new Stack<Transform>();
        private Dictionary<Customer, Transform> _unAvailableSeatDic = new Dictionary<Customer, Transform>();
        private bool _unlockAllSeats;

        public int AvailableSeatNumber = 1;
        public int TableIndex;
        public List<Transform> AllSeats => _allSeats;

        #region Unity Functions
        private void Start()
        {
            _availableSeatList = _allSeats;
            while (_availableSeatList.Count > AvailableSeatNumber && _unlockAllSeats == false)
            {
                // Deactivate all seats but 1
                Transform seat = _availableSeatList.Last();
                seat.gameObject.SetActive(false);
                _lockSeatList.Push(seat);
                _availableSeatList.Remove(seat);
            }
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
            _availableSeatList.Add(seat);
            TableManager.Instance.OnTableUpgrade?.Invoke(this);
        }
        public bool IsAvailable()
        {
            return _availableSeatList.Count > 0;
        }

        public Transform GetSeatForCustomer(Customer customer)
        {
            if (IsAvailable() == false) return null;
            Transform seat = _availableSeatList[Random.Range(0, _availableSeatList.Count - 1)];
            _availableSeatList.Remove(seat);
            _unAvailableSeatDic.Add(customer, seat);
            return seat;
        }

        public List<Transform> GetAllSeats()
        {
            // Return all seats including locked seats and available seats
            List<Transform> allSeats = new List<Transform>();
            allSeats.AddRange(_availableSeatList);
            // allSeats.AddRange(_lockSeatList);
            return allSeats;
        }
    }

}
