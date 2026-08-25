export function clamp(v, a, b) {
  return Math.max(a, Math.min(b, v));
}

export function dist(a, b) {
  const dx = a.x - b.x;
  const dy = a.y - b.y;
  return Math.hypot(dx, dy);
}

export function circleHits(a, b) {
  const r = (a.r || 0) + (b.r || 0);
  return dist(a, b) < r;
}

function closestPointOnAabb(px, py, b) {
  return {
    x: clamp(px, b.x, b.x + b.w),
    y: clamp(py, b.y, b.y + b.h),
  };
}

export function circleHitsAabb(c, b) {
  const p = closestPointOnAabb(c.x, c.y, b);
  const dx = c.x - p.x;
  const dy = c.y - p.y;
  return dx * dx + dy * dy < c.r * c.r;
}

export function separateCircleAabb(c, b) {
  const inside = c.x > b.x && c.x < b.x + b.w && c.y > b.y && c.y < b.y + b.h;
  if (inside) {
    const left = c.x - b.x;
    const right = b.x + b.w - c.x;
    const top = c.y - b.y;
    const bottom = b.y + b.h - c.y;
    const m = Math.min(left, right, top, bottom);
    if (m === left) c.x = b.x - c.r;
    else if (m === right) c.x = b.x + b.w + c.r;
    else if (m === top) c.y = b.y - c.r;
    else c.y = b.y + b.h + c.r;
    return;
  }
  const p = closestPointOnAabb(c.x, c.y, b);
  let dx = c.x - p.x;
  let dy = c.y - p.y;
  const d2 = dx * dx + dy * dy;
  if (d2 >= c.r * c.r || d2 === 0) return;
  const d = Math.sqrt(d2);
  const push = (c.r - d) / d;
  c.x += dx * push;
  c.y += dy * push;
}

export function moveAndSlide(body, walls, dt) {
  body.x += body.vx * dt;
  body.y += body.vy * dt;
  for (let i = 0; i < 3; i++) {
    for (const wall of walls) separateCircleAabb(body, wall);
  }
}
