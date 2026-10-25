import { Catalog } from "../src/catalog.js";
import { Game } from "../src/game.js";
import { COMBO_END, COMBO_STEP, REACH } from "../src/melee.js";

function expect(name, ok) {
  if (!ok) {
    console.error("FAIL " + name);
    process.exitCode = 1;
  } else {
    console.log("ok   " + name);
  }
}

class TapInput {
  constructor() {
    this.codes = new Set();
    this.stick = { x: 0, y: 0 };
  }
  tap(code) {
    this.codes.add(code);
  }
  consume(code) {
    if (!this.codes.has(code)) return false;
    this.codes.delete(code);
    return true;
  }
  axis() {
    return { x: this.stick.x, y: this.stick.y };
  }
  endFrame() {
    this.codes.clear();
  }
}

function stubCatalog() {
  const level = {
    id: "dojo",
    name: "Dojo",
    w: 400,
    h: 400,
    oil: 10,
    player: { x: 80, y: 200 },
    exit: { x: 350, y: 200, r: 20 },
    relic: { x: 300, y: 200, r: 12 },
    oilCans: [],
    walls: [
      { x: 0, y: 0, w: 400, h: 10 },
      { x: 0, y: 390, w: 400, h: 10 },
      { x: 0, y: 0, w: 10, h: 400 },
      { x: 390, y: 0, w: 10, h: 400 },
    ],
    guards: [
      { x: 200, y: 200, r: 13, speed: 0, patrol: [[200, 200], [280, 200]] },
      { x: 90, y: 200, r: 13, speed: 0, patrol: [[90, 200]] },
    ],
  };
  return new Catalog(["dojo"], { dojo: level });
}

function play() {
  const game = new Game(stubCatalog());
  game.status = "play";
  game.dev.on = false;
  game.dev.freezeGuards = true;
  game.player.lantern = false;
  return game;
}

function step(game, dt, input = new TapInput()) {
  game.update(dt, input);
}

const rear = play();
rear.player.x = 168;
rear.player.y = 200;
rear.guards[0].x = 200;
rear.guards[0].y = 200;
rear.guards[0].facing = 0;
step(rear, 0.016);
expect("rear approach prompts a stealth take", rear.prompt?.kind === "stealth");
expect("clipping a back does not kill", rear.status === "play" && rear.run.deaths === 0);

const grab = play();
grab.player.x = 168;
grab.player.y = 200;
grab.guards[0].facing = 0;
const grabIn = new TapInput();
grabIn.tap("KeyC");
step(grab, 0.016, grabIn);
expect("C from behind starts a choke", grab.melee?.phase === "choke" && grab.guards[0].held);
expect("grab kills the lantern", grab.player.lantern === false);

grabIn.stick.x = 1;
step(grab, 0.2, grabIn);
expect("stick drags the grab pair", grab.guards[0].x > 200);

const finishIn = new TapInput();
finishIn.tap("KeyC");
step(grab, 0.016, finishIn);
expect("second C drops them silent", grab.guards[0].down && !grab.melee);
expect("the body stays in the world", grab.guards.length === 2 && grab.guards[0].down);
expect("silent KO does not wake the far hunter", grab.guards[1].alert === 0);

const auto = play();
auto.player.x = 168;
auto.player.y = 200;
auto.guards[0].facing = 0;
const autoIn = new TapInput();
autoIn.tap("KeyC");
step(auto, 0.016, autoIn);
step(auto, 2.5);
expect("choke stays a grab until you tap C", auto.melee?.phase === "choke" && !auto.guards[0].down);

const face = play();
face.player.x = 175;
face.player.y = 200;
face.guards[0].x = 200;
face.guards[0].y = 200;
face.guards[0].facing = 0;
face.guards[0].alert = 1;
step(face, 0.016);
expect("alert + close offers the combo", face.prompt?.kind === "brawl");
const fight = new TapInput();
fight.tap("KeyC");
step(face, 0.016, fight);
expect("one press starts the combo", face.melee?.phase === "combo");
expect("the first punch lands on that press", face.melee?.hits === 1);
step(face, COMBO_STEP * 2);
expect("the combo keeps swinging with no extra input", face.melee?.hits > 1);
step(face, COMBO_END);
expect("the combo ends in a KO on its own", face.guards[0].down && face.status === "play");
expect("winning the combo does not kill you", face.run.deaths === 0);
expect("a loud KO wakes nearby hunters", face.guards[1].alert > 0);

const haul = play();
haul.guards[0].down = true;
haul.guards[0].x = 200;
haul.guards[0].y = 200;
haul.player.x = 220;
haul.player.y = 200;
step(haul, 0.016);
expect("standing over a body offers a drag", haul.prompt?.kind === "body");
const grabBody = new TapInput();
grabBody.tap("KeyC");
step(haul, 0.016, grabBody);
expect("C on a body starts hauling", haul.melee?.phase === "haul");
const hauling = new TapInput();
hauling.stick.x = 1;
for (let i = 0; i < 30; i++) step(haul, 0.033, hauling);
expect("the body follows you", haul.guards[0].x > 200);
expect("the body stays in tow", haul.melee?.phase === "haul");
const drop = new TapInput();
drop.tap("KeyC");
step(haul, 0.016, drop);
expect("C again lets go of the body", !haul.melee && haul.guards[0].down);
expect("hauling a body is never a fight", haul.status === "play");

const vis = play();
vis.player.lantern = false;
expect("hidden vision covers melee reach", vis.visionRadius() >= REACH);

if (process.exitCode) {
  console.error("FAIL takedown");
  process.exit(1);
}
console.log("PASS takedown");
