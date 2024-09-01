using System;
using System.Collections.Generic;
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
        public Action<Vector3> OnServerFail;
        public Action<Customer> OnServeComplete;

        [SerializeField] private int _moneyLostOnFail = 20;
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
        }
        void Start()
        {
	        _player = Player.Instance;
	        Utility.Socket.OnEvent("serveBeer", this.gameObject.name, nameof(ServeBeer), ServeBeer);
	        Utility.Socket.OnEvent("updateBeer", this.gameObject.name, nameof(UpdateBeer), UpdateBeer);
	        Utility.Socket.OnEvent("beerCollided", this.gameObject.name, nameof(BeerCollided), BeerCollided);

	        BeerList = new List<List<BeerCup>>();

	        for (int i = 0; i < TableManager.Instance.GetTableList().Count; i++)
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
		        string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { time.ToString() } });
		        Utility.Socket.EmitEvent("updateBeer", json);
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
        }
        #endregion


        private void OnServerFailHandler(Vector3 failPosition)
        {
            Debug.Log("Remove money " + _moneyLostOnFail);
            MoneyManager.Instance.RemoveMoney(_moneyLostOnFail);
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
		        BeerDto beerDto = JsonUtility.FromJson<BeerDto>(data);
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
				BeerDto beerDto = JsonUtility.FromJson<BeerDto>(data);
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
				BeerDto beerDto = JsonUtility.FromJson<BeerDto>(data);
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
        // {
        //
        //     if (GameplayManager.Instance.IsEndDay == true) return;
        //
        //     if(_player == null)
        //     {
        //         _player = Player.Instance;
        //     }
        //     Vector3 startPosition = _player.transform.position;
        //     startPosition.y = _tableHeight;
        //
        //     BeerCup beerCup = Instantiate(_beerCupPref, startPosition, Quaternion.identity).GetComponent<BeerCup>();
        //     float moveSpeed = 5;
        //     NoodyCustomCode.StartUpdater(beerCup, () =>
        //     {
        //         if (beerCup == null) return true;
        //
        //         beerCup.transform.position += Vector3.left * moveSpeed * TimeManager.DeltaTime;
        //         return false;
        //     });
        // }
    }
}
