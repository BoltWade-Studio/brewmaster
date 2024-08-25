using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Game
{
	public class TwitterTest : MonoBehaviour
	{
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Y))
			{
				Utility.Socket.EmitEvent("twitterTest", JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { "test" } }));
			}
		}
	}
}
