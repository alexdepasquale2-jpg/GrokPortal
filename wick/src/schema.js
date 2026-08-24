export function isComplete(level) {
  if (!level || typeof level !== "object") return "missing object";
  if (!level.id) return "missing id";
  if (!level.name) return "missing name";
  if (!(level.w > 0) || !(level.h > 0)) return "bad size";
  if (!(level.oil > 0)) return "oil must be > 0";
  if (!level.player || level.player.x == null || level.player.y == null) return "missing player";
  if (!level.exit || level.exit.x == null || level.exit.y == null) return "missing exit";
  if (!level.relic || level.relic.x == null || level.relic.y == null) return "missing relic";
  if (!Array.isArray(level.walls) || level.walls.length < 4) return "need at least 4 walls (the bounds)";
  if (!Array.isArray(level.guards) || level.guards.length < 1) return "need at least one guard";
  return null;
}
