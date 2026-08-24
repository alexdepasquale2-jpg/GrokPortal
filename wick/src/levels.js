import { readFileSync, existsSync, readdirSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import { isComplete } from "./schema.js";

export { isComplete };

const here = dirname(fileURLToPath(import.meta.url));
export const LEVELS_DIR = join(here, "..", "public", "levels");

export function readIndex() {
  const raw = JSON.parse(readFileSync(join(LEVELS_DIR, "index.json"), "utf8"));
  if (!raw?.ids?.length) throw new Error("levels/index.json needs an ids array");
  return raw.ids;
}

export function readLevel(id) {
  const path = join(LEVELS_DIR, `${id}.json`);
  if (!existsSync(path)) throw new Error(`no level file: ${id}.json`);
  return JSON.parse(readFileSync(path, "utf8"));
}

export function listLevelFiles() {
  return readdirSync(LEVELS_DIR)
    .filter((f) => f.endsWith(".json") && f !== "index.json")
    .map((f) => f.replace(/\.json$/, ""));
}

export function writeLevel(level) {
  writeFileSync(join(LEVELS_DIR, `${level.id}.json`), JSON.stringify(level, null, 2) + "\n");
}

export function writeIndex(ids) {
  writeFileSync(join(LEVELS_DIR, "index.json"), JSON.stringify({ ids }, null, 2) + "\n");
}

export function templateFrom(base, id) {
  const copy = structuredClone(base);
  copy.id = id;
  copy.name = id.replace(/_/g, " ");
  return copy;
}
