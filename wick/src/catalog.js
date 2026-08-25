import { isComplete } from "./schema.js";

export class Catalog {
  constructor(ids, byId) {
    this.ids = ids;
    this.byId = byId;
  }

  static async load() {
    const index = await fetch("/levels/index.json").then((r) => r.json());
    const byId = {};
    for (const id of index.ids) {
      const level = await fetch(`/levels/${id}.json`).then((r) => r.json());
      const err = isComplete(level);
      if (err) console.warn(`[WICK] ${id}: ${err}`);
      byId[id] = level;
    }
    return new Catalog(index.ids, byId);
  }

  get(id) {
    return this.byId[id] || this.byId[this.ids[0]];
  }

  nextId(id) {
    const i = this.ids.indexOf(id);
    if (i < 0) return this.ids[0];
    return this.ids[(i + 1) % this.ids.length];
  }

  async reload(id) {
    await this.refreshIndex();
    const level = await fetch(`/levels/${id}.json?t=${Date.now()}`).then((r) => r.json());
    const err = isComplete(level);
    if (err) throw new Error(err);
    this.byId[id] = level;
    if (!this.ids.includes(id)) this.ids.push(id);
    return level;
  }

  async refreshIndex() {
    const index = await fetch(`/levels/index.json?t=${Date.now()}`).then((r) => r.json());
    for (const id of index.ids) {
      if (!this.byId[id]) {
        const level = await fetch(`/levels/${id}.json?t=${Date.now()}`).then((r) => r.json());
        this.byId[id] = level;
      }
    }
    this.ids = index.ids.slice();
  }
}
