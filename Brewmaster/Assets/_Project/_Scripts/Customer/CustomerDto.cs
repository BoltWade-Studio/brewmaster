using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class CustomerListDto
	{
		public CustomerDto[] customerList;
	}

	[Serializable]
	public class CustomerDto
	{
		public int id;
		public Position position;
		public Rotation rotation;
		public int seatIndex;
		public int tableIndex;
		public float speed;
		public float rotateSpeed;
		public bool isRequestingBeer;
		public bool isServed;
		public bool isReturnCalled;
		public bool isWaiting;
		public float timeToServe;
		public float waitingTime;
		public bool served;
		public bool toDelete;
		public int money;
		public bool haveCoin;
	}

	[Serializable]
	public class Position
	{
		public float x;
		public float y;
		public float z;
	}

	[Serializable]
	public class Rotation
	{
		public float x;
		public float y;
		public float z;
		public float time;
	}
}
