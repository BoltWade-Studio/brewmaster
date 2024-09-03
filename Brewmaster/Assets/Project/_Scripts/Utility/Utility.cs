using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using Utils;

namespace Game
{
	public static class Utility
	{
		public static class Socket
		{
			private static SocketIOUnity socket;
			private static Dictionary<string, Action<string>> _actionEventDic = new Dictionary<string, Action<string>>();
			private static List<Action<string>> methodLists = new List<Action<string>>();

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

				socket.On("exception", (exception) =>
				{
					Debug.Log("on exception: " + exception.ToString());
					Debug.Log("Count" + methodLists.Count);
					WSError wSError;
					wSError = JsonConvert.DeserializeObject<WSError[]>(exception.ToString())[0];
					foreach (Action<string> action in methodLists)
					{
						Debug.Log("Active action");
						try
						{
							action(wSError.message);
						}
						catch (Exception e)
						{
							Debug.Log("Exception: " + e.Message);
						}
					}
				});

				socket.On("giftTxHash", (data) =>
				{
					if (_actionEventDic.ContainsKey("giftTxHash"))
					{
						_actionEventDic["giftTxHash"].Invoke(data.ToString());
					}
				});

				socket.On("playerMove", (data) =>
				{
					if (_actionEventDic.ContainsKey("playerMove"))
					{
						Debug.Log("playerMove: " + data.ToString());
						_actionEventDic["playerMove"].Invoke(data.ToString());
					}
				});

	            socket.On("spawnCustomer", (data) =>
				{
					Debug.Log("Receive event spawnCustomer!");
					if(_actionEventDic.ContainsKey("spawnCustomer"))
					{
						// Debug.Log("spawnCustomer: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["spawnCustomer"].Invoke(useData.ToString());
					}
				});

				socket.On("updateCustomerPosition", (data) =>
				{
					if (_actionEventDic.ContainsKey("updateCustomerPosition"))
					{
						// Debug.Log("updateCustomerPosition: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["updateCustomerPosition"].Invoke(useData.ToString());
					}
				});

				socket.On("updateCustomerWaitTime", (data) =>
				{
					if (_actionEventDic.ContainsKey("updateCustomerWaitTime"))
					{
						// Debug.Log("updateCustomerWaitTime: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["updateCustomerWaitTime"].Invoke(useData.ToString());
					}
				});

				socket.On("customerReachDestination", (data) =>
				{
					if (_actionEventDic.ContainsKey("customerReachDestination"))
					{
						// Debug.Log("customerReachDestination: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["customerReachDestination"].Invoke(useData.ToString());
					}
				});

				socket.On("customerReturn", (data) =>
				{
					if (_actionEventDic.ContainsKey("customerReturn"))
					{
						// Debug.Log("customerReturn: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["customerReturn"].Invoke(useData.ToString());
					}
				});

				socket.On("deleteCustomer", (data) =>
				{
					if (_actionEventDic.ContainsKey("deleteCustomer"))
					{
						// Debug.Log("deleteCustomer: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["deleteCustomer"].Invoke(useData.ToString());
					}
				});

				socket.On("serveBeer", (data) =>
				{
					if (_actionEventDic.ContainsKey("serveBeer"))
					{
						// Debug.Log("serveBeer: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["serveBeer"].Invoke(useData.ToString());
					}
				});

				socket.On("updateBeer", (data) =>
				{
					if (_actionEventDic.ContainsKey("updateBeer"))
					{
						// Debug.Log("updateBeer: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["updateBeer"].Invoke(useData.ToString());
					}
				});

				socket.On("beerCollided", (data) =>
				{
					if (_actionEventDic.ContainsKey("beerCollided"))
					{
						// Debug.Log("beerCollided: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["beerCollided"].Invoke(useData.ToString());
					}
				});

				socket.On("updateTimer", (data) =>
				{
					if (_actionEventDic.ContainsKey("updateTimer"))
					{
						// Debug.Log("updateTimer: " + data);
						object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
						_actionEventDic["updateTimer"].Invoke(useData.ToString());
					}
				});

				socket.On("timeUp", (data) =>
				{
					if (_actionEventDic.ContainsKey("timeUp"))
					{
						try
						{
							Debug.Log("timeUp: " + data);
							_actionEventDic["timeUp"].Invoke("");
						}
						catch (Exception e)
						{
							Debug.Log("Timeup Exception: " + e.Message);
						}
					}
				});

				socket.Connect();
			}
#endif

			public static void EmitEvent(string eventName, string jsonData = null)
			{
				Debug.Log("EmitEvent: " + eventName);
				jsonData = ToSocketJson(jsonData);

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

			public static void SubscribeOnException(string objectName, string methodName, Action<string> action)
			{
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.SubscribeOnException(objectName, methodName);
#else
				Debug.Log("Add method: " + methodName);
				methodLists.Add(action);
#endif
			}

			public static void UnSubscribeOnException(string objectName, string methodName, Action<string> action)
			{
#if UNITY_EDITOR && !UNITY_WEBGL
				JsSocketConnect.UnSubscribeOnException(objectName, methodName);
#else
				methodLists.Remove(action);
#endif
			}

			public static string ToSocketJson(string jsonRaw)
			{
				if (jsonRaw != null)
				{
					int startIndex = jsonRaw.IndexOf("[");
					int length = jsonRaw.Length - startIndex - 1;
					jsonRaw = jsonRaw.Substring(startIndex, length);
				}
				return jsonRaw;
			}
		}
	}
}

public class WSError
{
	public string status;
	public string message;
}
