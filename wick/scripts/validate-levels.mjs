import { isComplete, listLevelFiles, readIndex, readLevel } from "../src/levels.js";

let failed = 0;
const ids = readIndex();
const files = listLevelFiles();

for (const id of ids) {
  if (!files.includes(id)) {
    console.error(`index lists ${id} but ${id}.json is missing`);
    failed++;
    continue;
  }
  const level = readLevel(id);
  const err = isComplete(level);
  if (err) {
    console.error(`${id}: ${err}`);
    failed++;
    continue;
  }
  if (level.id !== id) {
    console.error(`${id}: json id is ${level.id}`);
    failed++;
  }
  console.log(`ok  ${id}  (${level.guards.length} guards, ${level.walls.length} walls)`);
}

for (const file of files) {
  if (!ids.includes(file)) {
    console.error(`${file}.json exists but is not in index.json — it will not appear in the run`);
    failed++;
  }
}

if (failed) {
  console.error(`FAIL ${failed}`);
  process.exit(1);
}
console.log(`PASS ${ids.length} levels`);
