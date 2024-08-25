using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
	public static class Utility
	{
		public static class Socket
		{
			private static SocketIOUnity socket;
			private static Dictionary<string, string> _methodName = new Dictionary<string, string>();
			private static Dictionary<string, string> _objectName = new Dictionary<string, string>();
			private static Dictionary<string, Action<string>> _actionEventDic = new Dictionary<string, Action<string>>();

#if UNITY_EDITOR
			public static void Init()
			{
				var uri = new Uri("http://localhost:5006");
				// var uri = new Uri("https://brewmaster-socket.starkarcade.com/");
				socket = new SocketIOUnity(uri, new SocketIOOptions
				{
					Query = new Dictionary<string, string>
					{
						{"token", "UNITY" }
					}
					,
					EIO = 4
					,
					Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
				});
				socket.JsonSerializer = new NewtonsoftJsonSerializer();

				socket.OnConnected += (sender, e) =>
				{
					Debug.Log("socket.OnConnected");
				};

				socket.On("updateCoin", (coin) =>
				{
					if (_actionEventDic.ContainsKey("updateCoin"))
					{
						string data = JsonConvert.DeserializeObject<string[]>(coin.ToString())[0];
						Debug.Log("Server update coin: " + data);
						_actionEventDic["updateCoin"].Invoke(data);
					}
				});

				socket.On("spawnCoin", (spawn) =>
				{
					if (_actionEventDic.ContainsKey("spawnCoin"))
					{
						_actionEventDic["spawnCoin"].Invoke(null);
					}
				});

				socket.On("updateProof", (proof) =>
				{
					if (_actionEventDic.ContainsKey("updateProof"))
					{
						string data = JsonConvert.DeserializeObject<string[]>(proof.ToString())[0];
						_actionEventDic["updateProof"].Invoke(data);
					}
				});

				socket.On("updateAnonymous", (data) =>
				{
					if (_actionEventDic.ContainsKey("updateAnonymous"))
					{
						Debug.Log("update anonymous: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["updateAnonymous"].Invoke(useData.ToString());
					}
				});

				socket.Connect();
			}
#endif

			public static void EmitEvent(string eventName, string jsonData = null)
			{
				Debug.Log("EmitEvent: " + eventName);
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.EmitEvent(eventName, jsonData);
#else
				socket.Emit(eventName, jsonData);
#endif
			}
			public static void OnEvent(string eventName, string objectName, string methodName, Action<string> action)
			{
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.OnEvent(eventName, objectName, methodName);
#else
				Debug.Log("OnEvent: " + eventName);
				if (_actionEventDic.ContainsKey(eventName))
					_actionEventDic[eventName] = action;
				else
					_actionEventDic.Add(eventName, action);
#endif
			}
		}
	}
}
