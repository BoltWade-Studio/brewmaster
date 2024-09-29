using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
	public class CustomerSpawner : MonoBehaviorInstance<CustomerSpawner>
	{
		public Action<Customer> onCustomerSpawn;

		[SerializeField] private Transform _customerPref;
		[SerializeField] private float _spawnTime;
		private List<CustomerDto> toSpawn = new List<CustomerDto>();

		#region Unity functions
		private void Start()
		{
			StartCoroutine(SpawnCustomer());
			Utility.Socket.SubscribeEvent(SocketEnum.spawnCustomerCallback.ToString(), this.gameObject.name, nameof(SpawnCustomer), SpawnCustomer);
		}

		private void Update()
		{
			while (toSpawn.Count > 0)
			{
				try
				{
					CustomerDto customerDto = toSpawn[0];
					toSpawn.RemoveAt(0);
					Vector3 spawnPosition = new Vector3(customerDto.position.x, customerDto.position.y, customerDto.position.z);
					// Debug.Log("Spawn customer at: " + spawnPosition);
					Customer customer = Instantiate(_customerPref, spawnPosition, Quaternion.identity)
						.GetComponent<Customer>();
					customer.id = customerDto.id;
					CustomerManager.Instance.AddCustomer(customer);
					onCustomerSpawn?.Invoke(customer);
				}
				catch (Exception e)
				{
					Debug.LogError("Spawn customer error: " + e.Message);
				}
			}
		}
		void OnDestroy()
		{
			Utility.Socket.UnSubscribeEvent(SocketEnum.spawnCustomerCallback.ToString(), this.gameObject.name, nameof(SpawnCustomer), SpawnCustomer);
		}
		#endregion

		#region Event functions
		private async void SpawnCustomer(string data)
		{
			await UniTask.SwitchToMainThread();
			Debug.Log("Spawn customer callback: " + data);
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			// Debug.Log("Spawn customer");
			try
			{
				CustomerDto customerDto = JsonConvert.DeserializeObject<CustomerDto>(useData.ToString());
				toSpawn.Add(customerDto);
			}
			catch (Exception e)
			{
				Debug.LogError("Spawning Error: " + e.Message);
			}
		}
		#endregion

		#region Coroutine
		IEnumerator SpawnCustomer()
		{
			while (true)
			{
				yield return new WaitForSeconds(_spawnTime);
				Utility.Socket.EmitEvent("spawnCustomer");
			}
		}
		#endregion
	}

}
