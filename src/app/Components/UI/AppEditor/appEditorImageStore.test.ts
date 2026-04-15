import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { appEditorImageStore } from "./infrastructure/indexeddb/appEditorImageStore";
import type { LocalImage } from "./infrastructure/indexeddb/localImage.types";

class FakeRequest<T> {
  result!: T;
  error: Error | null = null;
  onsuccess: ((event: Event) => void) | null = null;
  onerror: ((event: Event) => void) | null = null;
  onupgradeneeded: ((event: Event) => void) | null = null;

  succeed(result: T) {
    this.result = result;
    queueMicrotask(() => {
      this.onsuccess?.(new Event("success"));
    });
  }
}

class FakeIndex {
  private readonly records: Map<string, LocalImage>;
  private readonly keyPath: "documentDraftId" | "sessionId";

  constructor(records: Map<string, LocalImage>, keyPath: "documentDraftId" | "sessionId") {
    this.records = records;
    this.keyPath = keyPath;
  }

  getAll(value: string) {
    const request = new FakeRequest<LocalImage[]>() as unknown as IDBRequest<LocalImage[]>;
    const matches = [...this.records.values()].filter((record) => record[this.keyPath] === value);
    (request as unknown as FakeRequest<LocalImage[]>).succeed(matches);
    return request;
  }
}

class FakeObjectStore {
  private readonly indexes = new Map<string, "documentDraftId" | "sessionId">();
  private readonly records: Map<string, LocalImage>;

  constructor(records: Map<string, LocalImage>) {
    this.records = records;
  }

  createIndex(name: string, keyPath: "documentDraftId" | "sessionId") {
    this.indexes.set(name, keyPath);
  }

  index(name: string) {
    return new FakeIndex(this.records, this.indexes.get(name)!);
  }

  put(record: LocalImage) {
    const request = new FakeRequest<LocalImage>() as unknown as IDBRequest<LocalImage>;
    this.records.set(record.id, record);
    (request as unknown as FakeRequest<LocalImage>).succeed(record);
    return request;
  }

  get(id: string) {
    const request = new FakeRequest<LocalImage | undefined>() as unknown as IDBRequest<
      LocalImage | undefined
    >;
    (request as unknown as FakeRequest<LocalImage | undefined>).succeed(this.records.get(id));
    return request;
  }

  delete(id: string) {
    const request = new FakeRequest<undefined>() as unknown as IDBRequest<undefined>;
    this.records.delete(id);
    (request as unknown as FakeRequest<undefined>).succeed(undefined);
    return request;
  }
}

class FakeDb {
  readonly objectStoreNames = {
    contains: (name: string) => this.storeNames.has(name),
  };

  private readonly storeNames = new Set<string>();
  private readonly stores = new Map<string, FakeObjectStore>();

  createObjectStore(name: string) {
    this.storeNames.add(name);
    const store = new FakeObjectStore(fakeRecords);
    this.stores.set(name, store);
    return store as unknown as IDBObjectStore;
  }

  transaction(name: string) {
    return {
      objectStore: () => this.stores.get(name) as unknown as IDBObjectStore,
    } as unknown as IDBTransaction;
  }

  close() {}
}

const fakeRecords = new Map<string, LocalImage>();
const fakeDb = new FakeDb();
const originalIndexedDb = window.indexedDB;

beforeEach(() => {
  fakeRecords.clear();

  window.indexedDB = {
    open: () => {
      const request = new FakeRequest<IDBDatabase>();

      queueMicrotask(() => {
        request.result = fakeDb as unknown as IDBDatabase;
        request.onupgradeneeded?.(new Event("upgradeneeded"));
        request.onsuccess?.(new Event("success"));
      });

      return request as unknown as IDBOpenDBRequest;
    },
  } as unknown as IDBFactory;
});

afterEach(() => {
  window.indexedDB = originalIndexedDb;
});

describe("appEditorImageStore", () => {
  it("guarda y recupera una imagen local", async () => {
    const image: LocalImage = {
      id: "img_local_1",
      fileName: "logo.png",
      contentType: "image/png",
      size: 10,
      blob: new Blob(["img"], { type: "image/png" }),
      createdAt: Date.now(),
      sessionId: "session-1",
    };

    await appEditorImageStore.init();
    await appEditorImageStore.saveImage(image);

    const result = await appEditorImageStore.getImage("img_local_1");

    expect(result).not.toBeNull();
    expect(result?.fileName).toBe("logo.png");
    expect(result?.sessionId).toBe("session-1");
  });

  it("elimina imagenes por scope", async () => {
    await appEditorImageStore.saveImage({
      id: "img_local_1",
      fileName: "uno.png",
      contentType: "image/png",
      size: 10,
      blob: new Blob(["1"], { type: "image/png" }),
      createdAt: Date.now(),
      sessionId: "session-a",
    });

    await appEditorImageStore.saveImage({
      id: "img_local_2",
      fileName: "dos.png",
      contentType: "image/png",
      size: 10,
      blob: new Blob(["2"], { type: "image/png" }),
      createdAt: Date.now(),
      sessionId: "session-b",
    });

    await appEditorImageStore.clearByScope({ sessionId: "session-a" });

    expect(await appEditorImageStore.getImage("img_local_1")).toBeNull();
    expect(await appEditorImageStore.getImage("img_local_2")).not.toBeNull();
  });
});
