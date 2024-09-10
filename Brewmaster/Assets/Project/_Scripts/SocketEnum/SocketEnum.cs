public enum SocketEnum
{
	initGame,
	anonymousLogin,

	#region Update enum
	updateProof,
	updateAnonymous,
	updatePlayerAddress,
	updateCustomer,
	updateCustomerPosition,
	updateCustomerWaitTime,
	updateTablePositions,
	updateSeatPositions,
	requestUpdatePlayerTreasury,
	#endregion

	#region Callback enum
	loadCallback,
	updateTreasuryCallback,
	sendContractCallback,
	getEntryCallback,
	getPlayerPubCallback,
	#endregion

	#region Claim
	claim,
	afterClaim,
	#endregion

	shareToTwitterRequest,
	playerInputLink,

	saveDataRequest,
	loadDataRequest,
	playerMove,
	spawnCustomer,
	customerReachDestination,
	customerReturn,
	deleteCustomer,
	serveBeer,
	updateBeer,
	beerCollided,
	getEntry,
	getPlayerPub,
}

