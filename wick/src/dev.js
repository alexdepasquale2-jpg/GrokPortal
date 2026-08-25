import { eventToWorld } from "./render.js";

export class Dev {
  constructor(game, catalog, canvas) {
    this.game = game;
    this.catalog = catalog;
    this.note = "";
    this.pollAt = 0;
    this.lastJson = "";
    canvas.addEventListener("click", (e) => {
      if (!game.dev.on || game.touch) return;
      const p = eventToWorld(e, canvas, game);
      game.dev.mouse = p;
      this.note = `click ${p.x},${p.y}`;
      console.log(`[WICK] click ${p.x},${p.y}`);
    });
  }

  async tick(dt, input) {
    if (input.consume("F1") || input.consume("Backquote")) {
      this.game.dev.on = !this.game.dev.on;
    }
    if (!this.game.dev.on) return;

    if (input.consume("Digit0") || input.consume("Numpad0")) {
      const g = this.game.guards.find((x) => !x.down && !x.held);
      if (g) {
        const ang = g.facing ?? 0;
        this.game.player.lantern = false;
        this.game.player.x = g.x - Math.cos(ang) * 28;
        this.game.player.y = g.y - Math.sin(ang) * 28;
        this.note = "behind";
      }
    }
    if (input.consume("Digit1") || input.consume("Numpad1")) {
      this.game.giveOil(8);
      this.note = "+oil";
    }
    if (input.consume("Digit2") || input.consume("Numpad2")) {
      this.game.stealRelic();
      this.note = "relic";
    }
    if (input.consume("Digit3") || input.consume("Numpad3")) {
      this.game.extractNow();
      this.note = "extract";
    }
    if (input.consume("Digit4") || input.consume("Numpad4")) {
      await this.reload();
    }
    if (input.consume("Digit5") || input.consume("Numpad5")) {
      await this.catalog.refreshIndex();
      const id = this.catalog.nextId(this.game.levelId);
      this.game.bootLevel(id);
      this.game.status = "play";
      this.lastJson = "";
      this.note = id;
    }
    if (input.consume("Digit6") || input.consume("Numpad6")) {
      this.game.dev.infiniteOil = !this.game.dev.infiniteOil;
      this.note = this.game.dev.infiniteOil ? "inf oil" : "oil on";
    }
    if (input.consume("Digit7") || input.consume("Numpad7")) {
      this.game.dev.freezeGuards = !this.game.dev.freezeGuards;
      this.note = this.game.dev.freezeGuards ? "guards frozen" : "guards live";
    }
    if (input.consume("Digit8") || input.consume("Numpad8")) {
      this.game.kill();
      this.note = "die";
    }
    if (input.consume("Digit9") || input.consume("Numpad9")) {
      this.game.bootLevel(this.game.levelId);
      this.game.status = "play";
      this.note = "reset";
    }

    this.pollAt -= dt;
    if (this.pollAt <= 0) {
      this.pollAt = 0.8;
      await this.reloadQuiet();
    }
  }

  async reload() {
    try {
      await this.catalog.reload(this.game.levelId);
      this.game.bootLevel(this.game.levelId);
      this.game.status = "play";
      this.note = "reloaded " + this.game.levelId;
    } catch (err) {
      this.note = "reload fail: " + err.message;
    }
  }

  async reloadQuiet() {
    try {
      const res = await fetch(`/levels/${this.game.levelId}.json?t=${Date.now()}`);
      const text = await res.text();
      if (!this.lastJson) {
        this.lastJson = text;
        return;
      }
      if (text !== this.lastJson) {
        this.lastJson = text;
        await this.reload();
      }
    } catch {
      // keep playing the in-memory layout
    }
  }
}
