using System;
using Newtonsoft.Json;
using NOOD;
using NOOD.Sound;
using UnityEngine;
using Utils;

namespace Game
{
	public class GameplayManager : MonoBehaviorInstance<GameplayManager>
	{
		public Action OnEndDay;
		public Action OnNextDay;
		public Action OnPausePressed;
		public bool IsEndDay;
		public bool IsPlaying;

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
			OnNextDay += InitializeGame;

			InitializeGame();
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
			OnEndDay?.Invoke();
		}
		private void OnNextDayPressHandler()
		{
			IsEndDay = false;
			OnNextDay?.Invoke();
		}
		#endregion

		#region Private functions
		private void InitializeGame()
		{
			InitializedGameDataDto data = new InitializedGameDataDto();
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
				// Debug.Log("Seat List: " + JsonUtility.ToJson(seatListDto));
				data.tableList.Add(seatListDto);
				data.tablePositionList.Add(table.transform.position);
			}

			data.customerSpawnPosition = CustomerSpawner.Instance.transform.position;
			data.playerPosition = Player.Instance.transform.position;

			string json = JsonConvert.SerializeObject(new ArrayWrapper
			{ array = new string[] { JsonUtility.ToJson(data) } });

			Utility.Socket.EmitEvent(SocketEnum.initGame.ToString(), json);
		}
		#endregion
	}

}
