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

        var socket = io('http://localhost:5006');
        // var socket = io('https://brewmaster-socket.boltwade.xyz');

        socket.on('connect', () => {
            socket.isReady = true;
            console.log('Socket.IO connected');
        });

        socket.on('updateBrushPosition', () => {
            if(this.objectNameDic.updateBrushPosition && this.methodNameDic.updateBrushPosition)
                SendMessage(this.objectNameDic.updateBrushPosition, this.methodNameDic.updateBrushPosition);
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
            // console.log(data);
            // let dataString = UTF8ToString(data);
            // let array = JSON.parse(dataString.toString());
			const encodedText = encodeURIComponent(data);
			const tweetUrl = `https://twitter.com/intent/tweet?text=${encodedText}`;

            console.log('tweetUrl: ' + tweetUrl);
			// Open the URL in a new window/tab
			window.open(tweetUrl, '_blank');
		});

        socket.on('exception', (exception) => {
            for(let i = 0; i < this.exceptionArray.length; i++)
            {
                SendMessage(this.exceptionArray[i].objectName, this.exceptionArray[i].methodName, exception.message);
            }
        });

        socket.on('giftTxHash', (data) => {
            SendMessage(this.objectNameDic.giftTxHash, this.methodNameDic.giftTxHash, data.toString());
        });
        
        socket.on('playerMove', (data) => {
        	if(this.objectNameDic.playerMove && this.methodNameDic.playerMove)
        		SendMessage(this.objectNameDic.playerMove, this.methodNameDic.playerMove, data.toString());
		});

		socket.on('spawnCustomer', (data) => {
			if(this.objectNameDic.spawnCustomer && this.methodNameDic.spawnCustomer)
				SendMessage(this.objectNameDic.spawnCustomer, this.methodNameDic.spawnCustomer, data.toString());
		});

		socket.on('updateCustomerPosition', (data) => {
			if(this.objectNameDic.updateCustomerPosition && this.methodNameDic.updateCustomerPosition)
				SendMessage(this.objectNameDic.updateCustomerPosition, this.methodNameDic.updateCustomerPosition, data.toString());
		});

		socket.on('updateCustomerWaitTime', (data) => {
			if(this.objectNameDic.updateCustomerWaitTime && this.methodNameDic.updateCustomerWaitTime)
				SendMessage(this.objectNameDic.updateCustomerWaitTime, this.methodNameDic.updateCustomerWaitTime, data.toString());
		});

		socket.on('customerReachDestination', (data) => {
			if(this.objectNameDic.customerReachDestination && this.methodNameDic.customerReachDestination)
				SendMessage(this.objectNameDic.customerReachDestination, this.methodNameDic.customerReachDestination, data.toString());
		});

		socket.on('customerReturn', (data) => {
			if(this.objectNameDic.customerReturn && this.methodNameDic.customerReturn)
				SendMessage(this.objectNameDic.customerReturn, this.methodNameDic.customerReturn, data.toString());
		});

		socket.on('deleteCustomer', (data) => {
			if(this.objectNameDic.deleteCustomer && this.methodNameDic.deleteCustomer)
				SendMessage(this.objectNameDic.deleteCustomer, this.methodNameDic.deleteCustomer, data.toString());
		});

		socket.on('serveBeer', (data) => {
			if(this.objectNameDic.serveBeer && this.methodNameDic.serveBeer)
				SendMessage(this.objectNameDic.serveBeer, this.methodNameDic.serveBeer, data.toString());
		});

		socket.on('updateBeer', (data) => {
			if(this.objectNameDic.updateBeer && this.methodNameDic.updateBeer)
				SendMessage(this.objectNameDic.updateBeer, this.methodNameDic.updateBeer, data.toString());
		});

		socket.on('beerCollided', (data) => {
			if(this.objectNameDic.beerCollided && this.methodNameDic.beerCollided)
				SendMessage(this.objectNameDic.beerCollided, this.methodNameDic.beerCollided, data.toString());
		});

		socket.on('updateTimer', (data) => {
			if(this.objectNameDic.updateTimer && this.methodNameDic.updateTimer)
				SendMessage(this.objectNameDic.updateTimer, this.methodNameDic.updateTimer, data.toString());
		});

		socket.on('timeUp', (data) => {
			if(this.objectNameDic.timeUp && this.methodNameDic.timeUp)
				SendMessage(this.objectNameDic.timeUp, this.methodNameDic.timeUp, data.toString());
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
