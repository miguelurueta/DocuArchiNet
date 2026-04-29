type Entry<V> = { value: V };

export class LruCache<K, V> {
  private readonly maxEntries: number;
  private readonly map = new Map<K, Entry<V>>();

  constructor({ maxEntries }: { maxEntries: number }) {
    this.maxEntries = Math.max(1, Math.floor(maxEntries));
  }

  get(key: K): V | undefined {
    const entry = this.map.get(key);
    if (!entry) return undefined;
    this.map.delete(key);
    this.map.set(key, entry);
    return entry.value;
  }

  set(key: K, value: V) {
    if (this.map.has(key)) {
      this.map.delete(key);
    }
    this.map.set(key, { value });
    this.evictIfNeeded();
  }

  delete(key: K) {
    this.map.delete(key);
  }

  clear() {
    this.map.clear();
  }

  size() {
    return this.map.size;
  }

  private evictIfNeeded() {
    while (this.map.size > this.maxEntries) {
      const oldestKey = this.map.keys().next().value as K | undefined;
      if (oldestKey === undefined) return;
      this.map.delete(oldestKey);
    }
  }
}

