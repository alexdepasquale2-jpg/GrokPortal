import { dist } from "./physics.js";

export const REACH = 40;
export const BEHIND_DOT = -0.32;
export const GRAB_SPEED = 82;
export const NOISE_RADIUS = 220;

export const COMBO_HITS = 4;
export const COMBO_STEP = 0.13;
export const COMBO_END = COMBO_STEP * COMBO_HITS + 0.16;

export const BODY_REACH = 34;
export const HAUL_SPEED = 104;
export const HAUL_SLACK = 20;

export function facingOf(g) {
  if (Math.hypot(g.vx || 0, g.vy || 0) > 8) return Math.atan2(g.vy, g.vx);
  return g.facing ?? 0;
}

export function isBehind(player, guard) {
  const ang = facingOf(guard);
  const dx = player.x - guard.x;
  const dy = player.y - guard.y;
  const len = Math.hypot(dx, dy) || 1;
  const dot = (dx / len) * Math.cos(ang) + (dy / len) * Math.sin(ang);
  return dot <= BEHIND_DOT;
}

export function inReach(player, guard) {
  return dist(player, guard) <= REACH;
}

export function canStealth(player, guard) {
  if (!guard || guard.down || guard.held) return false;
  if (guard.alert > 0.05 && !(guard.stun > 0)) return false;
  return inReach(player, guard) && isBehind(player, guard);
}

export function canBrawl(player, guard) {
  if (!guard || guard.down || guard.held) return false;
  if (guard.alert <= 0.05 && !(guard.stun > 0)) return false;
  return inReach(player, guard);
}

export function bestStealth(player, guards) {
  let best = null;
  let bestD = REACH + 1;
  for (const g of guards) {
    if (!canStealth(player, g)) continue;
    const d = dist(player, g);
    if (d < bestD) {
      best = g;
      bestD = d;
    }
  }
  return best;
}

export function bestBrawl(player, guards) {
  let best = null;
  let bestD = REACH + 1;
  for (const g of guards) {
    if (!canBrawl(player, g)) continue;
    const d = dist(player, g);
    if (d < bestD) {
      best = g;
      bestD = d;
    }
  }
  return best;
}

export function canHaul(player, guard) {
  if (!guard || !guard.down) return false;
  return dist(player, guard) <= BODY_REACH;
}

export function bestBody(player, guards) {
  let best = null;
  let bestD = BODY_REACH + 1;
  for (const g of guards) {
    if (!canHaul(player, g)) continue;
    const d = dist(player, g);
    if (d < bestD) {
      best = g;
      bestD = d;
    }
  }
  return best;
}

export function pinBehind(player, guard) {
  const ang = facingOf(guard);
  player.x = guard.x - Math.cos(ang) * 16;
  player.y = guard.y - Math.sin(ang) * 16;
  player.vx = 0;
  player.vy = 0;
}
