using System;
using System.Collections.Generic;
using System.Globalization;
using Game;
using Newtonsoft.Json;
using NOOD;
using NOOD.NoodCamera;
using NOOD.Sound;
using UnityEngine;
using Utils;

namespace Game
{
	public class BeerServeManager : MonoBehaviorInstance<BeerServeManager>
	{
		public Action<Vector3, int> OnServerFail;
		public Action<Customer> OnServeComplete;

		// [SerializeField] private int _moneyLostOnFail = 20;
		[SerializeField] private Transform _beerCupPref;
		[SerializeField] private float _tableHeight = 0.8f;
		private Player _player;
		private List<BeerDto> toSpawn = new List<BeerDto>();
		private List<List<BeerCup>> BeerList;
		public static Action OnPlayerPressDeliverBeer;
		private float lastUpdate = 0;
		private int beerCount = 0;

		#region Unity functions
		void OnEnable()
		{
			OnServerFail += OnServerFailHandler;
			OnServeComplete += OnServeCompleteHandler;
			GameEvent.Instance.OnEndDay += OnEndDayHandler;
		}
		void Start()
		{
			_player = Player.Instance;
			Utility.Socket.SubscribeEvent(SocketEnum.serveBeer.ToString(), this.gameObject.name, nameof(ServeBeer), ServeBeer);
			Utility.Socket.SubscribeEvent(SocketEnum.updateBeer.ToString(), this.gameObject.name, nameof(UpdateBeer), UpdateBeer);
			Utility.Socket.SubscribeEvent(SocketEnum.beerCollided.ToString(), this.gameObject.name, nameof(BeerCollided), BeerCollided);

			BeerList = new List<List<BeerCup>>();

			for (int i = 0; i < 10; i++)
			{
				BeerList.Add(new List<BeerCup>());
			}
		}

		private void Update()
		{
			if (beerCount > 0 && Time.time - lastUpdate > 1f / GameSetting.FPS)
			{
				float time = Time.time - lastUpdate;
				lastUpdate = Time.time;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { time.ToString(CultureInfo.InvariantCulture) } });
				Utility.Socket.EmitEvent(SocketEnum.updateBeer.ToString(), json);
			}
			else
			{
				if (beerCount == 0)
				{
					lastUpdate = Time.time;
				}
			}

			while (toSpawn.Count > 0)
			{
				beerCount++;
				BeerDto beerDto = toSpawn[0];
				toSpawn.RemoveAt(0);
				Vector3 startPosition = Player.Instance.transform.position;
				startPosition.y = beerDto.position.y;

				BeerCup beerCup = Instantiate(_beerCupPref, startPosition, Quaternion.identity).GetComponent<BeerCup>();
				beerCup.id = beerDto.id;
				BeerList[beerDto.tableIndex].Add(beerCup);
				OnPlayerPressDeliverBeer?.Invoke();
				SoundManager.PlaySound(SoundEnum.ServeBeer);
			}
		}

		void OnDisable()
		{
			OnServerFail -= OnServerFailHandler;
			OnServeComplete -= OnServeCompleteHandler;
			GameEvent.Instance.OnEndDay -= OnEndDayHandler;
		}
		#endregion

		#region Event functions
		private void OnServerFailHandler(Vector3 failPosition, int _moneyLostOnFail)
		{
			TextPopup.Show("-" + _moneyLostOnFail, failPosition, Color.red);
			SoundManager.PlaySound(SoundEnum.ServeFail, failPosition);
			FlashingUI.Instance.Flash(Color.red);
			CustomCamera.Instance.Shake();
		}
		private void OnServeCompleteHandler(Customer completeCustomer)
		{
			SoundManager.PlaySound(SoundEnum.ServeComplete, completeCustomer.transform.position);
		}

		public void ServeBeer(string data)
		{
			try
			{
				object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
				BeerDto beerDto = JsonUtility.FromJson<BeerDto>(useData.ToString());
				toSpawn.Add(beerDto);
			}
			catch (Exception e)
			{
				Debug.LogError("Serve Beer Error: " + e.Message);
			}
		}

		public void BeerCollided(string data)
		{
			try
			{
				object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
				BeerDto beerDto = JsonUtility.FromJson<BeerDto>(useData.ToString());
				BeerCup beerCup = BeerList[beerDto.tableIndex].Find(b => b.id == beerDto.id);
				Customer customer = CustomerManager.Instance.CustomerList.Find(c => c.id == beerDto.customerId);

				if (beerCup != null)
				{
					beerCup.Collide(beerDto, customer);
					BeerList[beerDto.tableIndex].Remove(beerCup);
					beerCount--;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Beer Collided Error: " + e.Message);
			}
		}

		public void UpdateBeer(string data)
		{
			try
			{
				object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
				BeerDto beerDto = JsonUtility.FromJson<BeerDto>(useData.ToString());
				BeerCup beerCup = BeerList[beerDto.tableIndex].Find(b => b.id == beerDto.id);
				if (beerCup != null)
				{
					// Debug.Log("Update beer position: " + beerDto.position);
					beerCup.MoveTo(new Vector3(beerDto.position.x, beerDto.position.y, beerDto.position.z));
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Update Beer Error: " + e.Message);
			}
		}

		public void OnEndDayHandler()
		{
			foreach (List<BeerCup> beerCups in BeerList)
			{
				foreach (BeerCup beerCup in beerCups)
				{
					Destroy(beerCup.gameObject);
				}
				beerCups.Clear();
			}
		}
		#endregion
	}
}
