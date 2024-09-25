mergeInto(LibraryManager.library, {

    SocketIOInit:function()
    {
        this.objectNameDic = {};
        this.methodNameDic = {};
        this.exceptionArray = [];

        if(typeof io == 'undefined')
        {
            console.error('Socket.IO client library is not load');
            return;
        }

        // var socket = io('http://localhost:5006');
        var socket = io('https://brewmaster-socket.boltwade.xyz');

        socket.on('connect', () => {
            socket.isReady = true;
            console.log('Socket.IO connected');
        });

        socket.on('updateProof', (proof) => {
            if(this.objectNameDic.updateProof && this.methodNameDic.updateProof)
                SendMessage(this.objectNameDic.updateProof , this.methodNameDic.updateProof, proof.toString());
        });

        socket.on('updateAnonymous', (data) => {
            if(this.objectNameDic.updateAnonymous && this.methodNameDic.updateAnonymous)
                SendMessage(this.objectNameDic.updateAnonymous, this.methodNameDic.updateAnonymous, data.toString());
        });

        socket.on('updateSaveData', (data)=> {
            if(this.objectNameDic.updateSaveData && this.methodNameDic.updateSaveData)
                SendMessage(this.objectNameDic.updateSaveData, this.methodNameDic.updateSaveData, data.toString());
        });

		socket.on('twitterRequestCallback', (data) => {
			const encodedText = encodeURIComponent(data);
			const tweetUrl = `https://twitter.com/intent/tweet?text=${encodedText}`;

            console.log('tweetUrl: ' + tweetUrl);
			// Open the URL in a new window/tab
			window.open(tweetUrl, '_blank');
		});

        socket.on('exception', (exception) => {
        	console.log('Exception: ' + exception.toString());
            for(let i = 0; i < this.exceptionArray.length; i++)
            {
                SendMessage(this.exceptionArray[i].objectName, this.exceptionArray[i].methodName, exception.toString());
            }
        });

        socket.on('giftData', (data) => {
            SendMessage(this.objectNameDic.giftData, this.methodNameDic.giftData, data.toString());
        });

        socket.on('playerMove', (data) => {
        	let dataString = JSON.stringify(data).toString();
        	console.log(dataString);
        	if(this.objectNameDic.playerMove && this.methodNameDic.playerMove)
        		SendMessage(this.objectNameDic.playerMove, this.methodNameDic.playerMove, dataString);
		});

		socket.on('spawnCustomer', (data) => {
			console.log(data);
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.spawnCustomer && this.methodNameDic.spawnCustomer)
				SendMessage(this.objectNameDic.spawnCustomer, this.methodNameDic.spawnCustomer, dataString);
		});

		socket.on('updateCustomerPosition', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.updateCustomerPosition && this.methodNameDic.updateCustomerPosition)
				SendMessage(this.objectNameDic.updateCustomerPosition, this.methodNameDic.updateCustomerPosition, dataString);
		});

		socket.on('updateCustomerWaitTime', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.updateCustomerWaitTime && this.methodNameDic.updateCustomerWaitTime)
				SendMessage(this.objectNameDic.updateCustomerWaitTime, this.methodNameDic.updateCustomerWaitTime, dataString);
		});

		socket.on('customerReachDestination', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.customerReachDestination && this.methodNameDic.customerReachDestination)
				SendMessage(this.objectNameDic.customerReachDestination, this.methodNameDic.customerReachDestination, dataString);
		});

		socket.on('customerReturn', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.customerReturn && this.methodNameDic.customerReturn)
				SendMessage(this.objectNameDic.customerReturn, this.methodNameDic.customerReturn, dataString);
		});

		socket.on('deleteCustomer', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.deleteCustomer && this.methodNameDic.deleteCustomer)
				SendMessage(this.objectNameDic.deleteCustomer, this.methodNameDic.deleteCustomer, dataString);
		});

		socket.on('serveBeer', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.serveBeer && this.methodNameDic.serveBeer)
				SendMessage(this.objectNameDic.serveBeer, this.methodNameDic.serveBeer, dataString);
		});

		socket.on('updateBeer', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.updateBeer && this.methodNameDic.updateBeer)
				SendMessage(this.objectNameDic.updateBeer, this.methodNameDic.updateBeer, dataString);
		});

		socket.on('beerCollided', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.beerCollided && this.methodNameDic.beerCollided)
				SendMessage(this.objectNameDic.beerCollided, this.methodNameDic.beerCollided, dataString);
		});

		socket.on('updateTimer', (data) => {
			if(this.objectNameDic.updateTimer && this.methodNameDic.updateTimer)
				SendMessage(this.objectNameDic.updateTimer, this.methodNameDic.updateTimer, data.toString());
		});

		socket.on('totalPointCallback', (data) => {
			if(this.objectNameDic.totalPointCallback && this.methodNameDic.totalPointCallback)
				SendMessage(this.objectNameDic.totalPointCallback, this.methodNameDic.totalPointCallback, data.toString());
		});

		socket.on('timeUp', (data) => {
			if(this.objectNameDic.timeUp && this.methodNameDic.timeUp)
				SendMessage(this.objectNameDic.timeUp, this.methodNameDic.timeUp, '');
		});

		socket.on('upgradeTableCallback', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.upgradeTableCallback && this.methodNameDic.upgradeTableCallback)
				SendMessage(this.objectNameDic.upgradeTableCallback, this.methodNameDic.upgradeTableCallback, dataString);
		});

		socket.on('updateUpgradePrice', (data) => {
			let dataString = JSON.stringify(data).toString();
			if(this.objectNameDic.updateUpgradePrice && this.methodNameDic.updateUpgradePrice)
				SendMessage(this.objectNameDic.updateUpgradePrice, this.methodNameDic.updateUpgradePrice, dataString);
		});

        socket.on('getEntryCallback', (data) => {
            // use this data to call actual transaction
			if(this.objectNameDic.getEntryCallback && this.methodNameDic.getEntryCallback)
                SendMessage(this.objectNameDic.getEntryCallback, this.methodNameDic.getEntryCallback, data)
        });

        socket.on('getPlayerPubCallback', (data) => {
			if(this.objectNameDic.getPlayerPubCallback && this.methodNameDic.getPlayerPubCallback)
            {
                SendMessage(this.objectNameDic.getPlayerPubCallback, this.methodNameDic.getPlayerPubCallback, data)
            }
        });
        socket.on('updateTreasuryCallback', (data) =>
        {
			if(this.objectNameDic.updateTreasuryCallback && this.methodNameDic.updateTreasuryCallback)
            {
                SendMessage(this.objectNameDic.updateTreasuryCallback, this.methodNameDic.updateTreasuryCallback, data)
            }
        });

        socket.on('updateInDayTreasuryCallback', (data) =>
        {
			if(this.objectNameDic.updateInDayTreasuryCallback && this.methodNameDic.updateInDayTreasuryCallback)
			{
				SendMessage(this.objectNameDic.updateInDayTreasuryCallback, this.methodNameDic.updateInDayTreasuryCallback, data)
			}
		});

        socket.on('claimCallback', (data) =>
        {
			if(this.objectNameDic.claimCallback && this.methodNameDic.claimCallback)
            {
                SendMessage(this.objectNameDic.claimCallback, this.methodNameDic.claimCallback, data)
            }
        });
        socket.on('getPointBeforeClaimCallback', (data) =>
        {
			if(this.objectNameDic.getPointBeforeClaimCallback && this.methodNameDic.getPointBeforeClaimCallback)
            {
                SendMessage(this.objectNameDic.getPointBeforeClaimCallback, this.methodNameDic.getPointBeforeClaimCallback, data)
            }
        });
        socket.on('getCanUpgradeTableCallback', (data) =>
        {
			if(this.objectNameDic.getCanUpgradeTableCallback && this.methodNameDic.getCanUpgradeTableCallback)
            {
                SendMessage(this.objectNameDic.getCanUpgradeTableCallback, this.methodNameDic.getCanUpgradeTableCallback, data)
            }
        });
        socket.on('updateTablePositionCallback', (data) =>
        {
			if(this.objectNameDic.updateTablePositionCallback && this.methodNameDic.updateTablePositionCallback)
            {
                SendMessage(this.objectNameDic.updateTablePositionCallback, this.methodNameDic.updateTablePositionCallback, '')
            }
        });
        socket.on('updateSeatPositionsCallback', (data) =>
        {
			if(this.objectNameDic.updateSeatPositionsCallback && this.methodNameDic.updateSeatPositionsCallback)
            {
                SendMessage(this.objectNameDic.updateSeatPositionsCallback, this.methodNameDic.updateSeatPositionsCallback, '')
            }
        });
        socket.on('logoutCallback', (data) =>
        {
			if(this.objectNameDic.logoutCallback && this.methodNameDic.logoutCallback)
            {
                SendMessage(this.objectNameDic.logoutCallback, this.methodNameDic.logoutCallback, '')
            }
        });

        window.unitySocket = socket;
    },

    OnEvent:function(eventName, callbackObjectName, callbackMethodName)
    {
        let event = UTF8ToString(eventName);
        this.objectNameDic[event] = UTF8ToString(callbackObjectName);
        this.methodNameDic[event] = UTF8ToString(callbackMethodName);
    },

    EmitEvent:function(eventName, jsonArray)
    {
        let data = UTF8ToString(jsonArray);
        let event = UTF8ToString(eventName);

        if(window.unitySocket && data)
        {
            window.unitySocket.emit(event, data);
            return;
        }
        if(window.unitySocket)
            window.unitySocket.emit(event);
    },

    SubscribeOnException:function(callbackObjectName, callbackMethodName)
    {
        let object = UTF8ToString(callbackObjectName);
        let method = UTF8ToString(callbackMethodName);
        const objectMethod = new Object();
        objectMethod.objectName = object;
        objectMethod.methodName = method;

        this.exceptionArray.push(objectMethod);
    },
    UnSubscribeOnException:function(callbackObjectName, callbackMethodName)
    {
        let object = UTF8ToString(callbackObjectName);
        let method = UTF8ToString(callbackMethodName);
        _.remove(this.exceptionArray, function(e) {
            return e.objectName == object && e.methodName == method
        });
    },

    FreeWasmString: function (ptr) {
        _free(ptr);
    },
});
