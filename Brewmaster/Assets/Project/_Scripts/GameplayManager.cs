using System;
using NOOD;
using NOOD.Sound;

namespace Game
{
	public class GameplayManager : MonoBehaviorInstance<GameplayManager>
	{
		public Action OnEndDay;
		public Action OnNextDay;
		public Action OnPausePressed;
		public bool IsEndDay;
		public bool IsPlaying;
		public bool IsWin { get; private set; }

		void Start()
		{
			if (TimeManager.Instance)
			{
				TimeManager.Instance.OnTimeUp += OnTimeUpHandler;
			}
			if (UIManager.Instance)
			{
				UIManager.Instance.OnNextDayPressed += OnNextDayPressHandler;
			}
			SoundManager.InitSoundManager();
			SoundManager.PlayMusic(MusicEnum.PianoBGMusic);
			SoundManager.PlayMusic(MusicEnum.CrowdBGSound);
			TimeManager.TimeScale = 1;
		}
		void OnDestroy()
		{
			NoodyCustomCode.UnSubscribeAllEvent<TimeManager>(this);
			NoodyCustomCode.UnSubscribeAllEvent<UIManager>(this);
		}

		#region Event functions
		private void OnTimeUpHandler()
		{
			IsEndDay = true;
			if (MoneyManager.Instance.CurrentTotalMoney > MoneyManager.Instance.CurrentTarget)
			{
				IsWin = true;
			}
			else
			{
				IsWin = false;
			}
			OnEndDay?.Invoke();
		}
		private void OnNextDayPressHandler()
		{
			IsEndDay = false;
			OnNextDay?.Invoke();
		}
		#endregion
	}

}
