using System.Runtime.InteropServices;

namespace Game
{
    public static class JsReloadWindow
    {
        [DllImport("__Internal")]
        public static extern void ReloadWindow();
    }
}
