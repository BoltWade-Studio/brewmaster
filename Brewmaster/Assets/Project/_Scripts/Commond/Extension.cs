
using System.Collections.Generic;
using UnityEngine;

namespace Game.Extension
{
    public static class Vector3Extension
    {
        public static Vector3 ToVector3XZ (this Vector3 source)
        {
            return new Vector3(source.x, 0, source.z);
        }
        public static Vector3 ChangeX(this Vector3 source, float value)
        {
            return new Vector3(value, source.y, source.z);
        }
        public static Vector3 ChangeY(this Vector3 source, float value)
        {
            return new Vector3(source.x, value, source.z);
        }
        public static Vector3 ChangeZ(this Vector3 source, float value)
        {
            return new Vector3(source.x, source.y, value);
        }
    }
    public static class ListExtension
    {
        public static T GetRandom<T>(this List<T> source)
        {
            int r = Random.Range(0, source.Count - 1);
            return source[r];
        }
    }
    public static class DictionaryExtension
    {
        public static KeyValuePair<TKey, TValue> GetPair<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, source[key]);
        }
    }
}
