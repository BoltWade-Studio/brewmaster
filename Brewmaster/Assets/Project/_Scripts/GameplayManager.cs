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
		}

		#region Event functions
		private void OnStorePhaseHandler()
		{
			IsStorePhase = true;
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
			IsEndDay = false;
			IsStorePhase = false;
			InitializedGameDataDto data = new InitializedGameDataDto();
			if (PlayerData.PlayerDataClass != null)
			{
				PlayerData.InDayTreasury = 0;
				UIManager.Instance.UpdateMoneyUI();
			}
			foreach (Table table in TableManager.Instance.GetTableList())
			{
				SeatListDto seatListDto = new SeatListDto();
				foreach (Transform seat in table.GetAllSeats())
				{
					seatListDto.seatPositionList.Add(seat.position);
					if (seatListDto.seatPositionList.Count == table.AvailableSeatNumber)
					{
						break;
					}
				}
				data.tableList.Add(seatListDto);
				data.tablePositionList.Add(table.transform.position);
			}

			data.customerSpawnPosition = CustomerSpawner.Instance.transform.position;
			data.playerPosition = Player.Instance.transform.position;

			Utility.Socket.EmitEvent(SocketEnum.initGame.ToString());
		}
		#endregion
	}

}
