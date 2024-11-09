using UnityEngine;
using System.Runtime.InteropServices;

public class FirebaseBridge : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void initializeFirebase();

    [DllImport("__Internal")]
    private static extern void setData(string collection, string doc, string data);

    [DllImport("__Internal")]
    private static extern void getData(string collection, string doc, string callbackName, string functionName);

    private void Start()
    {
        // Initialize Firebase
        initializeFirebase();

        LoadData("server", "serverLink");
    }

    public void SaveData(string collection, string doc, object data)
    {
        string jsonData = JsonUtility.ToJson(data);
        setData(collection, doc, jsonData);
    }

    public void LoadData(string collection, string doc)
    {
        getData(collection, doc, gameObject.name, nameof(OnDataReceived));
    }

    // Callback function to receive data
    public void OnDataReceived(string jsonData)
    {
        // Handle the data as needed
        Debug.Log("Data received from Firebase: " + jsonData);
    }
}
