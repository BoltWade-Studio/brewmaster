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
	public class SocketEventClass : IEquatable<SocketEventClass>
	{
		public string ClassName;
		public string MethodName;
		[JsonIgnore]
		public Action<string> Action;

		public bool Equals(SocketEventClass other)
		{
			return ClassName == other.ClassName && MethodName == other.MethodName && Action == other.Action;
		}
	}

	public static class Utility
	{
		public static class Socket
		{
			private static SocketIOUnity socket;
			private static List<Action<string>> _exceptionMethodList = new List<Action<string>>();
			private static Dictionary<string, List<SocketEventClass>> _socketEventClassDic = new Dictionary<string, List<SocketEventClass>>();

#if UNITY_EDITOR
			public static void Init()
			{
				var uri = new Uri("http://localhost:5006");
				// var uri = new Uri("https://brewmaster-socket-test.boltwade.xyz");
				// var uri = new Uri("https://brewmaster-socket.starkarcade.com/");

				_socketEventClassDic = new Dictionary<string, List<SocketEventClass>>();
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

				socket.OnAny((eventName, data) =>
				{
					if (eventName == "exception")
					{
						WSError wSError;
						wSError = JsonConvert.DeserializeObject<WSError[]>(data.GetValue<string>())[0];
						foreach (Action<string> action in _exceptionMethodList)
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
					}
					else
					{
						if (_socketEventClassDic.ContainsKey(eventName))
						{
							string dataString = "";
							if (data.Count > 0)
							{
								try
								{
									dataString = data.GetValue<string>();
								}
								catch (Exception e)
								{
									dataString = data.ToString();
									Debug.Log("Socket.OnAny error: " + e.Message);
								}
							}

							foreach (var socketEventClass in _socketEventClassDic[eventName])
							{
								List<String> noLogList = new List<string>
									{
										"updateCustomerWaitTime",
										"updateTimer",
										"updateCustomerPosition",
										"updateBeer",
										"spawnCustomerCallback",
										"customerReachDestination",
										"deleteCustomer",
										"customerReturn",
									};
								if (!noLogList.Contains(eventName))
									Debug.Log("Socket.OnAny: " + eventName + " " + (dataString ?? ""));
								socketEventClass.Action?.Invoke(dataString);
							}
						}
					}
				});

				socket.Connect();
			}

#endif
			public static void Disconnect()
			{
#if UNITY_EDITOR
				socket.Disconnect();
#endif
			}

			public static void EmitEvent(string eventName, string jsonData = null)
			{
				// Debug.Log("EmitEvent: " + eventName);
				if (jsonData != null)
					jsonData = ToSocketJson(jsonData);

#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.EmitEvent(eventName, jsonData);
#else

				socket.Emit(eventName, jsonData);
#endif
			}
			public static void OnEvent(string eventName, string objectName, string methodName, Action<string> action)
			{
				// Debug.Log("OnEvent: " + eventName);
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.OnEvent(eventName, objectName, methodName);
#else
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
					int endIndex = jsonRaw.LastIndexOf("]") + 1;
					int length = endIndex - startIndex;
					jsonRaw = jsonRaw.Substring(startIndex, length);
				}
				return jsonRaw;
			}

			public static string StringToSocketJson(string normalString)
			{
				string jsonString = JsonConvert.SerializeObject(new ArrayWrapper() { array = new string[] { normalString } });
				return ToSocketJson(jsonString);
			}
		}
	}
}

public class WSError
{
	public string status;
	public string message;
}
