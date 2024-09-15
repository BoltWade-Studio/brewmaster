using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EasyTransition;
using Game.Extension;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;
using Debug = Game.DevelopDebug;

namespace Game
{
    public class TransactionManager : MonoBehaviorInstance<TransactionManager>
    {
        private Dictionary<TransactionID, string> _transactionJsonDataDic = new Dictionary<TransactionID, string>();
        private Dictionary<TransactionID, string> _transactionEntryDic = new Dictionary<TransactionID, string>();
        private bool _gettingData;
        private bool _sending;

        void Start()
        {
            Utility.Socket.OnEvent(SocketEnum.getEntryCallback.ToString(), this.gameObject.name, nameof(GetEntryCallback), GetEntryCallback);
            Utility.Socket.OnEvent(SocketEnum.claimCallback.ToString(), this.gameObject.name, nameof(ClaimCallback), ClaimCallback);
            Utility.Socket.OnEvent(SocketEnum.getPlayerPubCallback.ToString(), this.gameObject.name, nameof(GetPlayerPubCallback), GetPlayerPubCallback);
            Utility.Socket.OnEvent(SocketEnum.getPriceForAddStoolCallback.ToString(), this.gameObject.name, nameof(GetStoolPriceCallback), GetStoolPriceCallback);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Claim();
            }
        }

        #region Private
        private async UniTask<string> GetContractEntry(TransactionID transactionID)
        {
            if (_transactionEntryDic.ContainsKey(transactionID) == false)
            {
                string json = Utility.Socket.StringToSocketJson(transactionID.ToString());
                // Get transaction entry from server
                Utility.Socket.EmitEvent(SocketEnum.getEntry.ToString(), json);
            }

            await UniTask.WaitUntil(() => _transactionEntryDic.ContainsKey(transactionID));
            return _transactionEntryDic[transactionID];
        }
        private void GetEntryCallback(string data)
        {
            Debug.Log("AAA GetEntryCallback: " + data);
            try
            {
                Dictionary<string, string> dataArray = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                string contractName = dataArray["contractName"];
                Debug.Log("Entry callback: " + "ContractName: " + contractName + " Entry: " + dataArray["entry"]);

                // Convert to enum
                TransactionID transactionID = (TransactionID)Enum.Parse(typeof(TransactionID), contractName);

                // Add to dictionary
                if (_transactionEntryDic.ContainsKey(transactionID))
                    _transactionEntryDic[transactionID] = dataArray["entry"];
                else _transactionEntryDic.Add(transactionID, dataArray["entry"]);
            }
            catch (Exception e)
            {
                Debug.Log("AAA GetEntryCallback error: " + e.Message);
            }
        }
        #endregion

        #region GetPlayerPub
        public async UniTask<string> GetPlayerPub()
        {
            _gettingData = true;
            Utility.Socket.EmitEvent(SocketEnum.getPlayerPub.ToString());

            await UniTask.WaitUntil(() => _gettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PLAYER_PUB));

            return _transactionJsonDataDic[TransactionID.GET_PLAYER_PUB];
        }
        private async void GetPlayerPubCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("Player data callback: " + data);
            if (_transactionJsonDataDic.ContainsKey(TransactionID.GET_PLAYER_PUB))
            {
                _transactionJsonDataDic[TransactionID.GET_PLAYER_PUB] = data;
            }
            else
            {
                _transactionJsonDataDic.Add(TransactionID.GET_PLAYER_PUB, data);
            }

            _gettingData = false;
        }
        #endregion

        #region CreatePlayerPub
        public async UniTask CreatePub()
        {
            // SendContract to JSInteropManager
            _sending = true;
            string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
            string createPubEntry = await GetContractEntry(TransactionID.CREATE_PUB);
            Debug.Log("SendContract: " + contractAddress + " Entry: " + createPubEntry);
            string json = JsonConvert.SerializeObject(new ArrayWrapper
            { array = new string[] { } });
            JSInteropManager.SendTransaction(contractAddress, createPubEntry, json, this.gameObject.name, nameof(CreatePubCallback));
            await UniTask.WaitUntil(() => _sending == false);
        }
        private void CreatePubCallback(string data)
        {
            _sending = false;
        }
        #endregion

        #region Claim and ClosingUpPub
        public async UniTask Claim()
        {
            _sending = true;
            LoadingUIManager.Instance.Show("Getting claim data");
            Utility.Socket.EmitEvent(SocketEnum.claim.ToString());
            await UniTask.WaitUntil(() => _sending == false);
        }
        private async void ClaimCallback(string dataArray)
        {
            string jsonData = "";
            object[] strings = JsonConvert.DeserializeObject<object[]>(dataArray);
            string[] strings1 = new string[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                strings1[i] = JsonConvert.SerializeObject(strings[i]);
            }
            jsonData = JsonConvert.SerializeObject(new ArrayWrapper { array = strings1 });
            jsonData = jsonData.RemoveUnWantChar();

            if (Application.isEditor == false)
            {
                string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
                string closingUpPubEntry = await GetContractEntry(TransactionID.CLOSING_UP_PUB);

                LoadingUIManager.Instance.ChangeLoadingMessage("Sending data");
                JSInteropManager.SendTransaction(contractAddress, closingUpPubEntry, jsonData, this.gameObject.name, nameof(ClosingUpPubCallback));
            }
            Debug.Log("claimCallback: " + jsonData);
        }
        private async void ClosingUpPubCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("ClosedUpPubCallback: " + data);
            if (data != "Player abort" && data != "")
            {
                LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for transaction update");
                await UniTask.WaitForSeconds(30f);
                GameEvent.Instance.OnClaimSuccess?.Invoke();
            }
            LoadingUIManager.Instance.Hide();
            _sending = false;
        }
        #endregion

        #region Get Stool Price
        public async UniTask<int> GetStoolPrice()
        {
            _gettingData = true;
            Utility.Socket.EmitEvent(SocketEnum.getPriceForAddStool.ToString());
            await UniTask.WaitUntil(() => _gettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_STOOL));
            return JsonConvert.DeserializeObject<int>(_transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_STOOL]);
        }
        private void GetStoolPriceCallback(string data)
        {
            if (_transactionEntryDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_STOOL))
            {
                _transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_STOOL] = data;
            }
            else
            {
                _transactionJsonDataDic.Add(TransactionID.GET_PRICE_FOR_ADD_STOOL, data);
            }
            _gettingData = false;
        }
        #endregion

        #region Upgrade
        public async UniTask<bool> AddStool(int tableIndex)
        {
            _sending = true;
            if (Application.isEditor == false)
            {
                string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
                string addStoolEntry = await GetContractEntry(TransactionID.ADD_STOOL);

                JSInteropManager.SendTransaction(contractAddress, addStoolEntry, JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { tableIndex.ToString() } }), this.gameObject.name, nameof(AddStoolCallback));
                await UniTask.WaitUntil(() => _sending == false);
                await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.ADD_STOOL));
                return _transactionJsonDataDic[TransactionID.ADD_STOOL].Contains("User Abort") == false;
            }
            return true;
        }
        private async void AddStoolCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            if (_transactionJsonDataDic.ContainsKey(TransactionID.ADD_STOOL))
            {
                _transactionJsonDataDic[TransactionID.ADD_STOOL] = data;
            }
            else
                _transactionJsonDataDic.Add(TransactionID.ADD_STOOL, data);
            _sending = false;
        }
        #endregion        

        private void SendContractCallback(string data)
        {
            _sending = false;
        }
        public async UniTask UpdateSystemManager()
        {
            // SendContract to JSInteropManager
            _sending = true;
            JSInteropManager.SendTransactionArgentX("contractAddress", "entryPoint", "callData", this.gameObject.name, nameof(SendContractCallback));
            await UniTask.WaitUntil(() => _sending == false);
        }
    }

    public enum TransactionID
    {
        CONTRACT_ADDRESS,
        GET_PLAYER_PUB,
        GET_SYSTEM_MANAGER,
        GET_MAX_SCALE,
        GET_PRICE_FOR_ADD_TABLE,
        GET_PRICE_FOR_ADD_STOOL,
        UPDATE_SYSTEM_MANAGER,
        UPDATE_MAX_SCALE,
        UPDATE_UPGRADE_PRICE,
        CREATE_PUB,
        ADD_STOOL,
        ADD_TABLE,
        CLOSING_UP_PUB,
        UPGRADE
    }
}
