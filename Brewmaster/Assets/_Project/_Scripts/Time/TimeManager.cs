using System;
using System.Globalization;
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

			Utility.Socket.SubscribeEvent("updateTimer", this.gameObject.name, nameof(UpdateTimer), UpdateTimer);
			Utility.Socket.SubscribeEvent("timeUp", this.gameObject.name, nameof(TimeUp), TimeUp);
		}

		private void Update()
		{
			if (Time.time - lastUpdate > 1f / GameSetting.FPS && !_isTimeUp)
			{
				lastUpdate = Time.time;
				// Send time to server (in minutes)
				string json = Utility.Socket.StringToSocketJson(Time.time.ToString(CultureInfo.InvariantCulture));
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
			GameEvent.Instance.OnNextDay -= OnNextDayHandler;
			GameplayManager.Instance.OnPausePressed -= PauseGame;
			Utility.Socket.UnSubscribeEvent("updateTimer", this.gameObject.name, nameof(UpdateTimer), UpdateTimer);
			Utility.Socket.UnSubscribeEvent("timeUp", this.gameObject.name, nameof(TimeUp), TimeUp);
		}
		#endregion

		private async void UpdateTimer(string data)
		{
			try
			{
				await UniTask.SwitchToMainThread();
				// object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
				// data consists of a float number
				float timeLeft = float.Parse(data.ToString(), CultureInfo.InvariantCulture); // in seconds
				_hour = Mathf.Floor(timeLeft / 60);
				_minute = timeLeft % 60;
			}
			catch (Exception e)
			{
				Debug.Log("UpdateTimer: " + data);
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

		public void PauseGame()
		{
			_isPause = !_isPause;
			if (_isPause)
			{
				TimeScale = 0;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString(CultureInfo.InvariantCulture) } });
				Utility.Socket.EmitEvent(SocketEnum.pauseGame.ToString(), json);
			}
			else
			{
				TimeScale = 1;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString(CultureInfo.InvariantCulture) } });
				Utility.Socket.EmitEvent(SocketEnum.resumeGame.ToString(), json);
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
			_isTimeUp = false;
			UIManager.Instance.UpdateDayText();
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
