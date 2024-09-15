using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using NOOD.Sound;
using UnityEngine;
using Utils;

namespace Game
{
	public class TimeManager : MonoBehaviorInstance<TimeManager>
	{
		#region Events
		public Action OnTimeWarning;
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
		private float lastUpdate = 0;
		#endregion

		#region Unity functions
		protected override void ChildAwake()
		{
			TimeScale = 1;
		}
		void Start()
		{
			GameEvent.Instance.OnNextDay += OnNextDayHandler;
			GameplayManager.Instance.OnPausePressed += PauseGame;
			if (DataSaveLoadManager.Instance)
				_day = DataSaveLoadManager.Instance.Day;
			ResetTime();

			Utility.Socket.OnEvent("updateTimer", this.gameObject.name, nameof(UpdateTimer), UpdateTimer);
			Utility.Socket.OnEvent("timeUp", this.gameObject.name, nameof(TimeUp), TimeUp);
		}

		private void Update()
		{
			if (Time.time - lastUpdate > 1f / GameSetting.FPS)
			{
				lastUpdate = Time.time;
				// Send time to server (in minutes)
				string json = JsonConvert.SerializeObject(new ArrayWrapper
				{ array = new string[] { Time.time.ToString() } });
				Utility.Socket.EmitEvent("updateTimer", json);
			}

			if (_hour == 0 && _isWarned == false)
			{
				// Warning
				Warning();
			}
		}
		void OnDestroy()
		{
			NoodyCustomCode.UnSubscribeAllEvent<UIManager>(this);
			NoodyCustomCode.UnSubscribeAllEvent<GameplayManager>(this);
		}
		#endregion

		private void UpdateTimer(string data)
		{
			try
			{
				// data consists of a float number
				float timeLeft = float.Parse(data); // in seconds
				_hour = Mathf.Floor(timeLeft / 60);
				_minute = timeLeft % 60;
			}
			catch (Exception e)
			{
				Debug.LogError("An error occurred: " + e.Message);
				throw;
			}
		}

		private async void TimeUp(string data)
		{
			await UniTask.SwitchToMainThread();
			Debug.Log("TimeUp Invoked!");
			_isTimeUp = true;
			GameEvent.Instance.OnTimeUp?.Invoke();
			SoundManager.PlaySound(SoundEnum.EndLevel);
		}

		private void PauseGame()
		{
			_isPause = !_isPause;
			if (_isPause)
			{
				TimeScale = 0;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString() } });
				Utility.Socket.EmitEvent("pauseGame", json);
			}
			else
			{
				TimeScale = 1;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString() } });
				Utility.Socket.EmitEvent("resumeGame", json);
			}
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
		private void OnNextDayHandler()
		{
			_day += 1;
			ResetTimeScale();
			ResetTime();
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
