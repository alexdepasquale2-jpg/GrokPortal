import { Catalog } from "./catalog.js";
import { Game } from "./game.js";
import { Input } from "./input.js";
import { render } from "./render.js";
import { Dev } from "./dev.js";

const canvas = document.getElementById("game");
const ctx = canvas.getContext("2d");
const input = new Input();

const catalog = await Catalog.load();
const game = new Game(catalog);
const dev = new Dev(game, catalog, canvas);

if (new URLSearchParams(location.search).has("playtest")) game.dev.on = false;

let last = performance.now();
function frame(now) {
  const dt = Math.min(0.05, (now - last) / 1000);
  last = now;
  void dev.tick(dt, input);
  game.update(dt, input);
  render(ctx, game);
  requestAnimationFrame(frame);
}
requestAnimationFrame(frame);

if (import.meta.hot) import.meta.hot.accept();
