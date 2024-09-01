using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;

namespace Game
{
    public class TwitterShareManager : MonoBehaviorInstance<TwitterShareManager>
    {
        private Action<string> _onSuccess, _onError;

        void Start()
        {
            Utility.Socket.OnEvent("giftTxHash", this.gameObject.name, nameof(OnTwitterCallbackSuccess), OnTwitterCallbackSuccess);
        }

        public void ShareToTwitter()
        {
            string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { TwitterMessage.GetTwitterMessage() } });
            json = Utility.Socket.ToSocketJson(json);
            Debug.Log(json);
#if !UNITY_EDITOR
            JsSocketConnect.EmitEvent(SocketEnum.shareToTwitterRequest.ToString(), json);
#endif
            UIManager.Instance.ShowTwitterInputPanel();
        }

        public void ConfirmTwitterInput(string url, Action<string> callbackSuccess, Action<string> callbackError)
        {
            string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { url } });
            this._onSuccess = callbackSuccess;
            this._onError = callbackError;
            Utility.Socket.SubscribeOnException(this.gameObject.name, nameof(OnTwitterCallbackError), OnTwitterCallbackError);
            Utility.Socket.EmitEvent(SocketEnum.playerInputLink.ToString(), json);
        }

        private async void OnTwitterCallbackSuccess(string data)
        {
            await UniTask.SwitchToMainThread();
            Utility.Socket.UnSubscribeOnException(this.gameObject.name, nameof(OnTwitterCallbackError), OnTwitterCallbackError);
            this._onSuccess?.Invoke(data);
        }
        private async void OnTwitterCallbackError(string data)
        {
            await UniTask.SwitchToMainThread();
            Utility.Socket.UnSubscribeOnException(this.gameObject.name, nameof(OnTwitterCallbackError), OnTwitterCallbackError);
            this._onError?.Invoke(data);
        }
    }
}
