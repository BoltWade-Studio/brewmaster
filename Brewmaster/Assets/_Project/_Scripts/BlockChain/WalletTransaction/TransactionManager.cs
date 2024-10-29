using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private Dictionary<TransactionID, object> _transactionJsonDataDic = new Dictionary<TransactionID, object>();
        private Dictionary<TransactionID, string> _transactionEntryDic = new Dictionary<TransactionID, string>();
        private bool _isGettingData;
        private bool _isSendingData;
        private bool _isClaimSuccess;
        private bool _isTransactionSending;

        void Start()
        {
            Utility.Socket.SubscribeEvent(SocketEnum.getEntryCallback.ToString(), this.gameObject.name, nameof(GetEntryCallback), GetEntryCallback);
            Utility.Socket.SubscribeEvent(SocketEnum.claimCallback.ToString(), this.gameObject.name, nameof(ClaimCallback), ClaimCallback);
            Utility.Socket.SubscribeEvent(SocketEnum.getPlayerPubCallback.ToString(), this.gameObject.name, nameof(GetPlayerPubCallback), GetPlayerPubCallback);
        }

        #region Private
        private async UniTask<string> GetContractEntry(TransactionID transactionID)
        {
            if (_transactionEntryDic.ContainsKey(transactionID) == false)
            {
                string json = Utility.Socket.StringToSocketJson(transactionID.ToString(CultureInfo.InvariantCulture));
                // Get transaction entry from server
                Utility.Socket.EmitEvent(SocketEnum.getEntry.ToString(), json);
            }

            await UniTask.WaitUntil(() => _transactionEntryDic.ContainsKey(transactionID));
            return _transactionEntryDic[transactionID].ToString();
        }
        private async void GetEntryCallback(string data)
        {
            Debug.Log("AAA GetEntryCallback: " + data);
            try
            {
                await UniTask.SwitchToMainThread();
                object objectData = JsonConvert.DeserializeObject<object>(data.ToString());
                Dictionary<string, string> dataDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(objectData.ToString());
                string contractName = dataDic["contractName"];
                Debug.Log("Entry callback: " + "ContractName: " + contractName + " Entry: " + dataDic["entry"]);

                // Convert to enum
                TransactionID transactionID = (TransactionID)Enum.Parse(typeof(TransactionID), contractName);

                // Add to dictionary
                if (_transactionEntryDic.ContainsKey(transactionID))
                    _transactionEntryDic[transactionID] = dataDic["entry"];
                else _transactionEntryDic.Add(transactionID, dataDic["entry"]);

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
            _isGettingData = true;
            LoadingUIManager.Instance.ChangeLoadingMessage("Getting player pub");
            if (_transactionJsonDataDic.ContainsKey(TransactionID.GET_PLAYER_PUB))
                _transactionJsonDataDic.Remove(TransactionID.GET_PLAYER_PUB);
            Utility.Socket.EmitEvent(SocketEnum.getPlayerPub.ToString(CultureInfo.InvariantCulture));
            await UniTask.WaitUntil(() => _isGettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PLAYER_PUB));

            return _transactionJsonDataDic[TransactionID.GET_PLAYER_PUB].ToString();
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

            _isGettingData = false;
        }
        #endregion

        #region CreatePlayerPub
        public async UniTask<bool> CreatePub()
        {
            // SendContract to JSInteropManager
            _isSendingData = true;
            string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
            string createPubEntry = await GetContractEntry(TransactionID.CREATE_PUB);
            Debug.Log("SendContract: " + contractAddress + " Entry: " + createPubEntry);
            string json = JsonConvert.SerializeObject(new ArrayWrapper
            { array = new string[] { } });

            LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for create new pub");

            JSInteropManager.SendTransaction(contractAddress, createPubEntry, json, this.gameObject.name, nameof(CreatePubCallback));

            await UniTask.WaitUntil(() => _isSendingData == false);
            return _transactionJsonDataDic[TransactionID.CREATE_PUB].Equals("true");
        }
        private async void CreatePubCallback(string txHash)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("Create pub callback data: " + txHash);
            string result = "false";

            if (IsValidTransactionHash(txHash))
            {
                // This is a transaction hash, wait for transaction
                bool transactionResult = await WaitTransaction(txHash);
                result = transactionResult.ToString().ToLower();
            }
            else
            {
                // User abort
                result = "false";
            }

            if (_transactionJsonDataDic.ContainsKey(TransactionID.CREATE_PUB))
            {
                _transactionJsonDataDic[TransactionID.CREATE_PUB] = result;
            }
            else
            {
                _transactionJsonDataDic.Add(TransactionID.CREATE_PUB, result);
            }
            _isSendingData = false;
        }
        #endregion

        #region Claim and ClosingUpPub
        public async UniTask Claim()
        {
            _isSendingData = true;
            LoadingUIManager.Instance.ChangeLoadingMessage("Getting claim data");
            Utility.Socket.EmitEvent(SocketEnum.claim.ToString(CultureInfo.InvariantCulture));
            await UniTask.WaitUntil(() => _isSendingData == false);
        }
        private async void ClaimCallback(string dataArray)
        {
            // Receive data from server and send to blockchain
            await UniTask.SwitchToMainThread();
            // Convert data to json
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
                // Is not editor
                string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
                string closingUpPubEntry = await GetContractEntry(TransactionID.CLOSING_UP_PUB);

                LoadingUIManager.Instance.ChangeLoadingMessage("Sending data");
                JSInteropManager.SendTransaction(contractAddress, closingUpPubEntry, jsonData, this.gameObject.name, nameof(ClosingUpPubCallback));
            }
            else
            {
                // Is editor
                _isSendingData = false;
                GameEvent.Instance.OnClaimSuccess?.Invoke();
            }
            Debug.Log("claimCallback: " + jsonData);
        }
        private async void ClosingUpPubCallback(string txHash)
        {
            await UniTask.SwitchToMainThread();
            if (IsValidTransactionHash(txHash)) // this is transaction hash
            {
                LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for transaction update");

                // Use existing WaitTransaction method
                bool result = await WaitTransaction(txHash);

                if (result)
                {
                    GameEvent.Instance.OnClaimSuccess?.Invoke();
                }
                else
                {
                    NotifyManager.Instance.Show("Transaction failed");
                    GameEvent.Instance.OnClaimFail?.Invoke();
                }
            }
            else
            {
                NotifyManager.Instance.Show("Execute failed or user abort");
                GameEvent.Instance.OnClaimFail?.Invoke();
            }
            _isSendingData = false;
        }
        #endregion

        #region Get Og Pass Balance
        public async UniTask<int> GetOgPassBalance()
        {
            _isGettingData = true;
            if (_transactionJsonDataDic.ContainsKey(TransactionID.GET_OG_PASS_BALANCE))
                _transactionJsonDataDic.Remove(TransactionID.GET_OG_PASS_BALANCE);
            Utility.Socket.SubscribeEvent(SocketEnum.getOgPassBalanceCallback.ToString(), this.gameObject.name, nameof(GetOgPassBalanceCallback), GetOgPassBalanceCallback);
            Utility.Socket.EmitEvent(SocketEnum.getOgPassBalance.ToString(CultureInfo.InvariantCulture));
            await UniTask.WaitUntil(() => _isGettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_OG_PASS_BALANCE));
            return JsonConvert.DeserializeObject<int>(_transactionJsonDataDic[TransactionID.GET_OG_PASS_BALANCE].ToString());
        }
        private async void GetOgPassBalanceCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            _transactionJsonDataDic.Add(TransactionID.GET_OG_PASS_BALANCE, data);
            _isGettingData = false;
            Utility.Socket.UnSubscribeEvent(SocketEnum.getOgPassBalanceCallback.ToString(), this.gameObject.name, nameof(GetOgPassBalanceCallback), GetOgPassBalanceCallback);
        }
        #endregion

        #region Get Price For Add Stool
        public async UniTask<int> GetPriceForAddStool()
        {
            _isGettingData = true;
            Utility.Socket.SubscribeEvent(SocketEnum.getPriceForAddStoolCallback.ToString(), this.gameObject.name, nameof(GetStoolPriceCallback), GetStoolPriceCallback);
            if (_transactionEntryDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_STOOL))
                _transactionJsonDataDic.Remove(TransactionID.GET_PRICE_FOR_ADD_STOOL);
            Utility.Socket.EmitEvent(SocketEnum.getPriceForAddStool.ToString());
            await UniTask.WaitUntil(() => _isGettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_STOOL));
            return JsonConvert.DeserializeObject<int>(_transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_STOOL].ToString());
        }
        private async void GetStoolPriceCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            if (_transactionJsonDataDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_STOOL))
            {
                _transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_STOOL] = data;
            }
            else
            {
                _transactionJsonDataDic.Add(TransactionID.GET_PRICE_FOR_ADD_STOOL, data);
            }
            _isGettingData = false;
            Utility.Socket.UnSubscribeEvent(SocketEnum.getPriceForAddStoolCallback.ToString(), this.gameObject.name, nameof(GetStoolPriceCallback), GetStoolPriceCallback);
        }
        #endregion

        #region Get Price For Add Table
        public async UniTask<int> GetPriceForAddTable()
        {
            _isGettingData = true;
            Utility.Socket.SubscribeEvent(SocketEnum.getPriceForAddTableCallback.ToString(), this.gameObject.name, nameof(GetPriceForAddTableCallback), GetPriceForAddTableCallback);
            if (_transactionEntryDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_TABLE))
                _transactionJsonDataDic.Remove(TransactionID.GET_PRICE_FOR_ADD_TABLE);
            Utility.Socket.EmitEvent(SocketEnum.getPriceForAddTable.ToString());
            await UniTask.WaitUntil(() => _isGettingData == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_TABLE));
            return JsonConvert.DeserializeObject<int>(_transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_TABLE].ToString());
        }
        private async void GetPriceForAddTableCallback(string data)
        {
            await UniTask.SwitchToMainThread();
            if (_transactionJsonDataDic.ContainsKey(TransactionID.GET_PRICE_FOR_ADD_TABLE))
            {
                _transactionJsonDataDic[TransactionID.GET_PRICE_FOR_ADD_TABLE] = data;
            }
            else
            {
                _transactionJsonDataDic.Add(TransactionID.GET_PRICE_FOR_ADD_TABLE, data);
            }
            Utility.Socket.UnSubscribeEvent(SocketEnum.getPriceForAddTableCallback.ToString(), this.gameObject.name, nameof(GetPriceForAddTableCallback), GetPriceForAddTableCallback);
            _isGettingData = false;
        }
        #endregion

        #region Upgrade
        /// <summary>
        /// Return true if transaction is successful, false if transaction is failed or user abort
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public async UniTask<bool> AddStool(int tableIndex)
        {
            _isSendingData = true;
            if (Application.isEditor == false)
            {
                Debug.Log("AddStool: " + tableIndex);
                string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
                string addStoolEntry = await GetContractEntry(TransactionID.ADD_STOOL);

                // Send transaction to blockchain
                if (_transactionJsonDataDic.ContainsKey(TransactionID.ADD_STOOL))
                    _transactionJsonDataDic.Remove(TransactionID.ADD_STOOL);
                LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for player confirmation");
                JSInteropManager.SendTransaction(contractAddress, addStoolEntry, JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { tableIndex.ToString() } }), this.gameObject.name, nameof(AddStoolCallback));
                await UniTask.WaitUntil(() => _isSendingData == false);
                await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.ADD_STOOL));

                // return true if transaction is successful, false if transaction is failed or user abort
                return _transactionJsonDataDic[TransactionID.ADD_STOOL].ToString().ToLower() == "true";
            }
            else
            {
                _isSendingData = false;
                return true;
            }
        }

        private async void AddStoolCallback(string txHash)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("AddStoolCallback: " + txHash);

            if (IsValidTransactionHash(txHash) == false) // txHash is not valid
            {
                NotifyManager.Instance.Show("Execute failed or user abort");
                _transactionJsonDataDic.Add(TransactionID.ADD_STOOL, "false");
                _isSendingData = false;
                return;
            }

            // txHash is valid
            bool isTransactionSuccess = await WaitTransaction(txHash);

            if (isTransactionSuccess)
            {
                // Transaction is successful
                NotifyManager.Instance.Show("Transaction successful");
            }
            else
            {
                // Transaction is failed
                NotifyManager.Instance.Show("Transaction failed");
            }
            _transactionJsonDataDic.Add(TransactionID.ADD_STOOL, isTransactionSuccess.ToString().ToLower());
            _isSendingData = false;
        }

        public async UniTask<bool> AddTable()
        {
            _isSendingData = true;
            if (Application.isEditor == false)
            {
                Debug.Log("AddTable: ");
                string contractAddress = await GetContractEntry(TransactionID.CONTRACT_ADDRESS);
                string addTableEntry = await GetContractEntry(TransactionID.ADD_TABLE);

                // Send transaction to blockchain
                if (_transactionJsonDataDic.ContainsKey(TransactionID.ADD_TABLE))
                    _transactionJsonDataDic.Remove(TransactionID.ADD_TABLE);
                LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for player confirmation");
                JSInteropManager.SendTransaction(contractAddress, addTableEntry, JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { } }), this.gameObject.name, nameof(AddTableCallback));
                await UniTask.WaitUntil(() => _isSendingData == false);
                await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.ADD_TABLE));

                // return true if transaction is successful, false if transaction is failed or user abort
                return _transactionJsonDataDic[TransactionID.ADD_TABLE].ToString().ToLower() == "true";
            }
            else
            {
                _isSendingData = false;
                return true;
            }
        }

        private async void AddTableCallback(string txHash)
        {
            await UniTask.SwitchToMainThread();
            Debug.Log("AddTableCallback: " + txHash);

            if (IsValidTransactionHash(txHash) == false) // txHash is not valid
            {
                _transactionJsonDataDic.Add(TransactionID.ADD_TABLE, "false");
                _isSendingData = false;
                return;
            }

            // txHash is valid, wait for transaction done
            bool isTransactionSuccess = await WaitTransaction(txHash);

            if (isTransactionSuccess)
            {
                // Transaction is successful
                NotifyManager.Instance.Show("Transaction successful");
            }
            else
            {
                // Transaction is failed
                NotifyManager.Instance.Show("Transaction failed");
            }
            _transactionJsonDataDic.Add(TransactionID.ADD_TABLE, isTransactionSuccess.ToString().ToLower());
            _isSendingData = false;
        }
        #endregion

        #region Wait Transaction
        private async UniTask<bool> WaitTransaction(string txHash)
        {
            if (IsValidTransactionHash(txHash) == false)
            {
                Debug.LogError("Invalid transaction hash");
                return false;
            }

            LoadingUIManager.Instance.ChangeLoadingMessage("Waiting for transaction update");
            if (_transactionJsonDataDic.ContainsKey(TransactionID.WAIT_TRANSACTION))
                _transactionJsonDataDic.Remove(TransactionID.WAIT_TRANSACTION);
            _isTransactionSending = true;
            string socketString = Utility.Socket.StringToSocketJson(txHash);
            Utility.Socket.SubscribeEvent(SocketEnum.waitTransactionCallback.ToString(), this.gameObject.name, nameof(WaitTransactionCallback), WaitTransactionCallback);
            Utility.Socket.EmitEvent(SocketEnum.waitTransaction.ToString(), socketString);

            await UniTask.WaitUntil(() => _isTransactionSending == false);
            await UniTask.WaitUntil(() => _transactionJsonDataDic.ContainsKey(TransactionID.WAIT_TRANSACTION));
            await UniTask.WaitForSeconds(1f); // Wait extra 1s

            string result = _transactionJsonDataDic[TransactionID.WAIT_TRANSACTION].ToString();
            return result.ToLower() == "true";
        }
        private async void WaitTransactionCallback(string response)
        {
            await UniTask.SwitchToMainThread();
            // Transaction is completed callback
            Debug.Log("WaitTransactionCallback: " + response.ToString());
            if (_transactionJsonDataDic.ContainsKey(TransactionID.WAIT_TRANSACTION))
                _transactionJsonDataDic[TransactionID.WAIT_TRANSACTION] = response.ToString();
            else
                _transactionJsonDataDic.Add(TransactionID.WAIT_TRANSACTION, response.ToString());

            Utility.Socket.UnSubscribeEvent(SocketEnum.waitTransactionCallback.ToString(), this.gameObject.name, nameof(WaitTransactionCallback), WaitTransactionCallback);

            _isTransactionSending = false;
        }
        #endregion

        private bool IsValidTransactionHash(string txHash)
        {
            return !string.IsNullOrEmpty(txHash) && (txHash.Length >= 60);
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
        UPGRADE,
        WAIT_TRANSACTION,
        GET_OG_PASS_BALANCE,
    }
}
