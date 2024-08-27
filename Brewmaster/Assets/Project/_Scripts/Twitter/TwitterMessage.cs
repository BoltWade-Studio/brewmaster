using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public static class TwitterMessage
	{
		public static string GetTwitterMessage()
		{
			return $"StarkArcade challenge ----- {PlayerData.TotalPoint} -----";
		}
	}
}
