import { circleHits, circleHitsAabb, dist, moveAndSlide } from "./physics.js";

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
    }));
  }

  visionRadius() {
    if (!this.player.lantern) return 34;
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
    const p = this.player;
    if (input.consume("KeyE") || input.consume("Space")) {
      if (p.oil > 0.15) p.lantern = !p.lantern;
    }
    if (p.oil <= 0) p.lantern = false;

    if ((input.consume("KeyF") || input.consume("KeyQ")) && p.dashCd <= 0 && p.oil >= FLARE_OIL) {
      if (!this.dev.infiniteOil) p.oil -= FLARE_OIL;
      this.flare = 0.35;
      this.shake = 8;
      for (const g of this.guards) {
        if (dist(p, g) < this.visionRadius() + 40) g.stun = FLARE_STUN;
      }
    }

    const axis = input.axis();
    if (p.dashT > 0) {
      p.dashT -= dt;
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
    }
    p.dashCd = Math.max(0, p.dashCd - dt);
    moveAndSlide(p, this.def.walls, dt);

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
      g.vx = Math.cos(ang) * speed;
      g.vy = Math.sin(ang) * speed;
      moveAndSlide(g, this.def.walls, dt);

      if (circleHits(g, p)) {
        this.status = "dead";
        this.run.deaths += 1;
        this.shake = 14;
      }
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
