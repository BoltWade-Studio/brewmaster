using System.Runtime.InteropServices;

namespace Game
{
	public static class JsSocketConnect
	{
		[DllImport("__Internal")]
		public static extern void SocketIOInit();

		[DllImport("__Internal")]
		public static extern void EmitEvent(string eventName, string dataArray = null);
		[DllImport("__Internal")]
		public static extern void OnEvent(string eventName, string callbackObjectName, string callbackMethodName);
		// [DllImport("__Internal")]
		// public static extern void OpenTwitter(string message);
	}
}
