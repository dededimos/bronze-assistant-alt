// Define a global object to contain all cache-related methods
window.blazorCache = {
    db: null, // This will hold a reference to the IndexedDB instance

    // Open the IndexedDB and create object stores (collections) if needed
    openDb: function (storeNames) {
        return new Promise((resolve, reject) => {
            // Open (or create) the database "BlazorAppCache" with version 1
            const request = indexedDB.open("BlazorAppCache", 1);

            // This event fires only when the DB is created for the first time or upgraded
            request.onupgradeneeded = function (event) {
                const db = event.target.result;

                console.log("storeNames received:", storeNames);
                console.log("Type of storeNames:", typeof storeNames);
                // ✅ Fix if it's a single string (not an array)
                if (!Array.isArray(storeNames)) {
                    throw new Error("storeNames must be an array");
                }
                // Loop through the requested store names (like "Accessories", "Mirrors", etc.)
                storeNames.forEach(name => {
                    // If this store doesn't already exist, create it with keyPath "id"
                    if (!db.objectStoreNames.contains(name)) {
                        db.createObjectStore(name, { keyPath: "id" });
                    }
                });
            };

            // This fires when the DB is successfully opened
            request.onsuccess = function (event) {
                // Save the DB reference for future use
                window.blazorCache.db = event.target.result;
                resolve(true); // Notify the caller that the DB is ready
            };

            // If the open request fails, reject the promise
            request.onerror = function () {
                reject("Failed to open DB");
            };
        });
    },
    // Save a list of items into a specific store (like saving to a collection)
    saveItems: function (storeName, items) {
        return new Promise((resolve, reject) => {
            // Start a transaction with read/write access to the given store
            const tx = window.blazorCache.db.transaction(storeName, "readwrite");
            const store = tx.objectStore(storeName); // Get reference to the object store

            // Loop through the items and save (or update) each one
            items.forEach(item => store.put(item)); // `put` = insert or update by key

            // Resolve the promise when the transaction completes
            tx.oncomplete = () => resolve(true);

            // Reject if anything goes wrong
            tx.onerror = () => reject("Failed to save items");
        });
    },
    // Load all items from a specific store (like loading all documents from a collection)
    getAllItems: function (storeName) {
        return new Promise((resolve, reject) => {
            // Start a read-only transaction
            const tx = window.blazorCache.db.transaction(storeName, "readonly");
            const store = tx.objectStore(storeName);

            // Get all items in this store
            const request = store.getAll();

            // On success, resolve with the list of items
            request.onsuccess = () => resolve(request.result);

            // On failure, reject the promise
            request.onerror = () => reject("Failed to load items");
        });
    },
    // Clear all data from a specific store (used for refreshing or wiping the cache)
    clearStore: function (storeName) {
        return new Promise((resolve, reject) => {
            // Start a read/write transaction
            const tx = window.blazorCache.db.transaction(storeName, "readwrite");
            const store = tx.objectStore(storeName);

            // Clear everything in this store
            const request = store.clear();

            // On success, resolve
            request.onsuccess = () => resolve(true);

            // On failure, reject
            request.onerror = () => reject("Failed to clear store");
        });
    }
};