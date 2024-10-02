using System.Runtime.InteropServices;

namespace Game
{
	public static class JsSocketConnect
	{
		[DllImport("__Internal")]
		public static extern void SocketIOInit();

		[DllImport("__Internal")]
		public static extern void EmitEvent(string eventName, string jsonArray = null);

		[DllImport("__Internal")]
		public static extern void SubscribeEvent(string eventName, string callbackObjectName, string callbackMethodName);
		[DllImport("__Internal")]
		public static extern void UnSubscribeEvent(string eventName, string callbackObjectName, string callbackMethodName);
		[DllImport("__Internal")]
		public static extern void SubscribeOnException(string callbackObjectName, string callbackMethodName);
		[DllImport("__Internal")]
		public static extern void UnSubscribeOnException(string callbackObjectName, string callbackMethodName);
	}
}
