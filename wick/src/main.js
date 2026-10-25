import { Catalog } from "./catalog.js";
import { Game } from "./game.js";
import { Input } from "./input.js";
import { render } from "./render.js";
import { Dev } from "./dev.js";
import { mountTouch } from "./touch.js";

const canvas = document.getElementById("game");
const ctx = canvas.getContext("2d");
const input = new Input();

async function boot() {
  const catalog = await Catalog.load();
  const game = new Game(catalog);
  game.touch = false;
  const dev = new Dev(game, catalog, canvas);
  mountTouch(input, game, canvas);

  const params = new URLSearchParams(location.search);
  if (params.has("playtest")) game.dev.on = false;
  if (import.meta.env?.PROD && !params.has("dev")) game.dev.on = false;

  let last = performance.now();
  function frame(now) {
    const dt = Math.min(0.05, (now - last) / 1000);
    last = now;
    void dev.tick(dt, input);
    game.update(dt, input);
    render(ctx, game, canvas);
    requestAnimationFrame(frame);
  }
  requestAnimationFrame(frame);
}

void boot();

if (import.meta.hot) import.meta.hot.accept();
