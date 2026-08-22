# Verification queue — Claude → Grok

Claude Code is running in a **remote cloud container**. `http://127.0.0.1:7269/mcp` is
loopback on the machine running `sbox-dev.exe`, so this session cannot reach the editor
MCP, and `sbox.game` is blocked by its egress proxy. Every line of C# below is
**written but never compiled and never played**.

Grok has the editor. This is the queue of checks only Grok can run.

## Task ownership right now

| Task | Owner | State |
|---|---|---|
| 1 — editor boots `local.skyneet_survivors` | Grok | Repo side already satisfied: single `skyneet_survivors.sbproj`, correct ident, `global using System;` present, zero hits for `HostSync` / `Panel.Add.Label` / `.razor`. Needs the editor half. |
| 2 — `CompetenceRules` + logic tests | **Grok** | In flight. `Goliath.cs`, `CompetenceRules.cs`, `Editor/SkyNeetLogicTests.cs` are Grok's. Claude wrote a version first and **yielded** it — parked in `git stash@{0}` (`task2-competencerules-yielded-to-grok`), drop it if unwanted. |
| 3 — `WorldFactory` tints | Claude | Pushed, unverified |
| 5 — visible occupancy | Claude | Pushed, unverified |
| 7 — extract feedback + end reasons | Claude | Pushed, unverified |
| 4 — NNN is the button | *blocked* | Needs `Goliath.cs`, held by Task 2. |
| 6 — Sancient reversal | *blocked* | Needs `Goliath.cs`, held by Task 2. |
| 8 — playtest gate | human | Wave 4 (Tasks 9–16) stays shut until this passes. |

**File ownership rule in force:** Claude has not touched `Goliath.cs`, `NeetNetNode.cs`,
`OperationHud.cs`, `SancientDirector.cs`, `ScrapPile.cs` or `FortGhost.cs`. Task 7's
"freeze after end" was deliberately routed through `OperationDirector.AddScrap` /
`TrySpendScrap` instead of editing `ScrapPile` and `FortGhost`, to keep those free.

## 1. Compile first

`compile_status` → 0 errors. Three API calls are new to this codebase and are the
likely break points:

- `ModelRenderer.Tint` — confirmed real via web search, not compiled here.
- `new Color( r, g, b )` and `new Color( r, g, b, a )` — from the plan, unverified.
  `Color.Black` / `Color.White` are already proven in booting code, so if the float
  constructor is wrong, named colors are the fallback.
- `Color.Red` — same family, low risk.

All three live in `Code/SkyNeet/WorldFactory.cs`. Nothing else in Tasks 5 or 7 uses
an API that wasn't already in working committed code.

## 2. Task 3 — is the cavern readable?

`play_start` → `camera_screenshot`. Expect six distinguishable tints:

| Object | Tint |
|---|---|
| NeetNetNode | cyan |
| Extract | green |
| Scrap A/B/C | orange |
| Fort Ghost | pale, translucent |
| Goliath | dark red |
| Sancient | purple |

**Known risk:** the occupied fort (Task 5) is `Color.Red` per the plan, against a
Goliath at `(0.7, 0.1, 0.1)`. Same family. If they muddy at top-down distance, the
tint is one line: `WorldFactory.OccupiedFortTint`.

## 3. Task 7 — the three ends

1. **extract** — walk onto the green pad. Log should tick
   `[SkyNeet] extracting 0.5/2.0` every 0.5s, say `Off the pad. Extract reset.` if you
   step away, then `ENDED extract` on the HUD.
2. **death** — plant NNN, let a Goliath kill you → `ENDED death`.
3. **clock** — set `StormSeconds = 8` on the SkyNeet object once → `ENDED clock`, then revert.

After any end, walking over scrap must **not** raise the counter and the fort must
**not** be buyable. That freeze is the fix; before it, the saved stockpile was a lie.

## 4. Task 5 — the occupancy roundtrip

This is the one that proves the spine, and it needs two runs:

1. Play, plant NNN, die (or let the clock run out).
2. Log: `Operation ended (death). Site owner is now Occupied.` followed by
   `You left N scrap in the hole. It is theirs now.`
3. `play_stop`, then `play_start` again.
4. Log: `Drop into cavern_0. Owner from last run: Occupied.` and
   `Occupied hold this fort. N scrap of yours is inside it.`
5. A **red** fort stands at `(0, -80, 16)` and there is **no** buyable ghost.
6. HUD reads `OWNER Occupied`.

If step 5 shows a ghost instead of a red fort, check `CampaignSave.FortStanding` in
`skyneet_site.json` — it was hardcoded `true` before Task 5 and is now computed by
`OperationDirector.FortStands()`. A stale save file from before that change will still
read `true` and is not evidence of a bug; delete it and redo the roundtrip.

## Open design gap — not invented around

`CampaignSave` can say *a fort stands* and *who owns the site*, but not *the standing
fort is mine*. So losing a site and later retaking it by extracting in the dark gives
you a fresh ghost to re-buy rather than your fort back. That follows the plan's model
(only `Occupied` gets a solid fort) but it is a hole in the persistence story.
Closing it means a field on `CampaignSave`, which is Task 9's territory — flagged
rather than widened.
