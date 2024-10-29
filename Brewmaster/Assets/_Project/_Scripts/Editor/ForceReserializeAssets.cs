#if UNITY_EDITOR
using UnityEditor;

namespace Game
{
    public class ForceReserializeAssets
    {
        [MenuItem("AssetDatabase/ForceReserializeAssets")]
        public static void ForceReserialize()
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}
#endif
