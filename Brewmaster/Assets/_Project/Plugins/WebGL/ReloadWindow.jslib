mergeInto(LibraryManager.library, {
    ReloadWindow: function () {
        // delete all caches
        caches.keys().then((cacheNames) => {
            cacheNames.forEach((cacheName) => {
                caches.delete(cacheName).then((success) => {
                    // if (success) {
                    //     console.log(`Cache "${cacheName}" deleted.`);
                    // }
                });
            });
        });
        // delete all sessionStorage
        sessionStorage.clear();
        window.location.reload();
    },
});
