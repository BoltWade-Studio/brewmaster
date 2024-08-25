using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class TwitterTest : MonoBehaviour
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				Utility.Socket.EmitEvent("twitterTest", "test");
			}
		}
	}
}
