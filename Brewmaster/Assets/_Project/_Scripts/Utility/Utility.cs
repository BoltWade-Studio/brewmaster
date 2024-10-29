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

			public static void Init()
			{
#if UNITY_EDITOR
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
					Debug.Log("socket.OnAny: " + eventName);
					if (eventName == "exception")
					{
						foreach (Action<string> action in _exceptionMethodList)
						{
							Debug.Log("Active action");
							try
							{
								action(data.GetValue<string>());
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
									// Debug.Log("Socket.OnAny error: " + e.Message);
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
						else
						{
							Debug.Log("Socket.OnAny: " + eventName + " no listener");
						}
					}
				});

				socket.Connect();
#else
				JsSocketConnect.SocketIOInit();
#endif
			}

			public static void Disconnect()
			{
#if UNITY_EDITOR
				socket.Disconnect();
#endif
			}

			public static void EmitEvent(string eventName, string jsonData = null)
			{
				// Debug.Log("EmitEvent: " + eventName);
				try
				{
					if (jsonData != null)
						jsonData = ToSocketJson(jsonData);
					// Debug.Log("EmitEvent: " + eventName + " " + jsonData);

#if UNITY_WEBGL && !UNITY_EDITOR
					JsSocketConnect.EmitEvent(eventName, jsonData);
#else

					if (jsonData != null)
						socket.Emit(eventName, jsonData);
					else
						socket.Emit(eventName);
#endif
				}
				catch (Exception e)
				{
					Debug.Log("EmitEvent error: " + e.Message);
				}
			}
			public static void SubscribeEvent(string eventName, string objectName, string methodName, Action<string> action)
			{
				Debug.Log("SubscribeEvent: " + eventName + " " + objectName + " " + methodName);
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.SubscribeEvent(eventName, objectName, methodName);
#else
				if (_socketEventClassDic.ContainsKey(eventName))
				{
					if (_socketEventClassDic[eventName].Contains(new SocketEventClass()
					{
						ClassName = objectName,
						MethodName = methodName,
						Action = action
					}))
					{
						return;
					}
					_socketEventClassDic[eventName].Add(new SocketEventClass()
					{
						ClassName = objectName,
						MethodName = methodName,
						Action = action
					});
				}
				else
				{
					_socketEventClassDic.Add(eventName, new List<SocketEventClass>()
					{
						new SocketEventClass()
						{
							ClassName = objectName,
							MethodName = methodName,
							Action = action
						}
					});
				}
				if (_socketEventClassDic.ContainsKey(eventName) && _socketEventClassDic[eventName].Count > 0)
				{
					Debug.Log(JsonConvert.SerializeObject(_socketEventClassDic[eventName][0], Formatting.Indented));
				}
#endif
			}

			public static void UnSubscribeEvent(string eventName, string objectName, string methodName, Action<string> action)
			{
				Debug.Log("UnSubscribeEvent: " + eventName + " " + objectName + " " + methodName);
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.UnSubscribeEvent(eventName, objectName, methodName);
#else
				if (_socketEventClassDic.ContainsKey(eventName))
				{
					if (_socketEventClassDic[eventName].Contains(new SocketEventClass()
					{
						ClassName = objectName,
						MethodName = methodName,
						Action = action
					}))
					{
						_socketEventClassDic[eventName].Remove(new SocketEventClass()
						{
							ClassName = objectName,
							MethodName = methodName,
							Action = action
						});
					}
				}
#endif
			}

			public static void SubscribeOnException(string objectName, string methodName, Action<string> action)
			{
#if UNITY_WEBGL && !UNITY_EDITOR
				JsSocketConnect.SubscribeOnException(objectName, methodName);
#else
				_exceptionMethodList.Add(action);
#endif
			}

			public static void UnSubscribeOnException(string objectName, string methodName, Action<string> action)
			{
#if UNITY_EDITOR && !UNITY_WEBGL
				JsSocketConnect.UnSubscribeOnException(objectName, methodName);
#else
				_exceptionMethodList.Remove(action);
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
				string jsonString = JsonConvert.SerializeObject(new string[] { normalString });
				return jsonString;
			}

			internal static void SubscribeEvent(string v, object onGetLeaderboardCallback)
			{
				throw new NotImplementedException();
			}

		}
	}
}

public class WSError
{
	public string status;
	public string message;
}
