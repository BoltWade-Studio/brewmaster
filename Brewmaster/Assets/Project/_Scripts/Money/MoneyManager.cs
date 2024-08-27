using NOOD;
using UnityEngine;

namespace Game
{
	public class MoneyManager : MonoBehaviorInstance<MoneyManager>
	{
		[SerializeField] private int _bonus = 40;
		public int CurrentTarget => _currentTarget;
		public int NextTarget => _currentTarget + _bonus;
		public int Bonus => _bonus;

		private int _currentTarget;

		protected override void ChildAwake()
		{
			_currentTarget = 150;
		}

		void Start()
		{
			_currentTarget = DataSaveLoadManager.Instance.Target;
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
			PlayerData.TotalPoint -= CurrentTarget;
		}

		/// <summary>
		/// Use to buy item
		/// </summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public bool PayMoney(int amount)
		{
			if (PlayerData.TotalPoint >= amount)
			{
				PlayerData.TotalPoint -= amount;
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
			PlayerData.TotalPoint -= amount;
			UIManager.Instance.UpdateInDayMoney();
		}
		public void AddMoney(int amount)
		{
			PlayerData.TotalPoint += amount;
			UIManager.Instance.UpdateInDayMoney();
		}
	}

}
