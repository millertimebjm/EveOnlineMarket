class SearchStore {
    constructor(dbName = 'searchHistory', version = 1) {
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
        if (this.db.objectStoreNames.contains('searchResults')) {
            this.db.deleteObjectStore('searchResults');
        }

        const store = this.db.createObjectStore('searchResults', {
            keyPath: 'keyword',
            autoIncrement: false
        });

        store.createIndex('nameIndex', 'results.name', { unique: false, multiEntry: true });
    }

    async addSearch(keyword, results) {
        if (!this.db) {
            await this.init();
        }

        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['searchResults'], 'readwrite');
            const objectStore = transaction.objectStore('searchResults');

            const request = objectStore.put({
                keyword: keyword.toLowerCase(),
                results: results
            });

            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    }

    async getSearch(keyword) {
        if (!this.db) {
            await this.init();
        }

        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['searchResults'], 'readonly');
            const objectStore = transaction.objectStore('searchResults');

            const request = objectStore.get(keyword.toLowerCase());
            request.onsuccess = () => resolve(request.result);
            request.onerror = () => reject(request.error);
        });
    }

    async getAllSearches() {
        if (!this.db) {
            await this.init();
        }

        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['searchResults'], 'readonly');
            const objectStore = transaction.objectStore('searchResults');
            const request = objectStore.getAll();

            request.onsuccess = () => resolve(request.result);
            request.onerror = () => reject(request.error);
        });
    }

    async clear() {
        if (!this.db) {
            await this.init();
        }

        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(['searchResults'], 'readwrite');
            const objectStore = transaction.objectStore('searchResults');
            const request = objectStore.clear();

            request.onsuccess = () => resolve(true);
            request.onerror = () => reject(request.error);
        });
    }
}