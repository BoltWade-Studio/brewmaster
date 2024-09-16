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
	updateTablePositionCallback,
	updateSeatPositionsCallback,
	requestUpdatePlayerTreasury,
	updateUpgradePrice,
	updateIsMainMenu,
	#endregion

	#region Callback enum
	updateUpgradePriceCallback,
	loadCallback,
	updateTreasuryCallback,
	sendContractCallback,
	getEntryCallback,
	getPlayerPubCallback,
	#endregion

	#region Claim
	claim,
	claimCallback,
	#endregion

	#region Upgrade 
	getPriceForAddStool,
	getPriceForAddStoolCallback,
	upgradeTable,
	upgradeTableCallback,
	getCanUpgradeTable,
	getCanUpgradeTableCallback,
	#endregion

	#region Get temp point
	getPointBeforeClaim,
	getPointBeforeClaimCallback,
	#endregion

	shareToTwitterRequest, // do not delete, used but not see reference
	playerInputLink,

	#region Get twitter message
	getTwitterMessage,
	getTwitterMessageCallback,
	#endregion

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

