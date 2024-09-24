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
        [SerializeField] private List<Transform> _allSeats = new List<Transform>();
        private List<Transform> _availableSeatList = new List<Transform>();

        private Stack<Transform> _lockSeatList = new Stack<Transform>();
        private bool _unlockAllSeats;

        public int AvailableSeatNumber = 1;
        public int TableIndex { get; set; }
        public List<Transform> AllSeats => _allSeats;

        #region Unity Functions
        private void Awake()
        {
            _availableSeatList = new List<Transform>();
            _lockSeatList = new Stack<Transform>();
            foreach (var seat in _allSeats)
            {
                _availableSeatList.Add(seat);
            }
        }
        private void OnDestroy()
        {
        }
        #endregion

        #region Upgrade Functions
        public Vector3 GetNextUpgradePosition()
        {
            return _lockSeatList.ToArray()[0].position.ToVector3XZ();
        }
        #endregion

        #region Event functions
        #endregion

        #region In game functions
        public void LoadSeat()
        {
            _availableSeatList.Clear();
            foreach (Transform seat in _allSeats)
            {
                // Make a copy of all seat
                _availableSeatList.Add(seat);
            }

            Debug.Log("Table index: " + TableIndex + " Available Seat: " + AvailableSeatNumber);
            Debug.Log("Available Seat List: " + _availableSeatList.Count);
            while (_availableSeatList.Count != AvailableSeatNumber)
            {
                Debug.Log("Available Seat: " + _availableSeatList.Count);
                if (_availableSeatList.Count > AvailableSeatNumber)
                {
                    // Deactivate all seats > availableSeatNumber
                    Transform seat = _availableSeatList.Last();
                    seat.gameObject.SetActive(false);
                    _lockSeatList.Push(seat);
                    _availableSeatList.Remove(seat);
                }
                else if (_availableSeatList.Count < AvailableSeatNumber)
                {
                    // unlock all seats <= availableSeatNumber
                    Transform seat = _lockSeatList.Pop();
                    seat.gameObject.SetActive(true);
                    _availableSeatList.Add(seat);
                }
            }
        }
        public void SetIsUnlockAllSeats(bool value)
        {
            _unlockAllSeats = value;
        }
        #endregion

        public void UnlockSeat()
        {
            Debug.Log("Unlock seat at table: " + TableIndex);
            Transform seat = _lockSeatList.Pop();
            seat.gameObject.SetActive(true);
            _availableSeatList.Add(seat);
            TableManager.Instance.OnTableUpgrade?.Invoke(this);
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
