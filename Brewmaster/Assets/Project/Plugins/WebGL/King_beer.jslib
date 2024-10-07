mergeInto(LibraryManager.library, {
    SocketIOInit: function () {
        this.socketEventClassDic = {};
        this.exceptionArray = [];

		if (typeof io == "undefined") {
			console.error("Socket.IO client library is not load");
			return;
		}

        var socket = io("http://localhost:5006");
        // var socket = io("https://brewmaster-socket-test.boltwade.xyz");
        // var socket = io("https://brewmaster-socket.boltwade.xyz");

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

			if (eventName === "exception") {
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
		let object = UTF8ToString(callbackObjectName);
		let method = UTF8ToString(callbackMethodName);
		const objectMethod = new Object();
		objectMethod.objectName = object;
		objectMethod.methodName = method;

		this.exceptionArray.push(objectMethod);
	},
	UnSubscribeOnException: function (callbackObjectName, callbackMethodName) {
		let object = UTF8ToString(callbackObjectName);
		let method = UTF8ToString(callbackMethodName);
		_.remove(this.exceptionArray, function (e) {
			return e.objectName == object && e.methodName == method;
		});
	},

	FreeWasmString: function (ptr) {
		_free(ptr);
	},
});
