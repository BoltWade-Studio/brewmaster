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
		[HideInInspector] public List<Customer> CustomerList = new List<Customer>();
		private float lastSpawnTime = 0;

		#region Unity Functions
		private void Start()
		{
			GameEvent.Instance.OnEndDay += OnEndDayHandler;
			Utility.Socket.SubscribeEvent(SocketEnum.updateCustomerPosition.ToString(), this.gameObject.name, nameof(UpdateCustomerPosition), UpdateCustomerPosition);
			Utility.Socket.SubscribeEvent(SocketEnum.updateCustomerWaitTime.ToString(), this.gameObject.name, nameof(UpdateCustomerWaitTime),
				UpdateCustomerWaitTime);
			Utility.Socket.SubscribeEvent(SocketEnum.customerReachDestination.ToString(), this.gameObject.name, nameof(CustomerReachDestination), CustomerReachDestination);
			Utility.Socket.SubscribeEvent(SocketEnum.customerReturn.ToString(), this.gameObject.name, nameof(CustomerReturn), CustomerReturn);
			Utility.Socket.SubscribeEvent(SocketEnum.deleteCustomer.ToString(), this.gameObject.name, nameof(DeleteCustomer), DeleteCustomer);
		}

		private void Update()
		{
			if (CustomerList.Count > 0 && Time.time - lastSpawnTime > 1f / GameSetting.FPS)
			{
				float time = Time.time - lastSpawnTime;
				lastSpawnTime = Time.time;
				string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { time.ToString() } });
				Utility.Socket.EmitEvent(SocketEnum.updateCustomer.ToString(), json);
			}
			else
			{
				if (CustomerList.Count == 0)
				{
					lastSpawnTime = Time.time;
				}
			}
		}
		void OnDestroy()
		{
			GameEvent.Instance.OnEndDay -= OnEndDayHandler;
		}
		#endregion

		#region Public Functions
		public void AddCustomer(Customer customer)
		{
			CustomerList.Add(customer);
		}
		#endregion

		#region Event Handlers
		public void UpdateCustomerPosition(string data)
		{
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(useData.ToString());
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
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(useData.ToString());
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
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(useData.ToString());
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
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(useData.ToString());
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
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			try
			{
				CustomerDto customerDto = JsonUtility.FromJson<CustomerDto>(useData.ToString());
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

		public void OnEndDayHandler()
		{
			// CustomerList.Clear();
		}
		#endregion

	}
}
