using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class PlayerData
    {
        public static PlayerDataClass PlayerDataClass;
        public static string PlayerAddress => PlayerDataClass.Player;
        public static Scale[] PlayerScale => PlayerDataClass.Scale;
        public static int PlayerTreasury => PlayerDataClass.Treasury;
        public static int PlayerPoint => PlayerDataClass.Points;
        public static long CreatedAt => PlayerDataClass.CreatedAt;
    }

    public class PlayerDataClass
    {
        public string Player { get; set; }
        public Scale[] Scale { get; set; }
        public int Treasury { get; set; }
        public int Points { get; set; }
        public long CreatedAt { get; set; }
    }

    public class Scale
    {
        public int TableIndex { get; set; }
        public int Stools { get; set; }
    }
}
