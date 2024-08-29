using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public static class TwitterMessage
	{
		public static string GetTwitterMessage()
		{
			return $"Just brewed up a storm in Brewmaster 🍺\nScored {PlayerData.TotalPoint} points and 5 STRK for my skills——what a game!\nCheck out what @BoltwadeStudio has been cooking up on @Starknet\nIf you're not in on this yet, you're missing out! 👀";
		}
	}
}
