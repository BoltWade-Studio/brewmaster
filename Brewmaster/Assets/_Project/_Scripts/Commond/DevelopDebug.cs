using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class DevelopDebug
    {
        public static void Log(string message)
        {
            if (Debug.isDebugBuild)
            {
                UnityEngine.Debug.Log(message);
            }
        }
        public static void LogError(string message)
        {
            if (Debug.isDebugBuild)
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}
