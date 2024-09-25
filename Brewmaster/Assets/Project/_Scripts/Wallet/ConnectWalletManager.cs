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
#if UNITY_EDITOR
			Debug.Log("Socket init");
			Utility.Socket.Init();
#else
			JsSocketConnect.SocketIOInit();
#endif
		}

		void Start()
		{
			Utility.Socket.OnEvent(SocketEnum.updateAnonymous.ToString(), this.gameObject.name, nameof(OnUpdateAnonymous), OnUpdateAnonymous);
		}

		void OnDestroy()
		{
			Utility.Socket.Disconnect();
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

		private void OnUpdateAnonymous(string data)
		{
			Debug.Log("Update anonymous: " + data);
			string[] dataArray = JsonConvert.DeserializeObject<string[]>(data);

			// PlayerData.PlayerAddress = dataArray[0];
		}

		private async void WalletConnectAsync(Action walletAction)
		{
			// Debug.Log("WalletConnectAsync: IsConnected: " + JSInteropManager.IsConnected());
			string playerAddress;
			if (Application.isEditor)
			{
				playerAddress = Utility.Socket.StringToSocketJson("0x06805c9d975f3997b10f4fd2301b906cD8BD0bA6e9E0961C431a5f24B8D35726");
			}
			else
			{
				if (!JSInteropManager.IsWalletAvailable())
				{
					LoadingUIManager.Instance.Show("Please install wallet");
					JSInteropManager.AskToInstallWallet();
					await UniTask.WaitUntil(() => JSInteropManager.IsWalletAvailable());
				}
				LoadingUIManager.Instance.Show("Getting player data");
				walletAction?.Invoke();
				await UniTask.WaitUntil(() => JSInteropManager.IsConnected());
				playerAddress = Utility.Socket.StringToSocketJson(JSInteropManager.GetAccount());
			}

			Utility.Socket.EmitEvent(SocketEnum.updatePlayerAddress.ToString(), playerAddress);
			GameEvent.Instance.OnLoadDataSuccess += OnLoadDatasSuccessHandler;
			Debug.Log("Subscribe");
			await DataSaveLoadManager.Instance.LoadData();
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
