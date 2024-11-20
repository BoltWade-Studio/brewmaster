
mergeInto(LibraryManager.library, {
    // Initialize Firebase
    initializeFirebase: function () {
        const firebaseConfig = {
            apiKey: "AIzaSyAqYU0y8wKsiOhhI7o2LJ6h3pOcD2zQVlo",
            authDomain: "brewmaster-2de54.firebaseapp.com",
            projectId: "brewmaster-2de54",
            storageBucket: "brewmaster-2de54.firebasestorage.app",
            messagingSenderId: "831912550558",
            appId: "1:831912550558:web:5c55e33a00e39a5233db8f",
            measurementId: "G-124GJBFLXF"
        };

        const init = () => {
            if (!firebase.apps.length) {
                firebase.initializeApp(firebaseConfig);
                console.log("Firebase initialized successfully.");
            } else {
                console.log("Firebase is already initialized.");
            }
        }

        if (typeof firebase !== "undefined") {
            init();
        } else {
            console.warn("Firebase not yet available, retrying...");
            setTimeout(init, 100); // Retry every 100ms until Firebase is available
        }

        // Initialize Firebase only if it hasn’t been initialized
    },

    // Example function to set data in Firestore
    setFirestoreData: function (collection, doc, data) {
        const db = firebase.firestore();
        db.collection(collection).doc(doc).set(JSON.parse(data))
            .then(() => {
                console.log("Document successfully written!");
            })
            .catch((error) => {
                console.error("Error writing document: ", error);
            });
    },

    // Example function to get data from Firestore
    getFirestoreData: function (collection, doc, callbackName, functionName) {
        const db = firebase.firestore();
        let collectionName = UTF8ToString(collection);
        let docName = UTF8ToString(doc);
        let callbackObjectName = UTF8ToString(callbackName);
        let callbackMethodName = UTF8ToString(functionName);
        db.collection(collectionName).doc(docName).get()
            .then((doc) => {
                if (doc.exists) {
                    // Pass the data back to Unity
                    SendMessage(callbackObjectName, callbackMethodName, JSON.stringify(doc.data()));
                } else {
                    console.log("No such document!");
                }
            })
            .catch((error) => {
                console.log("Error getting document:", error);
            });
    },

    getDownloadUrl: function (path, callbackName, functionName) {
        const pathUrl = UTF8ToString(path);
        const callbackObjectName = UTF8ToString(callbackName);
        const callbackMethodName = UTF8ToString(functionName);
        const storage = firebase.storage();
        const storageRef = storage.ref(pathUrl);
        storageRef.getDownloadURL()
            .then((url) => {
                SendMessage(callbackObjectName, callbackMethodName, url);
            })
            .catch((error) => {
                console.log("Error getting download URL:", error);
                SendMessage(callbackObjectName, callbackMethodName, "");
            });
    }
});
