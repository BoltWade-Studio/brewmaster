using System;
using System.Collections.Generic;
using Game;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;

namespace Game
{
	public class CustomerManager : MonoBehaviorInstance<CustomerManager>
	{
		public List<Customer> CustomerList = new List<Customer>();
		private float lastSpawnTime = 0;

		private void Start()
		{
			Utility.Socket.OnEvent("updateCustomerPosition", this.gameObject.name, nameof(UpdateCustomerPosition), UpdateCustomerPosition);
			Utility.Socket.OnEvent("updateCustomerWaitTime", this.gameObject.name, nameof(UpdateCustomerWaitTime),
				UpdateCustomerWaitTime);
			Utility.Socket.OnEvent("customerReachDestination", this.gameObject.name, nameof(CustomerReachDestination), CustomerReachDestination);
			Utility.Socket.OnEvent("customerReturn", this.gameObject.name, nameof(CustomerReturn), CustomerReturn);
			Utility.Socket.OnEvent("deleteCustomer", this.gameObject.name, nameof(DeleteCustomer), DeleteCustomer);
		}

		private void Update()
		{
			if (CustomerList.Count > 0 && Time.time - lastSpawnTime > 1f / GameSetting.FPS)
			{
				float time = Time.time - lastSpawnTime;
				lastSpawnTime = Time.time;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { time.ToString() } });
				Utility.Socket.EmitEvent("updateCustomer", json);
			}
			else
			{
				if (CustomerList.Count == 0)
				{
					lastSpawnTime = Time.time;
				}
			}
		}

		public void AddCustomer(Customer customer)
		{
			CustomerList.Add(customer);
		}

		public void RemoveCustomer(Customer customer)
		{
			CustomerList.Remove(customer);
		}

		public void UpdateCustomerPosition(string data)
		{
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				Customer customer = CustomerList.Find(c => c.id == customerDto.id);
				if (customer != null)
				{
					customer.MoveTo(new Vector3(customerDto.position.x, customerDto.position.y,
						customerDto.position.z), new Vector3(customerDto.rotation.x, customerDto.rotation.y,
						customerDto.rotation.z), customerDto.rotation.time);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Update Customer Error: " + e.Message);
			}
		}

		public void UpdateCustomerWaitTime(string data)
		{
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				Customer customer = CustomerList.Find(c => c.id == customerDto.id);
				if (customer != null)
				{
					customer.UpdateWaitTimer(customerDto.waitingTime, customerDto.timeToServe);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Update Customer Error: " + e.Message);
			}
		}

		public void CustomerReachDestination(string data)
		{
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				Customer customer = CustomerList.Find(c => c.id == customerDto.id);
				if (customer != null)
				{
					// customer.Complete(customerDto.money);
					customer.reachedDestination = true;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Customer Reach Destination Error: " + e.Message);
			}
		}

		public void CustomerReturn(string data)
		{
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				Customer customer = CustomerList.Find(c => c.id == customerDto.id);
				if (customer != null)
				{
					customer.toReturn = true;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Customer Return Error: " + e.Message);
			}
		}

		public void DeleteCustomer(string data)
		{
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(data);
				Customer customer = CustomerList.Find(c => c.id == customerDto.id);
				if (customer != null)
				{
					customer.toDestroy = true;
					CustomerList.Remove(customer);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Delete Customer Error: " + e.Message);
			}
		}

	}
}
