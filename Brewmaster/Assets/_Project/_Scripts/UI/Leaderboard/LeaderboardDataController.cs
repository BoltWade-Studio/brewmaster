using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class OnGetRemoteDataCompleteArgs
    {
        public bool IsWeekly;
    }

    public class LeaderboardDataController : MonoBehaviour
    {
        public Action<OnGetRemoteDataCompleteArgs> OnGetRemoteDataComplete;
        private const string LEADERBOARD_ALL_TIME_DATA_KEY = "LeaderboardAllTime";
        private const string LEADERBOARD_WEEKLY_DATA_KEY = "LeaderboardWeekly";
        private float _refreshTime = 0;

        public void GetRemoteData()
        {
            if (Time.time - _refreshTime > 10)
            {
                _refreshTime = Time.time;
                GetWeeklyData();
                GetAllTimeLeaderboardData();
            }
        }

        private void GetWeeklyData()
        {
            Utility.Socket.SubscribeEvent(SocketEnum.getWeeklyLeaderboardCallback.ToString(), this.gameObject.name, nameof(OnGetWeeklyLeaderboardCallback), OnGetWeeklyLeaderboardCallback);
            Utility.Socket.EmitEvent(SocketEnum.getWeeklyLeaderboard.ToString());
        }

        private void GetAllTimeLeaderboardData()
        {
            Utility.Socket.SubscribeEvent(SocketEnum.getLeaderboardCallback.ToString(), this.gameObject.name, nameof(OnGetLeaderboardCallback), OnGetLeaderboardCallback);
            Utility.Socket.EmitEvent(SocketEnum.getLeaderboard.ToString());
        }

        private async void OnGetWeeklyLeaderboardCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            PlayerPrefs.SetString(LEADERBOARD_WEEKLY_DATA_KEY, data);
            PlayerPrefs.Save();
            Utility.Socket.UnSubscribeEvent(SocketEnum.getWeeklyLeaderboardCallback.ToString(), this.gameObject.name, nameof(OnGetWeeklyLeaderboardCallback), OnGetWeeklyLeaderboardCallback);
            OnGetRemoteDataComplete?.Invoke(new OnGetRemoteDataCompleteArgs
            {
                IsWeekly = true,
            });
        }

        private async void OnGetLeaderboardCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            PlayerPrefs.SetString(LEADERBOARD_ALL_TIME_DATA_KEY, data);
            PlayerPrefs.Save();
            Utility.Socket.UnSubscribeEvent(SocketEnum.getLeaderboardCallback.ToString(), this.gameObject.name, nameof(OnGetLeaderboardCallback), OnGetLeaderboardCallback);
            OnGetRemoteDataComplete?.Invoke(new OnGetRemoteDataCompleteArgs
            {
                IsWeekly = false,
            });
        }
    }
}
