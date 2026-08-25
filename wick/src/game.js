import { circleHits, circleHitsAabb, dist, moveAndSlide } from "./physics.js";
import {
  BRAWL_HITS,
  BRAWL_WINDOW,
  GRAB_SPEED,
  NOISE_RADIUS,
  bestBrawl,
  bestStealth,
  facingOf,
  pinBehind,
} from "./melee.js";

const PLAYER_SPEED = 190;
const DASH_SPEED = 460;
const DASH_TIME = 0.14;
const DASH_CD = 0.55;
const DASH_OIL = 1;
const FLARE_OIL = 2.2;
const FLARE_STUN = 1.15;
const OIL_DRAIN = 1.35;
const PLAYER_R = 11;

export class Game {
  constructor(catalog) {
    this.catalog = catalog;
    this.dev = {
      on: true,
      infiniteOil: false,
      freezeGuards: false,
      mouse: { x: 0, y: 0 },
    };
    this.shake = 0;
    this.run = { stolen: 0, deaths: 0 };
    this.status = "title";
    this.touch = false;
    this.levelId = catalog.ids[0];
    this.time = 0;
    this.bootLevel(this.levelId);
  }

  bootLevel(id) {
    const def = this.catalog.get(id);
    this.levelId = def.id;
    this.def = def;
    this.time = 0;
    this.flare = 0;
    this.flash = 0;
    this.melee = null;
    this.prompt = null;
    this.player = {
      x: def.player.x,
      y: def.player.y,
      vx: 0,
      vy: 0,
      r: PLAYER_R,
      oil: def.oil,
      maxOil: def.oil,
      lantern: true,
      dashT: 0,
      dashCd: 0,
      hasRelic: false,
    };
    this.relic = def.relic.taken
      ? null
      : { x: def.relic.x, y: def.relic.y, r: def.relic.r || 12, taken: false };
    this.exit = { x: def.exit.x, y: def.exit.y, r: def.exit.r || 26 };
    this.oilCans = (def.oilCans || []).map((c) => ({
      x: c.x,
      y: c.y,
      r: c.r || 10,
      amount: c.amount || 4,
      taken: false,
    }));
    this.guards = def.guards.map((g, i) => ({
      id: i,
      x: g.x,
      y: g.y,
      r: g.r || 13,
      speed: g.speed || 72,
      patrol: g.patrol?.length ? g.patrol.map((p) => ({ x: p[0], y: p[1] })) : [{ x: g.x, y: g.y }],
      patrolI: 0,
      stun: 0,
      alert: 0,
      vx: 0,
      vy: 0,
      facing: 0,
      down: false,
      held: false,
    }));
    for (const g of this.guards) {
      const a = g.patrol[0];
      const b = g.patrol[1];
      if (a && b) g.facing = Math.atan2(b.y - a.y, b.x - a.x);
    }
  }

  visionRadius() {
    if (!this.player.lantern) return 58;
    return 150 + this.player.oil * 4;
  }

  update(dt, input) {
    if (this.shake > 0) this.shake = Math.max(0, this.shake - dt * 18);

    if (this.status === "title") {
      if (input.consume("Space") || input.consume("Enter") || input.consume("KeyE")) {
        this.status = "play";
        this.bootLevel(this.catalog.ids[0]);
      }
      input.endFrame();
      return;
    }

    if (this.status !== "play") {
      if (input.consume("Space") || input.consume("KeyR") || input.consume("Enter")) {
        if (this.status === "run_complete") {
          this.run = { stolen: 0, deaths: 0 };
          this.bootLevel(this.catalog.ids[0]);
        } else {
          this.bootLevel(this.levelId);
        }
        this.status = "play";
      }
      input.endFrame();
      return;
    }

    this.time += dt;
    this.flash = Math.max(0, (this.flash || 0) - dt);
    const p = this.player;
    const meleeTap =
      input.consume("KeyC") ||
      input.consume("ControlLeft") ||
      input.consume("ControlRight") ||
      input.consume("TouchTakedown");

    if (!this.melee) {
      if ((input.consume("KeyE") || input.consume("Space") || input.consume("TouchLantern")) && p.oil > 0) {
        p.lantern = !p.lantern;
      }
    }
    if (p.oil <= 0) p.lantern = false;

    if (!this.melee && (input.consume("KeyF") || input.consume("KeyQ")) && p.dashCd <= 0 && p.oil >= FLARE_OIL) {
      if (!this.dev.infiniteOil) p.oil -= FLARE_OIL;
      this.flare = 0.35;
      this.shake = 8;
      for (const g of this.guards) {
        if (!g.down && dist(p, g) < this.visionRadius() + 40) g.stun = FLARE_STUN;
      }
    }

    this.prompt = null;
    if (!this.melee) {
      const stealth = bestStealth(p, this.guards);
      const brawl = bestBrawl(p, this.guards);
      if (stealth) this.prompt = { kind: "stealth", guard: stealth };
      else if (brawl) this.prompt = { kind: "brawl", guard: brawl };
      if (meleeTap && this.prompt) this.beginMelee(this.prompt);
    } else if (meleeTap) {
      this.meleeStrike();
    }

    const axis = input.axis();
    if (this.melee) {
      this.tickMelee(dt, axis);
    } else if (p.dashT > 0) {
      p.dashT -= dt;
      moveAndSlide(p, this.def.walls, dt);
    } else {
      p.vx = axis.x * PLAYER_SPEED;
      p.vy = axis.y * PLAYER_SPEED;
      if ((input.consume("ShiftLeft") || input.consume("ShiftRight")) && p.dashCd <= 0 && (axis.x || axis.y)) {
        p.dashT = DASH_TIME;
        p.dashCd = DASH_CD;
        p.vx = axis.x * DASH_SPEED;
        p.vy = axis.y * DASH_SPEED;
        if (!this.dev.infiniteOil) p.oil = Math.max(0, p.oil - DASH_OIL);
      }
      moveAndSlide(p, this.def.walls, dt);
    }
    p.dashCd = Math.max(0, p.dashCd - dt);

    if (p.lantern && !this.dev.infiniteOil) {
      p.oil = Math.max(0, p.oil - OIL_DRAIN * dt);
    }

    if (this.relic && !this.relic.taken && circleHits(p, this.relic)) {
      this.relic.taken = true;
      p.hasRelic = true;
    }
    for (const can of this.oilCans) {
      if (!can.taken && circleHits(p, can)) {
        can.taken = true;
        p.oil = Math.min(p.maxOil + 6, p.oil + can.amount);
        p.maxOil = Math.max(p.maxOil, p.oil);
      }
    }

    this.flare = Math.max(0, this.flare - dt);
    this.updateGuards(dt);

    if (p.hasRelic && circleHits(p, this.exit)) this.winLevel();

    input.endFrame();
  }

  winLevel() {
    this.run.stolen += 1;
    const i = this.catalog.ids.indexOf(this.levelId);
    if (i < 0 || i >= this.catalog.ids.length - 1) {
      this.status = "run_complete";
      return;
    }
    this.bootLevel(this.catalog.ids[i + 1]);
  }

  updateGuards(dt) {
    const p = this.player;
    const seeR = p.lantern ? this.visionRadius() * 0.78 : 42;
    for (const g of this.guards) {
      if (g.down || g.held) {
        g.vx = 0;
        g.vy = 0;
        continue;
      }
      g.stun = Math.max(0, g.stun - dt);
      if (this.dev.freezeGuards || g.stun > 0) {
        g.vx = 0;
        g.vy = 0;
        continue;
      }
      const d = dist(g, p);
      const sees = d < seeR && hasLos(g, p, this.def.walls);
      if (sees) g.alert = 1.6;
      else g.alert = Math.max(0, g.alert - dt);

      let tx;
      let ty;
      let speed = g.speed;
      if (g.alert > 0) {
        tx = p.x;
        ty = p.y;
        speed *= 1.45;
      } else {
        const node = g.patrol[g.patrolI % g.patrol.length];
        tx = node.x;
        ty = node.y;
        if (Math.hypot(g.x - tx, g.y - ty) < 10) g.patrolI += 1;
      }
      const ang = Math.atan2(ty - g.y, tx - g.x);
      g.facing = ang;
      g.vx = Math.cos(ang) * speed;
      g.vy = Math.sin(ang) * speed;
      moveAndSlide(g, this.def.walls, dt);

      if (this.status === "play" && circleHits(g, p) && !this.melee) {
        if (bestStealth(p, [g])) continue;
        this.status = "dead";
        this.run.deaths += 1;
        this.shake = 14;
      }
    }
  }

  beginMelee(prompt) {
    const g = prompt.guard;
    if (!g || g.down) return;
    g.held = true;
    g.vx = 0;
    g.vy = 0;
    g.facing = facingOf(g);
    if (prompt.kind === "stealth") {
      pinBehind(this.player, g);
      this.melee = { guard: g, phase: "choke", t: 0, silent: true, hits: 0 };
      this.prompt = { kind: "finish", guard: g };
      this.player.lantern = false;
    } else {
      this.melee = { guard: g, phase: "brawl", t: 0, silent: false, hits: 1 };
      this.prompt = { kind: "brawl", guard: g };
      this.shake = 7;
      this.flash = 0.12;
      this.noiseAlert(g, NOISE_RADIUS * 0.55);
    }
  }

  meleeStrike() {
    const m = this.melee;
    if (!m) return;
    if (m.phase === "choke") {
      this.finishTakedown(m, true);
      return;
    }
    if (m.phase === "brawl") {
      m.hits += 1;
      this.shake = 6;
      this.flash = 0.1;
      if (m.hits >= BRAWL_HITS) this.finishTakedown(m, false);
    }
  }

  tickMelee(dt, axis) {
    const m = this.melee;
    const g = m.guard;
    const p = this.player;
    m.t += dt;
    if (m.phase === "choke") {
      g.held = true;
      p.lantern = false;
      if (axis.x || axis.y) {
        g.facing = Math.atan2(axis.y, axis.x);
        g.vx = axis.x * GRAB_SPEED;
        g.vy = axis.y * GRAB_SPEED;
        moveAndSlide(g, this.def.walls, dt);
      } else {
        g.vx = 0;
        g.vy = 0;
      }
      pinBehind(p, g);
      this.prompt = { kind: "finish", guard: g };
      return;
    }
    if (m.phase === "brawl") {
      p.vx = 0;
      p.vy = 0;
      g.vx = 0;
      g.vy = 0;
      this.prompt = { kind: "brawl", guard: g };
      if (m.t >= BRAWL_WINDOW) {
        this.melee = null;
        g.held = false;
        this.status = "dead";
        this.run.deaths += 1;
        this.shake = 14;
      }
    }
  }

  finishTakedown(m, silent) {
    const g = m.guard;
    g.down = true;
    g.held = false;
    g.alert = 0;
    g.stun = 0;
    g.vx = 0;
    g.vy = 0;
    this.melee = null;
    this.prompt = null;
    this.shake = silent ? 4 : 10;
    this.flash = silent ? 0.05 : 0.18;
    if (!silent) this.noiseAlert(g, NOISE_RADIUS);
  }

  noiseAlert(from, radius) {
    for (const g of this.guards) {
      if (g.down || g === from) continue;
      if (dist(g, from) < radius) g.alert = Math.max(g.alert, 2.2);
    }
  }

  giveOil(n = 6) {
    this.player.oil = Math.min(this.player.maxOil + n, this.player.oil + n);
  }

  stealRelic() {
    if (this.relic) this.relic.taken = true;
    this.player.hasRelic = true;
  }

  extractNow() {
    this.stealRelic();
    this.winLevel();
  }

  kill() {
    if (this.status === "play") {
      this.status = "dead";
      this.run.deaths += 1;
      this.shake = 14;
    }
  }
}

function hasLos(a, b, walls) {
  const steps = 10;
  for (let i = 1; i < steps; i++) {
    const t = i / steps;
    const probe = { x: a.x + (b.x - a.x) * t, y: a.y + (b.y - a.y) * t, r: 2 };
    for (const wall of walls) {
      if (circleHitsAabb(probe, wall)) return false;
    }
  }
  return true;
}
