using System;
using DG.Tweening;
using NOOD;
using NOOD.Sound;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{

	public class Customer : MonoBehaviour
	{
		#region Events
		public Action OnCustomerEnterSeat;
		public Action OnCustomerReturn;

		private bool toUpdateTime = false;
		public bool reachedDestination = false;
		public bool toReturn = false;
		public bool toDestroy = false;

		private bool toMove = false;
		private Vector3 newPosition;
		private Vector3 newRotation;
		private float rotateTime;
		#endregion

		#region Variable
		[Header("Component")]
		public int id;

		[SerializeField] private CustomerView _customerView;

		[Header("Waiting time")]
		[SerializeField] private WaitingUI _waitingUI;
		private float _maxWaitingTime;
		private float _waitTimer = 0;

		// private Transform _coinTrans;
		#endregion

		#region Unity functions
		void Start()
		{

		}
		void Update()
		{
			if (toMove)
			{
				this.transform.position = newPosition;
				this.transform.DOKill(); // To stop rotate
				this.transform.forward = Vector3.Lerp(this.transform.forward, newRotation, rotateTime);
				toMove = false;
			}
			if (toUpdateTime)
			{
				_waitingUI.UpdateWaitingUI(_waitTimer / _maxWaitingTime);
				toUpdateTime = false;
			}
			if (reachedDestination)
			{
				ReachDestination();
				reachedDestination = false;
			}
			if (toReturn)
			{
				Return();
				toReturn = false;
			}
			if (toDestroy)
			{
				DestroySelf();
				toDestroy = false;
			}
		}

		#endregion

		#region Event functions
		private void ReachDestination()
		{
			this.transform.DORotate(Vector3.zero, 0.5f);
			OnCustomerEnterSeat?.Invoke();
		}

		private void Return()
		{
			OnCustomerReturn?.Invoke();
		}
		public void UpdateWaitTimer(float currentTime, float maxTime)
		{
			_waitTimer = currentTime;
			_maxWaitingTime = maxTime;
			// Debug.Log("UpdateWaitTimer: " + currentTime + " " + maxTime);
			toUpdateTime = true;
		}
		public void MoveTo(Vector3 position, Vector3 rotation, float time)
		{
			newPosition = position;
			newRotation = rotation;
			rotateTime = time;
			toMove = true;
		}

		public void Complete(int money)
		{
			TextPopup.Show("+" + money, this.transform.position, Color.yellow);
			// MoneyManager.Instance.AddMoney(money);
			BeerServeManager.Instance.OnServeComplete?.Invoke(this);
		}
		#endregion

		#region Private functions
		private void DestroySelf()
		{
			_customerView.StopAllAnimation();
			Destroy(this.gameObject, 1f);
		}
		#endregion
	}
}
