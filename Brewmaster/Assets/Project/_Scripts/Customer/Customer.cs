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
		#endregion

		#region Variable
		[Header("Component")]
		public int id;

		private Vector3 newPosition;
		private Vector3 newRotation;
		private float rotateTime;
		private bool toMove = false;
		[SerializeField] private CustomerView _customerView;

		[Header("Waiting time")]
		[SerializeField] private WaitingUI _waitingUI;
		private float _maxWaitingTime;

		private bool toUpdateTime = false;
		public bool reachedDestination = false;
		public bool toReturn = false;
		public bool toDestroy = false;
		// [SerializeField] private float _waitLossSpeed = 1f;
		//
		private float _waitTimer = 0;

		private Transform _coinTrans;
		#endregion

		#region Unity functions
		void OnEnable()
		{
			GameplayManager.Instance.OnEndDay += OnEndDayHandler;
		}
		void OnDisable()
		{
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
		}
		void Start()
		{
			// _isReturnCalled = false;

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
			// Move();
			// if (_isWaiting)
			// {
			// 	UpdateWaitTimer();
			// }
		}
		private void OnTriggerEnter(Collider other)
		{
			// if (other.transform.TryGetComponent<BeerCup>(out BeerCup beerCup) && _isServed == false && _isRequestBeer == true)
			// {
			// 	Destroy(other.gameObject);
			// 	Complete();
			// 	if (_coinTrans != null)
			// 	{
			// 		// Trigger event to get reward coin
			// 		Utility.Socket.EmitEvent("coinCollect");
			// 		// if (Application.isEditor)
			// 		// 	SocketConnectManager.Instance.EmitEvent("coinCollect");
			// 		// else
			// 		// 	JsSocketConnect.EmitEvent("coinCollect");
			//
			// 		_coinTrans.transform.DOMoveY(_coinTrans.transform.position.y + 1f, 0.5f);
			// 		_coinTrans.transform.DOScale(0, 0.4f);
			// 		NoodyCustomCode.StartDelayFunction(() => Destroy(_coinTrans.gameObject), 0.5f);
			// 	}
			// }
		}
		#endregion

		#region Event functions
		private void OnEndDayHandler()
		{
			Return();
		}
		#endregion

		#region Private functions
		private void Move()
		{
			// float distance = Vector3.Distance(this.transform.position, _targetSeat);
			// if (distance > 0.1f)
			// {
			// 	Vector3 direction = _targetSeat - this.transform.position;
			// 	direction = Vector3.Normalize(direction);
			// 	this.transform.position += direction * _speed * TimeManager.DeltaTime;
			// 	this.transform.DOKill(); // To stop rotate
			// 	this.transform.forward = Vector3.Lerp(this.transform.forward, direction, TimeManager.DeltaTime * _rotateSpeed);
			// }
			// else
			// {
			// 	// Get to seat
			// 	if (!_isRequestBeer)
			// 	{
			// 		this.transform.DORotate(Vector3.zero, 0.5f);
			// 		TableManager.Instance.RequestBeer(this);
			// 		OnCustomerEnterSeat?.Invoke();
			// 		_isRequestBeer = true;
			// 		_isWaiting = true;
			// 	}
			// }
		}
		public void UpdateWaitTimer(float currentTime, float maxTime)
		{
			_waitTimer = currentTime;
			_maxWaitingTime = maxTime;
			// Debug.Log("UpdateWaitTimer: " + currentTime + " " + maxTime);
			toUpdateTime = true;
			// if (_isWaiting)
			// {
				// _waitTimer += TimeManager.DeltaTime * _waitLossSpeed;
				// _waitingUI.UpdateWaitingUI(currentTime / maxTime);
			// }
			// if (_waitTimer >= _maxWaitingTime)
			// {
			// 	Return(); // Return and no money
			// }
		}
		public void Return()
		{
			// if (_isReturnCalled == true) return;
			//
			// _isReturnCalled = true;
			// _isServed = true;
			// TableManager.Instance.CustomerComplete(this); // Return seat
			// SetTargetPosition(CustomerSpawner.Instance.transform.position); // Move out
			OnCustomerReturn?.Invoke();
			// NoodyCustomCode.StartUpdater(this.gameObject, () =>
			// {
			// 	float distance = Vector3.Distance(this.transform.position, _targetSeat);
			// 	if (distance <= 0.1f)
			// 	{
			// 		DestroySelf();
			// 		return true;
			// 	}
			// 	return false;
			// });
		}
		public void DestroySelf()
		{
			_customerView.StopAllAnimation();
			Destroy(this.gameObject, 1f);
		}
		#endregion

		#region Public functions
		// public void SetTargetPosition(Vector3 position)
		// {
		// 	// _targetSeat = position;
		// }

		private void ReachDestination()
		{
			this.transform.DORotate(Vector3.zero, 0.5f);
			// TableManager.Instance.RequestBeer(this);
			OnCustomerEnterSeat?.Invoke();
		}

		public void MoveTo(Vector3 position, Vector3 rotation, float time)
		{
			// Debug.Log("MoveTo: " + position);
			newPosition = position;
			newRotation = rotation;
			rotateTime = time;
			toMove = true;
		}

		public void Complete(int _money)
		{
			// Return();
			// Pay money
			TextPopup.Show("+" + _money, this.transform.position, Color.yellow);
			MoneyManager.Instance.AddMoney(_money);
			BeerServeManager.Instance.OnServeComplete?.Invoke(this);
		}
		public void SetCoin(Transform coin)
		{
			_coinTrans = coin;
		}

		public bool HaveCoin()
		{
			return _coinTrans != null;
		}

		public void CollectCoin()
		{
			_coinTrans.transform.DOMoveY(_coinTrans.transform.position.y + 1f, 0.5f);
			_coinTrans.transform.DOScale(0, 0.4f);
			NoodyCustomCode.StartDelayFunction(() => Destroy(_coinTrans.gameObject), 0.5f);
		}

		#endregion
	}
}
