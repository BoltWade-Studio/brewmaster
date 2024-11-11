using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using System;

public class FirebaseBridge : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void initializeFirebase();

    [DllImport("__Internal")]
    private static extern void setFirestoreData(string collection, string doc, string data);

    [DllImport("__Internal")]
    private static extern void getFirestoreData(string collection, string doc, string callbackName, string functionName);

    [DllImport("__Internal")]
    private static extern void getDownloadUrl(string path, string callbackName, string functionName);

    private Dictionary<string, UnityWebRequest> _downloadedFiles = new Dictionary<string, UnityWebRequest>();

    private void Start()
    {
        // Initialize Firebase
        initializeFirebase();

        LoadFirebaseData("server", "serverLink");
    }

    #region Cloud Firestore
    public void SaveFirebaseData(string collection, string doc, object data)
    {
        string jsonData = JsonUtility.ToJson(data);
        setFirestoreData(collection, doc, jsonData);
    }

    public void LoadFirebaseData(string collection, string doc)
    {
        getFirestoreData(collection, doc, this.gameObject.name, nameof(OnDataReceived));
    }

    private void OnDataReceived(string jsonData)
    {
        // Handle the data as needed
        Debug.Log("Data received from Firebase: " + jsonData);
    }
    #endregion

    #region Firebase Storage
    public async UniTask<Sprite> LoadSprite(string filePath)
    {
        if (_downloadedFiles.ContainsKey(filePath))
        {
            _downloadedFiles.Remove(filePath);
        }
        getDownloadUrl(filePath, this.gameObject.name, nameof(OnUrlReceive));
        await UniTask.WaitUntil(() => _downloadedFiles.ContainsKey(filePath));
        Texture2D texture2D = DownloadHandlerTexture.GetContent(_downloadedFiles[filePath]);
        return ImageFactor.CreateSprite(texture2D);
    }
    private async void OnUrlReceive(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            await unityWebRequest.SendWebRequest();
            await UniTask.WaitUntil(() => unityWebRequest.isDone);
            _downloadedFiles.Add(Path.GetFileName(url), unityWebRequest);
        }
        else
        {
            Debug.Log("Do not have url: ");
        }
    }
    #endregion

}
