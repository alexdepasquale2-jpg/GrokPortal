# WICK

A new game. Steal the last light, hide from what hunts the glow, walk it out.

This is the **rapid iterate** title: a browser game so a cloud agent can run it, change it, and show you the result in the same session. SkyNeet Survivors (s&box) stays in `skyneet-survivors/` and is unchanged.

```
cd wick
npm install
npm run dev
```

Open `http://127.0.0.1:5173` on this machine, or the **Network** URL Vite prints on your phone (same WiFi).

## Phone

The game is built for a phone. Rotate or stay portrait — the well letterboxes, HUD stays readable.

- **Tap** to start / go again
- **Left stick** move
- **LANTERN** hide or see
- **DASH** burst (costs oil)
- **FLARE** stun (costs oil)
- **TAKE DOWN** appears when you are in range: behind an unaware hunter to grab, CHOKE to drop them, STRIKE if they already saw you

Add to Home Screen for a fullscreen app. Same WiFi as the computer running `npm run dev`, then open the Network address Vite prints (not 127.0.0.1).

Desktop: WASD, Shift, E, F, Space. Append `?touch=1` to force the on-screen stick.

## Loop (this is the whole point)

| You want | You do | You see |
|---|---|---|
| Change a rule | Edit `src/game.js`, save | Vite hot-reloads. Refresh if the sim is mid-run. |
| Move a guard / relic | Edit `public/levels/{id}.json`, save | ~1s later the room restamps, or press **4** |
| New room | `npm run new-level -- my_id` | It is in the run. Play, or press **5** |
| Skip the stealth | DEV keys **1–9** (cyan line) | oil, relic, extract, reload, next, inf oil, freeze, die, reset |
| Honest playtest | `http://127.0.0.1:5173/?playtest=1` | DEV keys off |

Click the canvas in DEV to print world coordinates. Paste those into a JSON file.

## Play

- **WASD** move
- **SHIFT** dash (costs oil)
- **E** / **Space** lantern on/off
- **F** flare (stun, costs oil)
- **C** / **Ctrl** takedown — behind an unaware hunter: grab, drag with the stick, tap again to choke them out. Face to face after they spot you: mash C to win a noisy fight, or they take you.
- **R** restart after death

Lantern on: you see, they see you. Lantern off: you are a silhouette. No oil → lantern dies. Grab the gold relic, the door unlocks, leave. Three rooms is a run.

## Files

```
wick/
  public/levels/     JSON rooms + index.json (the run order)
  src/game.js        rules
  src/melee.js       rear choke / hand-to-hand
  src/render.js      look
  src/dev.js         keys 1-9 + JSON poll
  scripts/new-level.mjs
```
