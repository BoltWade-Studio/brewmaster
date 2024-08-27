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
		[SerializeField] private CustomerView _customerView;

		[Header("Waiting time")]
		[SerializeField] private WaitingUI _waitingUI;
		[SerializeField] private float _maxWaitingTime;
		[SerializeField] private float _waitLossSpeed = 1f;
		[SerializeField] private int _money = 5;

		private float _waitTimer = 0;
		private bool _isWaiting;

		private Transform _coinTrans;
		private Vector3 _targetSeat;
		private float _speed = 4f;
		private float _rotateSpeed = 10;
		private bool _isRequestBeer;
		private bool _isServed;
		private bool _isReturnCalled;
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
			_isReturnCalled = false;
		}
		private void Update()
		{
			Move();
			if (_isWaiting)
			{
				UpdateWaitTimer();
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.TryGetComponent<BeerCup>(out BeerCup beerCup) && _isServed == false && _isRequestBeer == true)
			{
				Destroy(other.gameObject);
				Complete();
				if (_coinTrans != null)
				{
					// Trigger event to get reward coin
					_coinTrans.transform.DOMoveY(_coinTrans.transform.position.y + 1f, 0.5f);
					_coinTrans.transform.DOScale(0, 0.4f);
					NoodyCustomCode.StartDelayFunction(() => Destroy(_coinTrans.gameObject), 0.5f);
				}
			}
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
			float distance = Vector3.Distance(this.transform.position, _targetSeat);
			if (distance > 0.1f)
			{
				Vector3 direction = _targetSeat - this.transform.position;
				direction = Vector3.Normalize(direction);
				this.transform.position += direction * _speed * TimeManager.DeltaTime;
				this.transform.DOKill(); // To stop rotate
				this.transform.forward = Vector3.Lerp(this.transform.forward, direction, TimeManager.DeltaTime * _rotateSpeed);
			}
			else
			{
				// Get to seat
				if (!_isRequestBeer)
				{
					this.transform.DORotate(Vector3.zero, 0.5f);
					TableManager.Instance.RequestBeer(this);
					OnCustomerEnterSeat?.Invoke();
					_isRequestBeer = true;
					_isWaiting = true;
				}
			}
		}
		private void UpdateWaitTimer()
		{
			if (_isWaiting)
			{
				_waitTimer += TimeManager.DeltaTime * _waitLossSpeed;
				_waitingUI.UpdateWaitingUI(_waitTimer / _maxWaitingTime);
			}
			if (_waitTimer >= _maxWaitingTime)
			{
				Return(); // Return and no money
			}
		}
		private void Return()
		{
			if (_isReturnCalled == true) return;

			_isReturnCalled = true;
			_isServed = true;
			TableManager.Instance.CustomerComplete(this); // Return seat
			SetTargetPosition(CustomerSpawner.Instance.transform.position); // Move out
			OnCustomerReturn?.Invoke();
			NoodyCustomCode.StartUpdater(this.gameObject, () =>
			{
				float distance = Vector3.Distance(this.transform.position, _targetSeat);
				if (distance <= 0.1f)
				{
					DestroySelf();
					return true;
				}
				return false;
			});
		}
		private void DestroySelf()
		{
			_customerView.StopAllAnimation();
			Destroy(this.gameObject, 1f);
		}
		#endregion

		#region Public functions
		public void SetTargetPosition(Vector3 position)
		{
			_targetSeat = position;
		}
		public void Complete()
		{
			Return();
			// Pay money
			TextPopup.Show("+" + _money, this.transform.position, Color.yellow);
			MoneyManager.Instance.AddMoney(_money);
			BeerServeManager.Instance.OnServeComplete?.Invoke(this);
			_isWaiting = false;
		}
		public void SetCoin(Transform coin)
		{
			_coinTrans = coin;
		}
		#endregion
	}
}
