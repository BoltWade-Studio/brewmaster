using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private void Start()
		{
			StartCoroutine(SpawnerCustomer());
			Utility.Socket.OnEvent("spawnCustomer", this.gameObject.name, nameof(SpawnCustomer), SpawnCustomer);
		}

		private void SpawnCustomer(string data)
		{
			Debug.Log("Spawn customer");
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				toSpawn.Add(customerDto);
			}
			catch (Exception e)
			{
				Debug.LogError("Spawning Error: " + e.Message);
			}
		}

		private void Update()
		{
			while (toSpawn.Count > 0)
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
		}

		IEnumerator SpawnerCustomer()
		{
			while (true)
			{
				yield return new WaitForSeconds(_spawnTime);
				Utility.Socket.EmitEvent("spawnCustomer");

				// List<Table> availableTable = TableManager.Instance.GetTableList().Where(table => table.IsAvailable() == true).ToList();
				// if (availableTable != null && availableTable.Count > 0 && GameplayManager.Instance.IsEndDay == false)
				// {
				// 	Customer customer = Instantiate(_customerPref, this.transform.position, Quaternion.identity).GetComponent<Customer>();
				//
				// 	// Get random seat
				// 	int r = UnityEngine.Random.Range(0, availableTable.Count - 1);
				// 	Transform seat = availableTable[r].GetSeatForCustomer(customer);
				// 	// Check if seat valid
				// 	customer.SetTargetPosition(seat.position);
				// 	onCustomerSpawn?.Invoke(customer);
				//
				// 	// emit socket event to server
				// 	if (GameplayManager.Instance.IsPlaying)
				// 	{
				// 		Utility.Socket.EmitEvent("spawnCustomer");
				// 	}
				// }
			}
		}
	}

}
