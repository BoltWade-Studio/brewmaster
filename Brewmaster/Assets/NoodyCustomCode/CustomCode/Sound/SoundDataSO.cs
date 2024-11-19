using System.Linq;
using UnityEngine;
using NOOD.SerializableDictionary;
using NOOD.Extension;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NOOD.Sound
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "SoundData")]
    public class SoundDataSO : ScriptableObject
    {
        [SerializeField] public SerializableDictionary<string, AudioClip> soundDic = new SerializableDictionary<string, AudioClip>();
        [SerializeField] public SerializableDictionary<string, AudioClip> musicDic = new SerializableDictionary<string, AudioClip>();

        [SerializeField] public SerializableDictionary<string, AssetReference> addressableSoundDic = new SerializableDictionary<string, AssetReference>();
        [SerializeField] public SerializableDictionary<string, AssetReference> addressableMusicDic = new SerializableDictionary<string, AssetReference>();

#if UNITY_EDITOR
        public void GenerateSoundEnum()
        {
            GenerateSoundEnumNood();
            AssetDatabase.Refresh();
        }
        private void GenerateSoundEnumNood()
        {
            string rootPath = RootPathExtension<SoundDataSO>.RootPath;
            Debug.Log(rootPath);
            string folderPath = rootPath.Replace("SoundDataSO.cs", "");

            List<string> soundEnumList = soundDic.Dictionary.Keys.ToList();
            soundEnumList.AddRange(addressableSoundDic.Dictionary.Keys.ToList());
            EnumCreator.WriteToEnum<SoundEnum>(folderPath, "SoundEnum", soundEnumList, "NOOD.Sound");

            List<string> musicEnumList = musicDic.Dictionary.Keys.ToList();
            musicEnumList.AddRange(addressableMusicDic.Dictionary.Keys.ToList());
            EnumCreator.WriteToEnum<MusicEnum>(folderPath, "MusicEnum", musicEnumList, "NOOD.Sound");
        }
#endif

        public async UniTask<AudioClip> GetSound(SoundEnum soundEnum)
        {
            if (soundDic.Dictionary.ContainsKey(soundEnum.ToString()))
            {
                return soundDic.Dictionary[soundEnum.ToString()];
            }
            AsyncOperationHandle<AudioClip> handle = addressableSoundDic.Dictionary[soundEnum.ToString()].LoadAssetAsync<AudioClip>();
            await UniTask.WaitUntil(() => handle.Status == AsyncOperationStatus.Succeeded);
            return handle.Result;
        }

        public async UniTask<AudioClip> GetMusic(MusicEnum musicEnum)
        {
            if (musicDic.Dictionary.ContainsKey(musicEnum.ToString()))
            {
                return musicDic.Dictionary[musicEnum.ToString()];
            }
            AsyncOperationHandle<AudioClip> handle = addressableMusicDic.Dictionary[musicEnum.ToString()].LoadAssetAsync<AudioClip>();
            await UniTask.WaitUntil(() => handle.Status == AsyncOperationStatus.Succeeded);
            return handle.Result;
        }
    }

}
