using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using NOOD.Sound;
using UnityEngine;
using Utils;

namespace Game
{
	public class GameplayManager : MonoBehaviorInstance<GameplayManager>
	{
		public Action OnPausePressed;
		public bool IsEndDay;
		public bool IsPlaying;
		public bool IsMainMenu;
		public bool IsStorePhase;

		protected override void ChildAwake()
		{
			base.ChildAwake();
			GameEvent.Instance.OnUpdatePosComplete += OnUpdatePosCompleteHandler;
		}


		void Start()
		{
			GameEvent.Instance.OnTimeUp += OnTimeUpHandler;
			GameEvent.Instance.OnNextDay += OnNextDayHandler;
			GameEvent.Instance.OnStorePhase += OnStorePhaseHandler;
			// GameEvent.Instance.OnLoadDataSuccess += OnLoadDataSuccessHandler;

			SoundManager.InitSoundManager();
			SoundManager.PlayMusic(MusicEnum.PianoBGMusic);
			SoundManager.PlayMusic(MusicEnum.CrowdBGSound);
			TimeManager.TimeScale = 1;

			Utility.Socket.EmitEvent(SocketEnum.updateIsMainMenu.ToString(), Utility.Socket.StringToSocketJson(IsMainMenu.ToString()));
		}
		void OnDestroy()
		{
			GameEvent.Instance.OnTimeUp -= OnTimeUpHandler;
			GameEvent.Instance.OnNextDay -= OnNextDayHandler;
			GameEvent.Instance.OnStorePhase -= OnStorePhaseHandler;
			GameEvent.Instance.OnUpdatePosComplete -= OnUpdatePosCompleteHandler;
			// GameEvent.Instance.OnLoadDataSuccess -= OnLoadDataSuccessHandler;
		}

		#region Event functions
		private void OnStorePhaseHandler()
		{
			IsStorePhase = true;
		}

		private void OnLoadDataSuccessHandler()
		{
			InitializeGame();
		}

		private void OnUpdatePosCompleteHandler()
		{
			InitializeGame();
		}
		private void OnTimeUpHandler()
		{
			IsEndDay = true;
			GameEvent.Instance.OnEndDay?.Invoke();
		}
		private void OnNextDayHandler()
		{
			InitializeGame();
		}
		#endregion

		#region Private functions
		private void InitializeGame()
		{
			Debug.Log("InitializeGame");
			IsEndDay = false;
			IsStorePhase = false;
			if (PlayerData.PlayerDataClass != null)
			{
				PlayerData.InDayTreasury = 0;
				UIManager.Instance.UpdateMoneyUI();
			}

			Utility.Socket.EmitEvent(SocketEnum.initGame.ToString());
		}
		#endregion
	}

}
