class KeyValueStore {
    constructor(dbName = 'genericStore', version = 1) {
        this.dbName = dbName;
        this.version = version;
        this.db = null;
    }

    async init() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.dbName, this.version);
            
            request.onerror = (event) => {
                reject(event.target.error);
            };
            
            request.onupgradeneeded = (event) => {
                this.db = event.target.result;
                this.createStore();
            };
            
            request.onsuccess = (event) => {
                this.db = event.target.result;
                resolve(this.db);
            };
        });
    }

    createStore() {
        if (this.db.objectStoreNames.contains('values')) {
            this.db.deleteObjectStore('values');
        }
        this.db.createObjectStore('values', { 
            keyPath: 'key',
            autoIncrement: false 
        });
    }

    async set(key, value) {
        if (!this.db) {
            await this.init();
        }
        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['values'], 'readwrite');
            const objectStore = transaction.objectStore('values');
            const request = objectStore.put({ key, value });
            
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    }

    async get(key) {
        if (!this.db) {
            await this.init();
        }
        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['values'], 'readonly');
            const objectStore = transaction.objectStore('values');
            const request = objectStore.get(key);
            
            request.onsuccess = () => resolve(request.result?.value);
            request.onerror = () => reject(request.error);
        });
    }

    async getAll() {
        if (!this.db) {
            await this.init();
        }
        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['values'], 'readonly');
            const objectStore = transaction.objectStore('values');
            const request = objectStore.getAll();
            
            request.onsuccess = () => {
                const results = request.result.map(item => ({
                    key: item.key,
                    value: item.value
                }));
                resolve(results);
            };
            request.onerror = () => reject(request.error);
        });
    }

    async clear() {
        if (!this.db) {
            await this.init();
        }
        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['values'], 'readwrite');
            const objectStore = transaction.objectStore('values');
            const request = objectStore.clear();
            
            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    }
}