using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EasyTransition;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Utils;

namespace Game
{
    public class TransactionManager : MonoBehaviorInstance<TransactionManager>
    {
        private Dictionary<TransactionID, string> _transactionEntryDic = new Dictionary<TransactionID, string>();
        private bool _gettingData;
        private bool _sending;
        private Dictionary<TransactionID, string> _transactionJsonDataDic = new Dictionary<TransactionID, string>();

        void Start()
        {
            Utility.Socket.OnEvent(SocketEnum.getEntryCallback.ToString(), this.gameObject.name, nameof(GetEntryCallback), GetEntryCallback);
            Utility.Socket.OnEvent(SocketEnum.getPlayerPubCallback.ToString(), this.gameObject.name, nameof(GetPlayerPubCallback), GetPlayerPubCallback);
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

        #endregion

        #region Callback functions
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
        private void CreatePubCallback(string data)
        {
            _sending = false;
        }
        private void SendContractCallback(string data)
        {
            _sending = false;
        }
        #endregion

        #region Read data
        public async UniTask<string> GetPlayerPub()
        {
            _gettingData = true;
            Utility.Socket.EmitEvent(SocketEnum.getPlayerPub.ToString());
            await UniTask.WaitUntil(() => _gettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PLAYER_PUB));

            return _transactionJsonDataDic[TransactionID.GET_PLAYER_PUB];
        }
        #endregion

        #region Write data
        public async UniTask UpdateSystemManager()
        {
            // SendContract to JSInteropManager
            _sending = true;
            JSInteropManager.SendTransactionArgentX("contractAddress", "entryPoint", "callData", this.gameObject.name, nameof(SendContractCallback));
            await UniTask.WaitUntil(() => _sending == false);
        }
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
        #endregion
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
        CLOSING_PUB,
        UPGRADE
    }
}
