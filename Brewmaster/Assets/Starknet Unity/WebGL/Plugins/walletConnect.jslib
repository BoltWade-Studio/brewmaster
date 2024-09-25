mergeInto(LibraryManager.library,{

	IsWalletAvailable: function()
	{
		if (window.starknet)
		{
			return true;
		}
		else
		{
			return false;
		}
	},

	AskToInstallWallet: function()
	{
		window.alert("Please install Starknet Wallet");
	},

	ConnectWalletArgentX: async function()
	{
		if (window.starknet_argentX)
		{
			try
			{
				await window.starknet_argentX.enable();
				if(window.starknet.isConnected) 
				{
					window.walletConnect = true;
				}
			}
			catch(e)
			{
				console.log(e.message);
			}
		}
	},

	ConnectWalletBraavos: async function()
	{
		if (window.starknet_braavos)
		{
			try
			{
				await window.starknet_braavos.enable();
				if(window.starknet.isConnected) 
				{
					window.walletConnect = true;
				}
			}
			catch(e)
			{
				console.log("error braavos: ");
				console.log(e.message);
			}
		}
		else
		{
			console.log("don't have braavos");
		}
	},

	DisconnectWallet: async function()
	{
		window.walletConnect = false;
	},

	IsConnected: function()
	{
		return window.starknet && window.starknet.isConnected && window.walletConnect == true;
	},

	GetAccount: function()
	{
		const address = window.starknet.account.address;
		var bufferSize = lengthBytesUTF8(address) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(address, buffer, bufferSize);
		return buffer;
	},

	SendTransactionArgentX: async function(contractAddress, entrypoint, calldata, callbackObjectName, callbackMethodName)
	{
		const jsStringToWasm = (str) => {
			var bufferSize = lengthBytesUTF8(str) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(str, buffer, bufferSize);
			return buffer;
		}

		const toHex = (number) => {
			return "0x" + number.toString(16);
		}
		
		const calldataArray = JSON.parse(UTF8ToString(calldata))
		const contractAddressStr = UTF8ToString(contractAddress)
		const entrypointStr = UTF8ToString(entrypoint)
		const callbackObjectStr = UTF8ToString(callbackObjectName)
		const callbackMethodStr = UTF8ToString(callbackMethodName)
		const maxQtyGasAuthorized = 1800n; 
		const maxPriceAuthorizeForOneGas = 12n * 10n ** 9n;

		await window.starknet_argentX.enable();
		if (window.starknet_argentX.selectedAddress)
		{
			window.starknet_argentX.account.execute(
			[
				{
					contractAddress: contractAddressStr,
					entrypoint: entrypointStr,
					calldata: calldataArray.array
				}
			],
			// {
			// 	version: 3,
			// 	maxFee: 10 ** 15,
			// 	feeDataAvailabilityMode: "L1",
			// 	tip: 10 ** 13,
			// 	paymasterData: [],
			// 	// resourceBounds: {
			// 	// 	l1_gas: {
			// 	// 		max_amount: toHex(maxQtyGasAuthorized),
			// 	// 		max_price_per_unit: toHex(maxPriceAuthorizeForOneGas),
			// 	// 	},
			// 	// 	l2_gas: {
			// 	// 		max_amount: toHex(0),
			// 	// 		max_price_per_unit: toHex(0),
			// 	// 	},
			// 	// },
			// },
			).then((response) => {
				const transactionHash = response.transaction_hash;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, transactionHash);
			}).catch((error) => {
				const errorMessage = error.message;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, errorMessage);
			})	
		}
	},

	SendTransactionBraavos: async function(contractAddress, entrypoint, calldata, callbackObjectName, callbackMethodName)
	{
		const jsStringToWasm = (str) => {
			var bufferSize = lengthBytesUTF8(str) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(str, buffer, bufferSize);
			return buffer;
		}
		
		const calldataArray = JSON.parse(UTF8ToString(calldata))
		const contractAddressStr = UTF8ToString(contractAddress)
		const entrypointStr = UTF8ToString(entrypoint)
		const callbackObjectStr = UTF8ToString(callbackObjectName)
		const callbackMethodStr = UTF8ToString(callbackMethodName)

		await window.starknet_braavos.enable();
		if (window.starknet_braavos.selectedAddress)
		{
			window.starknet_braavos.account.execute([{
				contractAddress: contractAddressStr,
				entrypoint: entrypointStr,
				calldata: calldataArray.array
			}]).then((response) => {
				const transactionHash = response.transaction_hash;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, transactionHash);
			}).catch((error) => {
				const errorMessage = error.message;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, errorMessage);
			})	
		}
	},

	SendTransaction: async function(contractAddress, entrypoint, calldata, callbackObjectName, callbackMethodName)
	{
		const jsStringToWasm = (str) => {
			var bufferSize = lengthBytesUTF8(str) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(str, buffer, bufferSize);
			return buffer;
		}
		
		const calldataArray = JSON.parse(UTF8ToString(calldata))
		const contractAddressStr = UTF8ToString(contractAddress)
		const entrypointStr = UTF8ToString(entrypoint)
		const callbackObjectStr = UTF8ToString(callbackObjectName)
		const callbackMethodStr = UTF8ToString(callbackMethodName)

		await window.starknet.enable();
		if (window.starknet.selectedAddress)
		{
			window.starknet.account.execute([{
				contractAddress: contractAddressStr,
				entrypoint: entrypointStr,
				calldata: calldataArray.array
			}]).then((response) => {
				const transactionHash = response.transaction_hash;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, transactionHash);
			}).catch((error) => {
				const errorMessage = error.message;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, errorMessage);
			})
		}
	},

	SendTransactionNoData: async function(contractAddress, entrypoint, callbackObjectName, callbackMethodName)
	{
		const jsStringToWasm = (str) => {
			var bufferSize = lengthBytesUTF8(str) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(str, buffer, bufferSize);
			return buffer;
		}
		
		const contractAddressStr = UTF8ToString(contractAddress);
		const entrypointStr = UTF8ToString(entrypoint);
		const callbackObjectStr = UTF8ToString(callbackObjectName);
		const callbackMethodStr = UTF8ToString(callbackMethodName);
		console.log('socketEntryPoint: ' + entrypointStr);

		await window.starknet.enable();
		if (window.starknet.selectedAddress)
		{
			window.starknet.account.execute([{
				contractAddress: contractAddressStr,
				entrypoint: entrypointStr
			}]).then((response) => {
				// const transactionHash = response.transaction_hash;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr);
			}).catch((error) => {
				// const errorMessage = error.message;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr);
			})
		}
	},

	CallContract: async function(contractAddress, entrypoint, calldata, callbackObjectName, callbackMethodName)
	{
		const jsStringToWasm = (str) => {
			var bufferSize = lengthBytesUTF8(str) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(str, buffer, bufferSize);
			return buffer;
		}

		const calldataArray = JSON.parse(UTF8ToString(calldata))
		const contractAddressStr = UTF8ToString(contractAddress)
		const entrypointStr = UTF8ToString(entrypoint)
		const callbackObjectStr = UTF8ToString(callbackObjectName)
		const callbackMethodStr = UTF8ToString(callbackMethodName)
		
		await window.starknet.enable();
		if (window.starknet.selectedAddress)
		{
			window.starknet.account.callContract({
				contractAddress: contractAddressStr,
				entrypoint: entrypointStr,
				calldata: calldataArray.array
			}).then((response) => {
				const responseStr = JSON.stringify(response);
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, responseStr);
			}).catch((error) => {
				const errorMessage = error.message;
				myGameInstance.SendMessage(callbackObjectStr, callbackMethodStr, errorMessage);
			})
		}
	},

	FreeWasmString: function(ptr) {
    _free(ptr);
}
});