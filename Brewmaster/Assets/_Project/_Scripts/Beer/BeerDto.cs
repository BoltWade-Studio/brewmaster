using System;
using UnityEngine;

namespace Game
{
	[System.Serializable]
	public class BeerDto
	{
		public int id;
		public Position position;
		public float moveSpeed;
		public bool collided;
		public bool punished;
		public int earnedMoney;
		public int customerId;
		public int tableIndex;
	}

}
