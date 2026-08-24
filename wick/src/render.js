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

export function render(ctx, game) {
  const { def, player } = game;
  const w = ctx.canvas.width;
  const h = ctx.canvas.height;

  ctx.setTransform(1, 0, 0, 1, 0, 0);
  ctx.fillStyle = "#05060a";
  ctx.fillRect(0, 0, w, h);

  const sx = w / def.w;
  const sy = h / def.h;
  const shakeX = game.shake ? (Math.random() - 0.5) * game.shake : 0;
  const shakeY = game.shake ? (Math.random() - 0.5) * game.shake : 0;
  ctx.setTransform(sx, 0, 0, sy, shakeX, shakeY);

  drawFloor(ctx, def);
  for (const wall of def.walls) drawWall(ctx, wall);
  drawExit(ctx, game.exit, player.hasRelic, game.time);
  for (const can of game.oilCans) {
    if (!can.taken) drawOil(ctx, can, game.time);
  }
  if (game.relic && !game.relic.taken) drawRelic(ctx, game.relic, game.time);
  for (const g of game.guards) drawGuard(ctx, g, game.time);
  drawPlayer(ctx, player, game.time, game.flare);

  drawDarkness(ctx, def, player, game);

  ctx.setTransform(1, 0, 0, 1, 0, 0);
  drawHud(ctx, game);
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
  const ang = Math.atan2(g.vy, g.vx);
  ctx.rotate(ang);
  ctx.fillStyle = g.stun > 0 ? "#8aa" : g.alert > 0 ? "#6ad0e8" : "#3d6d7a";
  ctx.beginPath();
  ctx.moveTo(14, 0);
  ctx.lineTo(-10, 9);
  ctx.lineTo(-6, 0);
  ctx.lineTo(-10, -9);
  ctx.closePath();
  ctx.fill();
  ctx.fillStyle = g.alert > 0 ? "#ff5a5a" : "#7ee7ff";
  ctx.beginPath();
  ctx.arc(4, 0, 3, 0, Math.PI * 2);
  ctx.fill();
  if (g.alert > 0) {
    ctx.strokeStyle = "rgba(255,80,80,0.35)";
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(0, 0, 22 + Math.sin(t * 10) * 3, 0, Math.PI * 2);
    ctx.stroke();
  }
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

function drawHud(ctx, game) {
  const w = ctx.canvas.width;
  const h = ctx.canvas.height;
  ctx.save();
  ctx.font = "16px Segoe UI";
  ctx.fillStyle = "#e8dcc4";

  if (game.status === "title") {
    ctx.fillStyle = "rgba(4,5,10,0.55)";
    ctx.fillRect(0, 0, w, h);
    ctx.textAlign = "center";
    ctx.fillStyle = "#ffb347";
    ctx.font = "700 72px Segoe UI";
    ctx.fillText("WICK", w / 2, h / 2 - 40);
    ctx.fillStyle = "#e8dcc4";
    ctx.font = "18px Segoe UI";
    ctx.fillText("Steal the last light. Hide in the dark. Walk it out.", w / 2, h / 2 + 8);
    ctx.font = "14px Segoe UI";
    ctx.fillStyle = "#9aa3b8";
    ctx.fillText("WASD move   SHIFT dash   E lantern   F flare   SPACE start", w / 2, h / 2 + 48);
    ctx.restore();
    return;
  }

  if (game.status === "dead" || game.status === "win" || game.status === "run_complete") {
    ctx.fillStyle = "rgba(4,5,10,0.62)";
    ctx.fillRect(0, 0, w, h);
    ctx.textAlign = "center";
    ctx.font = "700 42px Segoe UI";
    ctx.fillStyle = game.status === "dead" ? "#ff6b6b" : "#5dffb0";
    const title =
      game.status === "dead" ? "THEY SAW THE FLAME" : "THE LIGHT LEFT WITH YOU";
    ctx.fillText(title, w / 2, h / 2 - 10);
    ctx.font = "16px Segoe UI";
    ctx.fillStyle = "#e8dcc4";
    ctx.fillText("SPACE / R  —  again", w / 2, h / 2 + 32);
    ctx.restore();
    return;
  }

  const p = game.player;
  const oilW = 180;
  const oilH = 10;
  ctx.textAlign = "left";
  ctx.font = "12px Segoe UI";
  ctx.fillStyle = "#9aa3b8";
  ctx.fillText(game.def.name.toUpperCase(), 24, 28);
  ctx.fillStyle = "#3a2a1a";
  ctx.fillRect(24, 38, oilW, oilH);
  ctx.fillStyle = "#ff8a3d";
  ctx.fillRect(24, 38, oilW * (p.oil / Math.max(p.maxOil, 0.01)), oilH);
  ctx.fillStyle = "#e8dcc4";
  ctx.fillText(p.lantern ? "LANTERN ON" : "HIDDEN", 24, 64);
  ctx.fillText(p.hasRelic ? "RELIC  take it to the door" : "find the relic", 24, 82);

  if (game.dev.on) {
    ctx.font = "12px ui-monospace, Consolas, monospace";
    ctx.fillStyle = "#7ee7ff";
    ctx.fillText(
      "DEV  1 oil  2 relic  3 extract  4 reload  5 next  6 inf oil  7 freeze  8 die  9 reset   click=coords",
      24,
      h - 18
    );
  }
  ctx.restore();
}
