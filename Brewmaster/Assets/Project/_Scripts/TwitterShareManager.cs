using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;
using Debug = Game.DevelopDebug;

namespace Game
{
    public class TwitterShareManager : MonoBehaviorInstance<TwitterShareManager>
    {
        private Action<string> _onSuccess, _onError;
        private string _twitterMessage;
        private bool _isGettingData = false;

        void Start()
        {
            Utility.Socket.OnEvent("giftTxHash", this.gameObject.name, nameof(OnTwitterCallbackSuccess), OnTwitterCallbackSuccess);
            Utility.Socket.OnEvent(SocketEnum.getTwitterMessageCallback.ToString(), this.gameObject.name, nameof(GetTwitterMessageCallback), GetTwitterMessageCallback);
            GameEvent.Instance.OnClaimSuccess += OnClaimSuccessHandler;
        }

        private void OnClaimSuccessHandler()
        {
            ActiveShareToXPopup();
        }

        public void ActiveShareToXPopup()
        {
            PopupManager.Instance.ShowYesNoPopup("Share to X", "Share to X to get rewards", () =>
            {
                OpenTwitterNewTab();
                UIManager.Instance.ShowTwitterInputPanel();
            }, null);
        }

        public void OpenTwitterNewTab()
        {
            _isGettingData = true;
#if !UNITY_EDITOR
            // Get message on server
            JsSocketConnect.EmitEvent(SocketEnum.shareToTwitterRequest.ToString());
#endif
        }

        private void GetTwitterMessageCallback(string message)
        {
            _twitterMessage = message;
            _isGettingData = false;
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
