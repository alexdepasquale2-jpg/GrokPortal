function roundRect(ctx, x, y, w, h, r) {
  const rr = Math.min(r, w / 2, h / 2);
  ctx.beginPath();
  ctx.moveTo(x + rr, y);
  ctx.arcTo(x + w, y, x + w, y + h, rr);
  ctx.arcTo(x + w, y + h, x, y + h, rr);
  ctx.arcTo(x, y + h, x, y, rr);
  ctx.arcTo(x, y, x + w, y, rr);
  ctx.closePath();
}

export function eventToWorld(e, canvas, game) {
  const r = canvas.getBoundingClientRect();
  const cw = r.width;
  const ch = r.height;
  const scale = Math.min(cw / game.def.w, ch / game.def.h);
  const ox = (cw - game.def.w * scale) / 2;
  const oy = (ch - game.def.h * scale) / 2;
  return {
    x: Math.round((e.clientX - r.left - ox) / scale),
    y: Math.round((e.clientY - r.top - oy) / scale),
  };
}

export function render(ctx, game, canvas) {
  const { def, player } = game;
  const dpr = Math.min(2, window.devicePixelRatio || 1);
  const cw = Math.max(1, canvas.clientWidth);
  const ch = Math.max(1, canvas.clientHeight);
  const bw = Math.max(1, Math.floor(cw * dpr));
  const bh = Math.max(1, Math.floor(ch * dpr));
  if (canvas.width !== bw || canvas.height !== bh) {
    canvas.width = bw;
    canvas.height = bh;
  }

  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.fillStyle = "#05060a";
  ctx.fillRect(0, 0, cw, ch);

  const scale = Math.min(cw / def.w, ch / def.h);
  const ox = (cw - def.w * scale) / 2;
  const oy = (ch - def.h * scale) / 2;
  const shakeX = game.shake ? (Math.random() - 0.5) * game.shake : 0;
  const shakeY = game.shake ? (Math.random() - 0.5) * game.shake : 0;
  ctx.setTransform(dpr * scale, 0, 0, dpr * scale, dpr * (ox + shakeX), dpr * (oy + shakeY));

  drawFloor(ctx, def);
  for (const wall of def.walls) drawWall(ctx, wall);
  drawExit(ctx, game.exit, player.hasRelic, game.time);
  for (const can of game.oilCans) {
    if (!can.taken) drawOil(ctx, can, game.time);
  }
  if (game.relic && !game.relic.taken) drawRelic(ctx, game.relic, game.time);
  for (const g of game.guards) drawGuard(ctx, g, game.time);
  if (game.prompt?.guard) drawPromptRing(ctx, game.prompt);
  if (game.melee) drawGrabLink(ctx, player, game.melee.guard, game.melee.phase);
  drawPlayer(ctx, player, game.time, game.flare);

  drawDarkness(ctx, def, player, game);

  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  drawHud(ctx, game, cw, ch);
}

function drawFloor(ctx, def) {
  ctx.fillStyle = "#0b0d16";
  ctx.fillRect(0, 0, def.w, def.h);
  ctx.strokeStyle = "rgba(80,90,130,0.07)";
  ctx.lineWidth = 1;
  for (let x = 0; x < def.w; x += 40) {
    ctx.beginPath();
    ctx.moveTo(x, 0);
    ctx.lineTo(x, def.h);
    ctx.stroke();
  }
  for (let y = 0; y < def.h; y += 40) {
    ctx.beginPath();
    ctx.moveTo(0, y);
    ctx.lineTo(def.w, y);
    ctx.stroke();
  }
}

function drawWall(ctx, wall) {
  ctx.fillStyle = "#1c2133";
  roundRect(ctx, wall.x, wall.y, wall.w, wall.h, 3);
  ctx.fill();
  ctx.strokeStyle = "#2c334c";
  ctx.lineWidth = 2;
  ctx.stroke();
}

function drawExit(ctx, exit, open, t) {
  ctx.save();
  ctx.translate(exit.x, exit.y);
  ctx.strokeStyle = open ? "#5dffb0" : "#2e6b52";
  ctx.fillStyle = open ? "rgba(61,255,154,0.15)" : "rgba(46,107,82,0.08)";
  ctx.lineWidth = 3;
  ctx.beginPath();
  ctx.arc(0, 0, exit.r, 0, Math.PI * 2);
  ctx.fill();
  ctx.stroke();
  ctx.globalAlpha = open ? 0.9 : 0.4;
  ctx.fillStyle = open ? "#9dffd0" : "#5a7a6a";
  ctx.font = "11px Segoe UI";
  ctx.textAlign = "center";
  ctx.fillText(open ? "EXIT" : "LOCKED", 0, 4);
  if (open) {
    ctx.strokeStyle = `rgba(93,255,176,${0.4 + Math.sin(t * 6) * 0.25})`;
    ctx.beginPath();
    ctx.arc(0, 0, exit.r + 6, 0, Math.PI * 2);
    ctx.stroke();
  }
  ctx.restore();
}

function drawOil(ctx, can, t) {
  ctx.save();
  ctx.translate(can.x, can.y);
  ctx.rotate(Math.sin(t * 3) * 0.1);
  ctx.fillStyle = "#ff8a3d";
  ctx.beginPath();
  ctx.moveTo(0, -9);
  ctx.lineTo(7, 4);
  ctx.lineTo(-7, 4);
  ctx.closePath();
  ctx.fill();
  ctx.fillStyle = "#ffd0a0";
  ctx.fillRect(-2, -2, 4, 5);
  ctx.restore();
}

function drawRelic(ctx, relic, t) {
  ctx.save();
  ctx.translate(relic.x, relic.y);
  ctx.rotate(t * 1.2);
  ctx.shadowColor = "#ffd36a";
  ctx.shadowBlur = 18;
  ctx.fillStyle = "#ffd36a";
  ctx.beginPath();
  ctx.moveTo(0, -12);
  ctx.lineTo(10, 0);
  ctx.lineTo(0, 12);
  ctx.lineTo(-10, 0);
  ctx.closePath();
  ctx.fill();
  ctx.fillStyle = "#fff6d2";
  ctx.beginPath();
  ctx.arc(0, 0, 4, 0, Math.PI * 2);
  ctx.fill();
  ctx.restore();
}

function drawGuard(ctx, g, t) {
  ctx.save();
  ctx.translate(g.x, g.y);
  const ang = g.facing ?? Math.atan2(g.vy, g.vx);
  ctx.rotate(ang);
  if (g.down) {
    ctx.globalAlpha = 1;
    ctx.rotate(0.9);
    ctx.fillStyle = "#9aa3b8";
    ctx.strokeStyle = "#e8dcc4";
    ctx.lineWidth = 1.5;
    ctx.beginPath();
    ctx.moveTo(14, 0);
    ctx.lineTo(-11, 9);
    ctx.lineTo(-6, 0);
    ctx.lineTo(-11, -9);
    ctx.closePath();
    ctx.fill();
    ctx.stroke();
    ctx.restore();
    return;
  }
  ctx.fillStyle = g.held ? "#c4a574" : g.stun > 0 ? "#8aa" : g.alert > 0 ? "#6ad0e8" : "#3d6d7a";
  ctx.beginPath();
  ctx.moveTo(14, 0);
  ctx.lineTo(-10, 9);
  ctx.lineTo(-6, 0);
  ctx.lineTo(-10, -9);
  ctx.closePath();
  ctx.fill();
  ctx.fillStyle = g.held ? "#ffb347" : g.alert > 0 ? "#ff5a5a" : "#7ee7ff";
  ctx.beginPath();
  ctx.arc(4, 0, 3, 0, Math.PI * 2);
  ctx.fill();
  if (g.alert > 0 && !g.held) {
    ctx.strokeStyle = "rgba(255,80,80,0.35)";
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(0, 0, 22 + Math.sin(t * 10) * 3, 0, Math.PI * 2);
    ctx.stroke();
  }
  ctx.restore();
}

const LOUD = new Set(["brawl", "combo"]);

function drawPromptRing(ctx, prompt) {
  const g = prompt.guard;
  ctx.save();
  ctx.translate(g.x, g.y);
  ctx.strokeStyle = LOUD.has(prompt.kind) ? "rgba(255,90,90,0.8)" : "rgba(255,179,71,0.85)";
  ctx.lineWidth = 2;
  ctx.beginPath();
  ctx.arc(0, 0, 22 + (prompt.kind === "finish" ? 4 : 0), 0, Math.PI * 2);
  ctx.stroke();
  ctx.restore();
}

function drawGrabLink(ctx, player, guard, phase) {
  if (!guard) return;
  ctx.save();
  ctx.strokeStyle = phase === "combo" ? "rgba(255,90,90,0.6)" : "rgba(255,179,71,0.55)";
  ctx.lineWidth = phase === "haul" ? 2 : 3;
  if (phase === "haul") ctx.setLineDash([5, 4]);
  ctx.beginPath();
  ctx.moveTo(player.x, player.y);
  ctx.lineTo(guard.x, guard.y);
  ctx.stroke();
  ctx.restore();
}

function drawPlayer(ctx, p, t, flare) {
  ctx.save();
  ctx.translate(p.x, p.y);
  if (p.hasRelic) {
    ctx.shadowColor = "#ffd36a";
    ctx.shadowBlur = 16;
  }
  ctx.fillStyle = "#f4c98a";
  ctx.beginPath();
  ctx.arc(0, 0, p.r, 0, Math.PI * 2);
  ctx.fill();
  if (p.lantern) {
    const flick = 3 + Math.sin(t * 18) * 1.5;
    ctx.fillStyle = "#ffb347";
    ctx.beginPath();
    ctx.arc(0, -p.r - 2, flick, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = "#fff4c8";
    ctx.beginPath();
    ctx.arc(0, -p.r - 2, flick * 0.4, 0, Math.PI * 2);
    ctx.fill();
  }
  if (flare > 0) {
    ctx.strokeStyle = `rgba(255,220,140,${flare * 2})`;
    ctx.lineWidth = 4;
    ctx.beginPath();
    ctx.arc(0, 0, 28 + (0.35 - flare) * 80, 0, Math.PI * 2);
    ctx.stroke();
  }
  ctx.restore();
}

function drawDarkness(ctx, def, player, game) {
  if (game.status !== "play" && game.status !== "dead") return;
  const vis = game.visionRadius() + (game.flare > 0 ? 90 : 0);
  ctx.save();
  ctx.fillStyle = "rgba(2,3,8,0.88)";
  ctx.beginPath();
  ctx.rect(0, 0, def.w, def.h);
  ctx.arc(player.x, player.y, vis, 0, Math.PI * 2, true);
  ctx.fill("evenodd");
  ctx.restore();
}

const PROMPTS = {
  stealth: ["BEHIND THEM  —  C  choke", "BEHIND THEM  —  TAKE DOWN"],
  finish: ["C  knock out    stick to drag", "CHOKE  —  TAP TO DROP"],
  brawl: ["C  —  one combo, loud", "COMBO  —  TAP, loud"],
  combo: ["COMBO", "COMBO"],
  body: ["C  drag the body", "DRAG  —  TAP"],
  haul: ["hauling    C to let go", "HAULING  —  TAP TO DROP"],
};

function promptLabel(kind, touch) {
  const pair = PROMPTS[kind];
  if (!pair) return "";
  return touch ? pair[1] : pair[0];
}

function drawHud(ctx, game, w, h) {
  ctx.save();
  ctx.fillStyle = "#e8dcc4";

  if (game.status === "title") {
    ctx.fillStyle = "rgba(4,5,10,0.55)";
    ctx.fillRect(0, 0, w, h);
    ctx.textAlign = "center";
    ctx.fillStyle = "#ffb347";
    const titleSize = Math.max(42, Math.min(72, w * 0.16));
    ctx.font = `700 ${titleSize}px Segoe UI`;
    ctx.fillText("WICK", w / 2, h / 2 - 36);
    ctx.fillStyle = "#e8dcc4";
    ctx.font = `${Math.max(14, w * 0.032)}px Segoe UI`;
    ctx.fillText("Steal the last light. Hide in the dark. Walk it out.", w / 2, h / 2 + 8);
    ctx.font = `${Math.max(13, w * 0.028)}px Segoe UI`;
    ctx.fillStyle = "#9aa3b8";
    ctx.fillText(
      game.touch
        ? "TAP TO START     stick · lantern · dash · flare · take down"
        : "WASD move   SHIFT dash   E lantern   F flare   C choke / combo / drag   SPACE start",
      w / 2,
      h / 2 + 48
    );
    ctx.restore();
    return;
  }

  if (game.status === "dead" || game.status === "win" || game.status === "run_complete") {
    ctx.fillStyle = "rgba(4,5,10,0.62)";
    ctx.fillRect(0, 0, w, h);
    ctx.textAlign = "center";
    ctx.font = `700 ${Math.max(28, Math.min(42, w * 0.08))}px Segoe UI`;
    ctx.fillStyle = game.status === "dead" ? "#ff6b6b" : "#5dffb0";
    const title =
      game.status === "dead" ? "THEY SAW THE FLAME" : "THE LIGHT LEFT WITH YOU";
    ctx.fillText(title, w / 2, h / 2 - 10);
    ctx.font = "16px Segoe UI";
    ctx.fillStyle = "#e8dcc4";
    ctx.fillText(game.touch ? "TAP TO GO AGAIN" : "SPACE / R  —  again", w / 2, h / 2 + 32);
    ctx.restore();
    return;
  }

  const p = game.player;
  const oilW = Math.min(180, w * 0.4);
  const oilH = 10;
  const pad = 16;
  ctx.textAlign = "left";
  ctx.font = "13px Segoe UI";
  ctx.fillStyle = "#9aa3b8";
  ctx.fillText(game.def.name.toUpperCase(), pad, pad + 12);
  ctx.fillStyle = "#3a2a1a";
  ctx.fillRect(pad, pad + 22, oilW, oilH);
  ctx.fillStyle = "#ff8a3d";
  ctx.fillRect(pad, pad + 22, oilW * (p.oil / Math.max(p.maxOil, 0.01)), oilH);
  ctx.fillStyle = "#e8dcc4";
  ctx.fillText(p.oil <= 0 ? "NO OIL" : p.lantern ? "LANTERN ON" : "HIDDEN", pad, pad + 50);
  ctx.fillText(p.hasRelic ? "RELIC  take it to the door" : "find the relic", pad, pad + 68);
  if (game.prompt) {
    const label = promptLabel(game.prompt.kind, game.touch);
    ctx.fillStyle = LOUD.has(game.prompt.kind) ? "#ff6b6b" : "#ffb347";
    ctx.font = "700 14px Segoe UI";
    ctx.fillText(label, pad, pad + 90);
  }
  if (game.flash > 0) {
    ctx.fillStyle = `rgba(255,220,180,${Math.min(0.35, game.flash * 2)})`;
    ctx.fillRect(0, 0, w, h);
  }

  if (game.dev.on) {
    ctx.font = "12px ui-monospace, Consolas, monospace";
    ctx.fillStyle = "#7ee7ff";
    const hint = game.touch
      ? "DEV  0 behind  7 freeze"
      : "DEV  0 behind  1 oil  2 relic  3 extract  4 reload  5 next  6 inf oil  7 freeze  8 die  9 reset   click=coords";
    ctx.fillText(hint, pad, h - 16);
  }
  ctx.restore();
}
