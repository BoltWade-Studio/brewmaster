using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class LeaderboardResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public LeaderboardData Data { get; set; }
    }

    public class LeaderboardData
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("items")]
        public List<LeaderboardItem> Items { get; set; }

        [JsonProperty("pages")]
        public int Pages { get; set; }

        [JsonProperty("hasNext")]
        public bool HasNext { get; set; }

        [JsonProperty("hasPrevious")]
        public bool HasPrevious { get; set; }
    }

    public class LeaderboardItem
    {
        [JsonProperty("point")]
        public int Point { get; set; }

        [JsonProperty("userAddress")]
        public string UserAddress { get; set; }
    }

}
