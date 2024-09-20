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

		protected override void ChildAwake()
		{
			base.ChildAwake();
			GameEvent.Instance.OnUpdatePosComplete += OnUpdatePosCompleteHandler;
		}


		void Start()
		{
			GameEvent.Instance.OnTimeUp += OnTimeUpHandler;
			GameEvent.Instance.OnNextDay += OnNextDayHandler;

			SoundManager.InitSoundManager();
			SoundManager.PlayMusic(MusicEnum.PianoBGMusic);
			SoundManager.PlayMusic(MusicEnum.CrowdBGSound);
			TimeManager.TimeScale = 1;

			Utility.Socket.EmitEvent(SocketEnum.updateIsMainMenu.ToString(), Utility.Socket.StringToSocketJson(IsMainMenu.ToString()));
		}

		void OnDestroy()
		{
			NoodyCustomCode.UnSubscribeAllEvent(GameEvent.Instance, this);
		}

		#region Event functions
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
			InitializedGameDataDto data = new InitializedGameDataDto();
			if (PlayerData.PlayerDataClass != null)
			{
				PlayerData.PlayerDataClass.Points = 0;
				PlayerData.PlayerDataClass.Treasury = 0;
				UIManager.Instance.UpdateInDayMoney();
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
