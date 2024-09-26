public enum SocketEnum
{
	initGame,
	anonymousLogin,
	logout,
	logoutCallback,

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
	updateInDayTreasuryCallback,
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
	giftData,
	#endregion

	#region Transaction
	waitTransaction,
	waitTransactionCallback,
	#endregion

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

