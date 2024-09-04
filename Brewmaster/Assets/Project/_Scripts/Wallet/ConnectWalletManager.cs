using System;
using System.Collections;
using Newtonsoft.Json;
using NOOD;
using StarkSharp.Settings;
using UnityEngine;
using Utils;

namespace Game
{
	public static class PlayerData
	{
		public static string PlayerAddress = "0x000000000";
		public static int TotalPoint = 0;

		public static void Reset()
		{
			PlayerAddress = "0x000000000";
			TotalPoint = 0;
		}
	}

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
		private string contractAddress = "0x7bd89ba87f34b47facaeb4d408dadd1915d16a6c828d7ba55692eb705f0a5cc";

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
			Utility.Socket.OnEvent(SocketEnum.totalPointCallback.ToString(), this.gameObject.name, nameof(OnUpdateTotalPoint), OnUpdateTotalPoint);
		}


		public void StartConnectWallet(Action onSuccess)
		{
			_onSuccess = onSuccess;
			PlayerData.Reset();
			_gameUITransform = GameObject.FindGameObjectWithTag("MainMenuCanvas").transform;
			_connectWalletUI = Instantiate<ConnectWalletUI>(_connectWalletUIPref, _gameUITransform);
			_connectWalletUI.Open();
			_connectWalletUI.onArgentXClick = ConnectArgentX;
			_connectWalletUI.onBraavosClick = ConnectBraavos;
			_connectWalletUI.onAnonymousClick = ConnectAnonymous;
		}

		private void ConnectArgentX()
		{
			StartCoroutine(WalletConnectAsync(JSInteropManager.ConnectWalletArgentX));
			isAnonymous = false;
		}
		private void ConnectBraavos()
		{
			StartCoroutine(WalletConnectAsync(JSInteropManager.ConnectWalletBraavos));
			isAnonymous = false;
		}
		private void ConnectAnonymous()
		{
			// Request address and private key from server
			Utility.Socket.EmitEvent(SocketEnum.anonymousLogin.ToString());
			Utility.Socket.EmitEvent(SocketEnum.requestUpdateTotalPoint.ToString());
			isAnonymous = true;

			// Hide UI
			_connectWalletUI.Close();
			_onSuccess?.Invoke();
		}

		private void OnUpdateTotalPoint(string point)
		{
			Debug.Log("Update total point" + point);
			PlayerData.TotalPoint = Convert.ToInt32(point);
			MoneyManager.Instance.toUpdatePoint = true;
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

			PlayerData.PlayerAddress = dataArray[0];
		}

		private IEnumerator WalletConnectAsync(Action walletAction)
		{
			walletAction?.Invoke();
			yield return new WaitUntil(() => JSInteropManager.IsConnected());

			PlayerData.PlayerAddress = JSInteropManager.GetAccount();
			string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { PlayerData.PlayerAddress } });
			Utility.Socket.EmitEvent("updatePlayerAddress", json);
			_connectWalletUI.Close();
			_onSuccess.Invoke();
			// SyncPlayerPoint()
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
				// if (Application.isEditor)
				// 	SocketConnectManager.Instance.EmitEvent("afterClaim");
				// else
				// 	JsSocketConnect.EmitEvent("afterClaim");
			}
		}
	}
}
