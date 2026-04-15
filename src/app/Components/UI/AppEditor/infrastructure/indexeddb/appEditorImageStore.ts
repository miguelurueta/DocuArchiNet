import type { LocalImage, LocalImageScope } from "./localImage.types";

const DB_NAME = "app-editor-images";
const DB_VERSION = 1;
const STORE_NAME = "local-images";

type PersistedLocalImage = LocalImage;

function getIndexedDb() {
  if (typeof window === "undefined" || !("indexedDB" in window) || !window.indexedDB) {
    throw new Error("IndexedDB no esta disponible en este entorno.");
  }

  return window.indexedDB;
}

function openDb() {
  const indexedDb = getIndexedDb();

  return new Promise<IDBDatabase>((resolve, reject) => {
    const request = indexedDb.open(DB_NAME, DB_VERSION);

    request.onupgradeneeded = () => {
      const db = request.result;

      if (!db.objectStoreNames.contains(STORE_NAME)) {
        const store = db.createObjectStore(STORE_NAME, { keyPath: "id" });
        store.createIndex("documentDraftId", "documentDraftId", { unique: false });
        store.createIndex("sessionId", "sessionId", { unique: false });
      }
    };

    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error ?? new Error("No fue posible abrir IndexedDB."));
  });
}

function runRequest<T = undefined>(request: IDBRequest<T>) {
  return new Promise<T>((resolve, reject) => {
    request.onsuccess = () => resolve(request.result);
    request.onerror = () => reject(request.error ?? new Error("Operacion IndexedDB fallida."));
  });
}

async function withStore<T>(
  mode: IDBTransactionMode,
  callback: (store: IDBObjectStore) => Promise<T> | T,
) {
  const db = await openDb();

  try {
    const transaction = db.transaction(STORE_NAME, mode);
    const store = transaction.objectStore(STORE_NAME);
    return await callback(store);
  } finally {
    db.close();
  }
}

async function clearByIndex(indexName: "documentDraftId" | "sessionId", value: string) {
  await withStore("readwrite", async (store) => {
    const index = store.index(indexName);
    const records = (await runRequest(index.getAll(value))) as PersistedLocalImage[];

    await Promise.all(records.map((record) => runRequest(store.delete(record.id))));
  });
}

export const appEditorImageStore = {
  async init() {
    const db = await openDb();
    db.close();
  },

  async saveImage(image: LocalImage) {
    await withStore("readwrite", (store) => runRequest(store.put(image)));
    return image;
  },

  async getImage(id: string) {
    return withStore("readonly", async (store) => {
      const image = (await runRequest(store.get(id))) as PersistedLocalImage | undefined;
      return image ?? null;
    });
  },

  async deleteImage(id: string) {
    await withStore("readwrite", (store) => runRequest(store.delete(id)));
  },

  async clearByScope(scope: LocalImageScope) {
    if (scope.documentDraftId) {
      await clearByIndex("documentDraftId", scope.documentDraftId);
    }

    if (scope.sessionId) {
      await clearByIndex("sessionId", scope.sessionId);
    }
  },
};
