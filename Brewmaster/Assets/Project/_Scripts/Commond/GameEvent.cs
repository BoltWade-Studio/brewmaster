using System;
using UnityEngine;

namespace Game
{
    public class GameEvent
    {
        private static object obj = new object();
        public static GameEvent Instance
        {

            get
            {
                lock (obj)
                {
                    if (s_instance == null || s_initGO == null || s_initGO.gameObject == null)
                    {
                        s_instance = new GameEvent();
                        s_initGO = new GameObject("Game Event Init");
                        s_initGO.AddComponent<DontDestroyOnLoad>();
                    }
                    return s_instance;

                }
            }
        }
        private static GameEvent s_instance;
        private static GameObject s_initGO;

        public Action OnStorePhase;
        public Action OnNextDay;
        public Action OnEndDay;
        public Action OnTimeUp;
        public Action OnClaimSuccess;
        public Action OnClaimFail;
        public Action OnLoadDataSuccess;
        public Action OnUpgradeComplete;
        public Action OnUpdatePosComplete;
    }
}
