# Task 8 — Slice playtest gate

> **Not Wave 4.** No campaign board, occupier AI, cameras, blueprints, or feats.
> Compile `claude/grok-build-env-setup-rkk3ac` first (`docs/superpowers/status/2026-08-22-grok-compile-this.md`).
> Code only if a **pillar fails**. File a fix task; do not start Tasks 9–16.

**Pillars:** greed, dread, power, panic, consequence.

Editor menu **SkyNeet / Print Task 8 Script** dumps this into the log if you do not want to flip back to git.

---

## Run A — dark extract (greed + leave)

1. `play_start`. Owner Neet (delete stale `skyneet_site.json` if a prior Occupied run is lying).
2. Walk orange scrap. Counter rises. That is greed.
3. Do **not** press E on cyan.
4. Walk the red Goliath. It must not chase. Log once: `Goliath dormant (dark)`.
5. Stand on green pad ~2s. Log `extracting …/2.0` then `ENDED extract`. OWNER Neet.
6. After ENDED: no scrap pickup, no fort buy, pawn frozen, cyan does not brighten on E.

**Pass:** you wanted more scrap and still chose to leave.  
**Fail:** Goliath moves in the dark, or extract happens without a hold, or the world keeps simulating after ENDED.

Screenshot: pad + HUD `ENDED extract`.

---

## Run B — wake the hole and die (dread, power, panic, consequence)

1. New play. Grab scrap to 20. HUD on cyan: `HOLD E — WAKE THE HOLE`.
2. E. Log `NNN online`. Dread line gone. Goliath weaves and **misses**.
3. E on pale ghost. Log fort solidified + blocks. Loudness jumps. That is power.
4. Wait until clock < 14:00 (or plant after 60s). Log `They remember.` HUD `THEY REMEMBER`. Goliath yellow and **hits**. That is panic.
5. ~25s later window closes, dark-red tint, misses return — or you die first. Either is fine.
6. Die or clock-out with NNN up. `ENDED death|clock`. OWNER Occupied. Log about leftover scrap.

**Pass:** the button felt like a mistake you chose; the fort mattered; the catalog waking was the same robot, not a new HP bar.  
**Fail:** Goliath was already accurate in the dark, or the Sancient is just a fatter box, or the fort does not block **you or the Goliath**. A wall that only stops the player is not power (Claude review e5800ee).

Screenshot: yellow Goliath + `THEY REMEMBER`, then the end line.

---

## Run C — someone else's field (consequence)

1. `play_stop` then `play_start` without deleting the save.
2. Log: `Owner from last run: Occupied` and occupied fort scrap line.
3. Red fort at `(0,-80,16)`, **no** buyable ghost, fort **blocks**.
4. HUD `OWNER Occupied`.

**Pass:** you came back to a hole that is not yours.  
**Fail:** a cheap ghost you can re-buy, or no red fort. Stale save from before `FortStands()` is not a bug — delete `skyneet_site.json` and redo Run B then C.

Screenshot: red fort + OWNER Occupied.

---

## Observe, do not "fix" mid-gate

| Thing | Why it is not a Task 8 rewrite |
|---|---|
| AutoNeedler ticks wrecks in the dark | Possible greed. Only file a task if it ruins dread. |
| Default `BoxCollider` size vs the box mesh | Claude flagged it. Tune only if you walk through a "solid" fort. |
| Editor MCP files fail compile | Delete per grok-compile ladder. Gameplay Code/ does not depend on them. |

---

## Gate write-up (required)

Five lines, one per pillar, in an empty commit on the merged branch:

```bash
git commit --allow-empty -m "test: slice playtest gate
greed: ...
dread: ...
power: ...
panic: ...
consequence: ..."
```

If a line is "missing", write a **fix task** under `docs/superpowers/plans/` and stop. Do not open Task 9.
