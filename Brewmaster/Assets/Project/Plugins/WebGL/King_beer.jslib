mergeInto(LibraryManager.library, {

    SocketIOInit:function()
    {
        this.objectNameDic = {};
        this.methodNameDic = {};
        if(typeof io == 'undefined')
        {
            console.error('Socket.IO client library is not load');
            return;
        }

        // var socket = io('http://localhost:5006');
        var socket = io('https://brewmaster-socket.starkarcade.com/');

        socket.on('connect', () => {
            socket.isReady = true;
            console.log('Socket.IO connected');
        });

        socket.on('updateBrushPosition', () => {
            if(this.objectNameDic.updateBrushPosition && this.methodNameDic.updateBrushPosition)
                SendMessage(this.objectNameDic.updateBrushPosition, this.methodNameDic.updateBrushPosition);
        });

        socket.on('updateCoin', (coin) => {
            if(this.objectNameDic.updateCoin && this.methodNameDic.updateCoin)
                SendMessage(this.objectNameDic.updateCoin.toString(), this.methodNameDic.updateCoin.toString(), coin.toString());

        });

        socket.on('spawnCoin', () => {
            if(this.objectNameDic.spawnCoin && this.methodNameDic.spawnCoin)
            {
                SendMessage(this.objectNameDic.spawnCoin, this.methodNameDic.spawnCoin, 'SpawnCoin');
            }
        });

        socket.on('updateProof', (proof) => {
            if(this.objectNameDic.updateProof && this.methodNameDic.updateProof)
            {
                SendMessage(this.objectNameDic.updateProof , this.methodNameDic.updateProof, proof.toString());
            }
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

			// Open the URL in a new window/tab
			window.open(tweetUrl, '_blank');
		});

        window.unitySocket = socket;
    },

    OnEvent:function(eventName, callbackObjectName, callbackMethodName)
    {
        let event = UTF8ToString(eventName);
        this.objectNameDic[event] = UTF8ToString(callbackObjectName);
        this.methodNameDic[event] = UTF8ToString(callbackMethodName);
    },

    EmitEvent:function(eventName, dataArray)
    {
        let callDataArray;
        if(dataArray)
            calldataArray = JSON.parse(UTF8ToString(dataArray));
        let event = UTF8ToString(eventName);

        if(window.unitySocket && dataArray)
        {
            window.unitySocket.emit(event, calldataArray.array);
            return;
        }
        if(window.unitySocket)
            window.unitySocket.emit(event);
    },

	OpenTwitter:function(text)
	{
		const encodedText = encodeURIComponent(text);
		const tweetUrl = `https://twitter.com/intent/tweet?text=${encodedText}`;

		// Open the URL in a new window/tab
		window.open(tweetUrl, '_blank');
	},

    FreeWasmString: function (ptr) {
        _free(ptr);
    },
});
