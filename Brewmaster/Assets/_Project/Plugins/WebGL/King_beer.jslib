mergeInto(LibraryManager.library, {
    SocketIOInit: function () {
        this.socketEventClassDic = {};
        this.exceptionArray = [];

		if (typeof io == "undefined") {
			console.error("Socket.IO client library is not load");
			return;
		}

        // var socket = io("http://localhost:5006");
        // var socket = io("https://brewmaster-socket-test.boltwade.xyz");
        var socket = io("https://brewmaster-socket.boltwade.xyz");

		socket.on("connect", () => {
			socket.isReady = true;
			console.log("Socket.IO connected");
		});

        socket.onAny((eventName, ...args) => {
            let data;

			// Check if args[0] is a string
			if (args.length > 0) {
				if (typeof args[0] === "string") {
					data = args[0];
					console.log("dataString: " + data);
				} else {
					// Old code need to refactor (don't send object directly)
					data = JSON.stringify(args);
					console.log("dataObject: " + data);
				}
			} else {
				data = "";
			}

			if (eventName === 'exception') {
				this.exceptionArray.forEach(([objectName, methodName]) => {
					SendMessage(objectName, methodName, data);
				});
			} else {
				if (this.socketEventClassDic[eventName]) {
					this.socketEventClassDic[eventName].forEach(
						([objectName, methodName]) => {
							SendMessage(objectName, methodName, data);
						}
					);
				}
			}
		});

		socket.on("twitterRequestCallback", (data) => {
            const encodedText = encodeURIComponent(data);
            const tweetUrl = `https://twitter.com/intent/tweet?text=${encodedText}`;

            console.log("tweetUrl: " + tweetUrl);
            // Open the URL in a new window/tab
            window.open(tweetUrl, "_blank");
        });


		window.unitySocket = socket;
	},

	SubscribeEvent: function (
		eventName,
		callbackObjectName,
		callbackMethodName
	) {
		const event = UTF8ToString(eventName);
		const objectName = UTF8ToString(callbackObjectName);
		const methodName = UTF8ToString(callbackMethodName);

		if (!this.socketEventClassDic[event]) {
			this.socketEventClassDic[event] = [];
		}

		const existingCallback = this.socketEventClassDic[event].find(
			(callback) =>
				callback[0] === objectName && callback[1] === methodName
		);

		if (!existingCallback) {
			this.socketEventClassDic[event].push([objectName, methodName]);
		}
	},

	UnSubscribeEvent: function (
		eventName,
		callbackObjectName,
		callbackMethodName
	) {
		const event = UTF8ToString(eventName);
		const objectName = UTF8ToString(callbackObjectName);
		const methodName = UTF8ToString(callbackMethodName);

		if (this.socketEventClassDic[event]) {
			this.socketEventClassDic[event] = this.socketEventClassDic[
				event
			].filter(
				(callback) =>
					callback[0] !== objectName || callback[1] !== methodName
			);
		}
	},

	EmitEvent: function (eventName, jsonArray) {
		let data = UTF8ToString(jsonArray);
		let event = UTF8ToString(eventName);

		if (window.unitySocket && data) {
			window.unitySocket.emit(event, data);
			return;
		}
		if (window.unitySocket) window.unitySocket.emit(event);
	},

	SubscribeOnException: function (callbackObjectName, callbackMethodName) {
		let objectName = UTF8ToString(callbackObjectName);
		let methodName = UTF8ToString(callbackMethodName);

		this.exceptionArray.push([objectName, methodName]);
	},
	UnSubscribeOnException: function (callbackObjectName, callbackMethodName) {
		let objectName = UTF8ToString(callbackObjectName);
		let methodName = UTF8ToString(callbackMethodName);

		this.exceptionArray = this.exceptionArray.filter((e) => e[0] !== objectName || e[1] !== methodName);
	},

	FreeWasmString: function (ptr) {
		_free(ptr);
	},
});
