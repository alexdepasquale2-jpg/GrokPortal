import { Catalog } from "../src/catalog.js";
import { Game } from "../src/game.js";
import { BRAWL_HITS, BRAWL_WINDOW, CHOKE_AUTO, REACH } from "../src/melee.js";

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
step(auto, CHOKE_AUTO + 0.05);
expect("choke finishes on its own if you hold", auto.guards[0].down && !auto.melee);

const face = play();
face.player.x = 175;
face.player.y = 200;
face.guards[0].x = 200;
face.guards[0].y = 200;
face.guards[0].facing = 0;
face.guards[0].alert = 1;
step(face, 0.016);
expect("alert + close is a brawl prompt", face.prompt?.kind === "brawl");
const fight = new TapInput();
fight.tap("KeyC");
step(face, 0.016, fight);
expect("C into an alert hunter starts a fight", face.melee?.phase === "brawl");
for (let i = 0; i < BRAWL_HITS; i++) {
  const mash = new TapInput();
  mash.tap("KeyC");
  step(face, 0.05, mash);
}
expect("mashing C wins a noisy fight", face.guards[0].down && face.status === "play");
expect("a loud KO wakes nearby hunters", face.guards[1].alert > 0);

const lose = play();
lose.player.x = 175;
lose.player.y = 200;
lose.guards[0].alert = 1;
const startFight = new TapInput();
startFight.tap("KeyC");
step(lose, 0.016, startFight);
step(lose, BRAWL_WINDOW + 0.05);
expect("losing the mash is death once", lose.status === "dead" && lose.run.deaths === 1);

const vis = play();
vis.player.lantern = false;
expect("hidden vision covers melee reach", vis.visionRadius() >= REACH);

if (process.exitCode) {
  console.error("FAIL takedown");
  process.exit(1);
}
console.log("PASS takedown");
