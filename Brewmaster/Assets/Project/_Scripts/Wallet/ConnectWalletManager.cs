using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using StarkSharp.Settings;
using UnityEngine;
using Utils;

namespace Game
{

	public class ProofClass
	{
		public string[] address;
		public int point;
		public int timestamp;
		public string[] proof;
	}

	public class ConnectWalletManager : MonoBehaviorInstance<ConnectWalletManager>
	{
		[SerializeField] private Transform _gameUITransform;
		[SerializeField] private ConnectWalletUI _connectWalletUIPref;

		public bool isAnonymous = false;

		private ConnectWalletUI _connectWalletUI;
		private Action _onSuccess;
		private string contractAddress = "0x76f7bf89ee554e8e182be67600e006fc543339f6074d04ce5c86515525539bd";

		protected override void ChildAwake()
		{
			base.ChildAwake();
#if UNITY_EDITOR
			Utility.Socket.Init();
#else
			JsSocketConnect.SocketIOInit();
#endif
		}

		void Start()
		{
			Utility.Socket.OnEvent(SocketEnum.updateProof.ToString(), this.gameObject.name, nameof(OnUpdateProof), OnUpdateProof);
			Utility.Socket.OnEvent(SocketEnum.updateAnonymous.ToString(), this.gameObject.name, nameof(OnUpdateAnonymous), OnUpdateAnonymous);
		}


		public void StartConnectWallet(Action onSuccess)
		{
			_onSuccess = onSuccess;
			_gameUITransform = GameObject.FindGameObjectWithTag("MainMenuCanvas").transform;
			_connectWalletUI = Instantiate<ConnectWalletUI>(_connectWalletUIPref, _gameUITransform);
			_connectWalletUI.Open();
			_connectWalletUI.onArgentXClick = ConnectArgentX;
			_connectWalletUI.onBraavosClick = ConnectBraavos;
			_connectWalletUI.onAnonymousClick = ConnectAnonymous;
		}

		private void ConnectArgentX()
		{
			WalletConnectAsync(JSInteropManager.ConnectWalletArgentX);
			isAnonymous = false;
		}
		private void ConnectBraavos()
		{
			WalletConnectAsync(JSInteropManager.ConnectWalletBraavos);
			isAnonymous = false;
		}
		private void ConnectAnonymous()
		{
			// Request address and private key from server
			Utility.Socket.EmitEvent(SocketEnum.anonymousLogin.ToString());
			Utility.Socket.EmitEvent(SocketEnum.requestUpdatePlayerTreasury.ToString());
			isAnonymous = true;

			// Hide UI
			_connectWalletUI.Close();
			_onSuccess?.Invoke();
		}

		private void OnUpdateProof(string proof)
		{
			Settings.apiurl = "https://starknet-mainnet.public.blastapi.io/rpc/v0_7";
			ProofClass proofClass = JsonConvert.DeserializeObject<ProofClass>(proof);
			string[] calldata = new string[2];
			calldata[0] = proofClass.point.ToString();
			calldata[1] = proofClass.timestamp.ToString();
			string proofArray = $",[\"{proofClass.proof[0]}\", \"{proofClass.proof[1]}\"]";

			string callDataString = JsonUtility.ToJson(new ArrayWrapper { array = calldata });
			callDataString = callDataString.Replace("]}", "");
			callDataString = callDataString + proofArray + "]}";

			Debug.Log("Proof data: " + callDataString);

			// Debug.Log("callDataString: " + callDataString);
			JSInteropManager.SendTransaction(contractAddress, "rewardPoint", callDataString, gameObject.name, nameof(ClaimCallback));
		}

		private void OnUpdateAnonymous(string data)
		{
			Debug.Log("Update anonymous: " + data);
			string[] dataArray = JsonConvert.DeserializeObject<string[]>(data);

			// PlayerData.PlayerAddress = dataArray[0];
		}

		private async void WalletConnectAsync(Action walletAction)
		{
			string playerAddress;
			if (Application.isEditor)
			{
				playerAddress = Utility.Socket.StringToSocketJson("0x0449A464Bce984F1d1d987B3a1Ff91bE490da7115f9776c1c7FED2ba3c5d14C0");
			}
			else
			{
				walletAction?.Invoke();
				await UniTask.WaitUntil(() => JSInteropManager.IsConnected());
				playerAddress = Utility.Socket.StringToSocketJson(JSInteropManager.GetAccount());
			}

			Utility.Socket.EmitEvent(SocketEnum.updatePlayerAddress.ToString(), playerAddress);
			await DataSaveLoadManager.Instance.LoadData();
			_connectWalletUI.Close();
			_onSuccess?.Invoke();
		}

		public void Claim()
		{
			if (PlayerData.TotalPoint > 0)
			{
				string[] datas = new string[] {
					PlayerData.PlayerAddress
				};
				Utility.Socket.EmitEvent(SocketEnum.claim.ToString(), JsonConvert.SerializeObject(new ArrayWrapper { array = datas }));
			}
		}

		public void SyncPlayerPoint()
		{
			Settings.apiurl = "https://starknet-mainnet.public.blastapi.io/rpc/v0_7";

			string[] calldata = new string[1];
			calldata[0] = PlayerData.PlayerAddress;
			string calldataString = JsonUtility.ToJson(new ArrayWrapper { array = calldata });
			// JSInteropManager.CallContract(contractAddress, "getUserPoint", calldataString, gameObject.name, nameof(PlayerPointCallback));
		}
		private void PlayerPointCallback(string response)
		{
			JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(response);
			int balance = Convert.ToInt32(jsonResponse.result[0], 16);
			// UIManager.Instance.UpdatePlayerInfoPanel();
		}

		private void ClaimCallback(string response)
		{
			Debug.Log("claim response: " + response);
			if (response == "User abort" || response == "Execute failed")
			{
				// user decline
				Debug.Log("Response: " + response);
			}
			else
			{
				// user claim
				SyncPlayerPoint();
				Utility.Socket.EmitEvent(SocketEnum.afterClaim.ToString());
			}
		}
	}
}
