import {
  bestBody,
  bestBrawl,
  bestStealth,
  canBrawl,
  canHaul,
  canStealth,
  isBehind,
} from "../src/melee.js";

function expect(name, ok) {
  if (!ok) {
    console.error("FAIL " + name);
    process.exitCode = 1;
  } else {
    console.log("ok   " + name);
  }
}

const north = { x: 100, y: 100, vx: 0, vy: -80, facing: -Math.PI / 2, alert: 0, stun: 0 };
const behind = { x: 100, y: 130 };
const front = { x: 100, y: 70 };
const side = { x: 140, y: 100 };

expect("behind a northbound guard is stealth geometry", isBehind(behind, north));
expect("in front is not behind", !isBehind(front, north));
expect("off to the side is not a rear takedown", !isBehind(side, north));

expect(
  "unaware + behind + close = stealth",
  canStealth(behind, { ...north, r: 13 }) && distOk(behind, north)
);

function distOk(a, b) {
  return Math.hypot(a.x - b.x, a.y - b.y) < 50;
}

expect(
  "alerted guard is a brawl, not a ghost take",
  canBrawl(behind, { ...north, alert: 1, r: 13 }) && !canStealth(behind, { ...north, alert: 1, r: 13 })
);

const guards = [
  { ...north, down: true, r: 13 },
  { x: 100, y: 100, vx: 0, vy: -80, facing: -Math.PI / 2, alert: 0, stun: 0, r: 13 },
];
expect("downed bodies are not targets", !bestStealth(behind, [guards[0]]));
expect("live rear target is picked", bestStealth(behind, guards) === guards[1]);
expect("no brawl if everyone is calm", !bestBrawl(behind, guards));

const stunned = { ...north, alert: 1, stun: 1, r: 13 };
expect("a flashed hunter can still be choked from behind", canStealth(behind, stunned));

const body = { x: 100, y: 100, vx: 0, vy: 0, facing: 0, alert: 0, stun: 0, down: true, r: 13 };
expect("a body in reach can be hauled", canHaul({ x: 100, y: 120 }, body));
expect("a body across the room cannot", !canHaul({ x: 100, y: 400 }, body));
expect("only bodies can be hauled", !canHaul({ x: 100, y: 120 }, { ...body, down: false }));
expect("the nearest body is picked", bestBody({ x: 100, y: 120 }, [body]) === body);

if (process.exitCode) {
  console.error("FAIL melee");
  process.exit(1);
}
console.log("PASS melee");
