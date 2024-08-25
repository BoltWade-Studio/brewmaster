using System;
using NOOD;
using NOOD.Sound;
using UnityEngine;

namespace Game
{
	public class TimeManager : MonoBehaviorInstance<TimeManager>
	{
		#region Events
		public Action OnTimeWarning;
		public Action OnTimeUp;
		public static Action OnTimePause;
		public static Action OnTimeResume;
		#endregion

		#region Property
		public static float DeltaTime => Time.deltaTime * TimeScale;
		public static float UnScaledDeltaTime => Time.unscaledDeltaTime;
		private static float s_timeScale = 1;
		public static float TimeScale
		{
			get => s_timeScale; set
			{
				if (value == 0)
					OnTimePause?.Invoke();
				else
					OnTimeResume?.Invoke();
				s_timeScale = value;
			}
		}
		public int CurrentDay => _day;
		#endregion

		#region SerializeField
		[SerializeField] private float _timeMultipler;
		[SerializeField] private float _hourInLevel;
		#endregion

		#region Private
		private float _hour;
		private float _minute;
		private int _day = 1;
		private bool _isWarned, _isTimeUp;
		private bool _isPause;
		#endregion

		#region Unity functions
		protected override void ChildAwake()
		{
			TimeScale = 1;
		}
		void Start()
		{
			if (UIManager.Instance)
				UIManager.Instance.OnNextDayPressed += NextDay;
			GameplayManager.Instance.OnNextDay += ResetTimeScale;
			GameplayManager.Instance.OnNextDay += ResetTime;
			GameplayManager.Instance.OnPausePressed += PauseGame;
			if (DataSaveLoadManager.Instance)
				_day = DataSaveLoadManager.Instance.Day;
			ResetTime();
		}

		private void Update()
		{
			if (_isTimeUp == false)
				_minute -= DeltaTime * _timeMultipler * TimeScale;
			if (_minute >= 59)
			{
				_hour++;
				_minute = 0;
			}
			if (_minute < 0)
			{
				_hour--;
				_minute = 59;
			}
			if (_hour == 0 && _isWarned == false)
			{
				// Warning
				Warning();
			}
			if (_hour < 0 && GameplayManager.Instance.IsEndDay == false)
			{
				// Stop level
				if (_isTimeUp == false)
				{
					OnTimeUp?.Invoke();
					_isTimeUp = true;
					SoundManager.PlaySound(SoundEnum.EndLevel);
				}
			}
		}
		void OnDestroy()
		{
			NoodyCustomCode.UnSubscribeAllEvent<UIManager>(this);
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
		}
		#endregion

		private void PauseGame()
		{
			_isPause = !_isPause;
			if (_isPause)
				TimeScale = 0;
			else
				TimeScale = 1;
		}
		private void ResetTimeScale()
		{
			TimeScale = 1;
		}
		private void ResetTime()
		{
			Debug.Log("Hours: " + _hourInLevel);
			_hour = _hourInLevel;
			_minute = 0;
			_isTimeUp = false;
			_isWarned = false;
		}
		private void NextDay()
		{
			_day += 1;
		}
		private void Warning()
		{
			OnTimeWarning?.Invoke();
			SoundManager.PlaySound(SoundEnum.Timer);
			_isWarned = true;
		}

		public float GetHour() => _hour;
		public float GetMinute() => _minute;
	}

}
