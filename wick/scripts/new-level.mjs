import { readIndex, readLevel, templateFrom, writeIndex, writeLevel } from "../src/levels.js";

const id = (process.argv[2] || "").trim();
if (!/^[a-z0-9_]+$/.test(id)) {
  console.error("usage: npm run new-level -- my_level_id");
  process.exit(1);
}

const ids = readIndex();
if (ids.includes(id)) {
  console.error(`${id} already exists`);
  process.exit(1);
}

const def = templateFrom(readLevel(ids[0]), id);
writeLevel(def);
writeIndex([...ids, id]);
console.log(`wrote public/levels/${id}.json and added it to the run`);
console.log("edit the JSON, save, press 4 in-game (or wait ~1s) — the hole restamps");
