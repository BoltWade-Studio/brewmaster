using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using NOOD;
using UnityEngine;
using Debug = Game.DevelopDebug;

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

		public bool IsAnonymous = false;

		private ConnectWalletUI _connectWalletUI;
		private Action _onSuccess;

		protected override void ChildAwake()
		{
			base.ChildAwake();
			if (ConnectWalletManager.Instance != null)
			{
				Destroy(this.gameObject);
			}
#if UNITY_EDITOR
			Debug.Log("Socket init");
			Utility.Socket.Init();
#else
			JsSocketConnect.SocketIOInit();
#endif
		}

		void Start()
		{
			Utility.Socket.SubscribeEvent(SocketEnum.updateAnonymous.ToString(), this.gameObject.name, nameof(OnUpdateAnonymous), OnUpdateAnonymous);
		}

		public void StartConnectWallet(Action onSuccess)
		{
			Debug.Log("StartConnectWallet");
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
			Debug.Log("Connect ArgentX");
			WalletConnectAsync(JSInteropManager.ConnectWalletArgentX);
			IsAnonymous = false;
		}
		private void ConnectBraavos()
		{
			Debug.Log("Connect Braavos");
			WalletConnectAsync(JSInteropManager.ConnectWalletBraavos);
			IsAnonymous = false;
		}
		private void ConnectAnonymous()
		{
			// Request address and private key from server
			Utility.Socket.EmitEvent(SocketEnum.anonymousLogin.ToString());
			Utility.Socket.EmitEvent(SocketEnum.requestUpdatePlayerTreasury.ToString());
			IsAnonymous = true;

			// Hide UI
			_connectWalletUI.Close();
			_onSuccess?.Invoke();
		}

		private async void OnUpdateAnonymous(string data)
		{
			Debug.Log("Update anonymous: " + data);
			await UniTask.SwitchToMainThread();
			object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
			string[] dataArray = JsonConvert.DeserializeObject<string[]>(useData.ToString());

			// PlayerData.PlayerAddress = dataArray[0];
		}

		private async void WalletConnectAsync(Action walletAction)
		{
			try
			{
				// Debug.Log("WalletConnectAsync: IsConnected: " + JSInteropManager.IsConnected());
				string playerAddress;
				if (Application.isEditor)
				{
					playerAddress =
							"0x005893a02E717eeeE01572066DdD47D70014Cb497345991F8c7D4338F6b29d79";
				}
				else
				{
					if (!JSInteropManager.IsWalletAvailable())
					{
						PopupManager.Instance.ShowAnnouncePopup("Please install wallet", "Do not find starknet wallet. Please install wallet", "OK", null);
						await UniTask.WaitUntil(() => JSInteropManager.IsWalletAvailable());
					}

					walletAction?.Invoke();
					await UniTask.WaitUntil(() => JSInteropManager.IsConnected());
					playerAddress = JSInteropManager.GetAccount();
				}

				playerAddress = Utility.Socket.StringToSocketJson(playerAddress);
				Utility.Socket.EmitEvent(SocketEnum.updatePlayerAddress.ToString(), playerAddress);
				GameEvent.Instance.OnLoadDataSuccess += OnLoadDatasSuccessHandler;
				await DataSaveLoadManager.Instance.LoadData();
			}
			catch (Exception e)
			{
				Debug.LogError("An error occurred in WalletConnectAsync: " + e.Message);
				throw;
			}
		}

		private void OnLoadDatasSuccessHandler()
		{
			GameEvent.Instance.OnLoadDataSuccess -= OnLoadDatasSuccessHandler;
			_connectWalletUI.Close();
			LoadingUIManager.Instance.Hide();
			_onSuccess?.Invoke();
		}
	}
}
