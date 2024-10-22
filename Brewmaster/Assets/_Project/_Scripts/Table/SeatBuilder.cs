using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class SeatBuilder : MonoBehaviour
	{
		[SerializeField] private GameObject _seatPrefab;
		[SerializeField] private Transform _beginPoint;
		[SerializeField] private Transform _endPoint;
		[SerializeField] private int _seatCount;
		private Table _table;

		private void Awake()
		{
			_table = GetComponent<Table>();
			BuildSeat();
		}

		private void BuildSeat()
		{
			Vector3 beginPoint = _beginPoint.position;
			Vector3 endPoint = _endPoint.position;
			Vector3 direction = (endPoint - beginPoint).normalized;
			float distance = Vector3.Distance(beginPoint, endPoint);
			float step = distance / (_seatCount - 1);
			Vector3 currentPosition = beginPoint;
			List<Transform> allSeats = new List<Transform>();
			for (int i = 0; i < _seatCount; i++)
			{
				GameObject seat = Instantiate(_seatPrefab, currentPosition, Quaternion.identity, this.transform);
				seat.transform.parent = this.transform;
				currentPosition += direction * step;
				allSeats.Add(seat.transform);
			}

			for (int i = 0; i < _seatCount / 2; i++)
			{
				_table.AllSeats.Add(allSeats[2 * i]);
			}

			for (int i = 0; i < _seatCount / 2; i++)
			{
				_table.AllSeats.Add(allSeats[2 * i + 1]);
			}

			_beginPoint.gameObject.SetActive(false);
			_endPoint.gameObject.SetActive(false);
		}
	}
}
