using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class PointDisplay
    {
        private static readonly string[] CURRENCY_SYMBOL = { "", "K", "M" };

        public static string GetPointDisplayText(int point)
        {
            int symbolIndex = 0;
            float temp = point;
            while (temp >= 1000 && symbolIndex < CURRENCY_SYMBOL.Length - 1)
            {
                temp /= 1000;
                symbolIndex++;
            }
            return temp.ToString("F1") + CURRENCY_SYMBOL[symbolIndex];
        }
    }
}
