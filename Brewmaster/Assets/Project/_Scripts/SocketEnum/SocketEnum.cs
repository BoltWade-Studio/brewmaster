public enum SocketEnum
{
	initGame,
	anonymousLogin,
	logout,
	logoutCallback,

	#region Update enum
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
	getPriceForAddTable,
	getPriceForAddTableCallback,
	getCanUpgradeTable,
	getCanUpgradeTableCallback,
	getCanAddTable,
	getCanAddTableCallback,
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

	#region Og pass
	getOgPassBalance,
	getOgPassBalanceCallback,
	#endregion

	spawnCustomer,
	spawnCustomerCallback,
	customerReachDestination,
	customerReturn,
	deleteCustomer,
	serveBeer,
	updateBeer,
	beerCollided,
	getEntry,
	getPlayerPub,
}
