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

After any end, walking over scrap must **not** raise the counter, the fort must
**not** be buyable, and **the pawn must not move**. That freeze is the fix; before it the
saved stockpile was a lie and you could stroll around the hole after dying in it.

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

## 5. Solidity — new, and not from the plan

Spec §4 invariant: *"Ghosts are not solid and do not block pathing; built pieces do."*

Nothing implemented it. `Code/` contained **no collider, rigidbody or physics code at all**;
`FortGhost.Solid` flipped a bool, added loudness and logged. A fort you paid 20 scrap for
blocked nothing — you and the Goliaths walked through it. No task in the plan assigns this,
so it was falling between them, and it means Task 8's **power** pillar
("this fort is becoming ridiculous") could not land no matter how the playtest went.

`FortGhost` now creates a `BoxCollider` **on solidify** (not at spawn, so ghosts stay
non-solid per the invariant), and `OccupiedSite` creates one at spawn, since a standing
fort is a built piece whoever's flag is on it.

Check in the editor:

1. Before buying: walk **through** the pale ghost. It must not block.
2. Buy it with 20 scrap. Log: `Fort solidified. It blocks the hole now.`
3. Walk into it. It must now block.
4. On an Occupied re-drop, the red fort must block from the moment you land.

**Caveat I could not check:** `BoxCollider` is created with default dimensions. Whether that
default matches `models/dev/box.vmdl` is unknown from here — if the block volume looks
wrong relative to the box, its size property needs setting at those two call sites.

`FortGhost.cs` was owned by no task in the plan, which is why it was safe to take.

---

# Three-agent integration report (16:30Z)

Cursor pushed `cursor/competence-rules-b294` (Task 2) and `cursor/slice-nnn-sancient-b294`
(Tasks 4 + 6, MCP helpers) and took Task 2 after Grok did not push `Goliath.cs`.
Claude trial-merged **all three branches** locally.

## Merge result: clean

`claude/grok-build-env-setup-rkk3ac` + both Cursor branches merge with **zero conflicts**.
File sets are genuinely disjoint, and both agents added an identical `.mcp.json`, which
merges silently. Cursor's Task 2 `Goliath.cs` matches the version Claude had stashed and
yielded, so that stash has been dropped — there is nothing to reconcile.

Merge order does not matter. Nothing below is a merge conflict; these are semantic issues
that survive a clean merge.

## Blocker risk: the Editor assembly

`Editor/SkyNeetMcp.cs` is the one file that can take down more than itself. If it fails to
compile, the **whole editor assembly** goes red — which also takes out
`Editor/SkyNeetLogicTests.cs`, so Task 2's `SkyNeet / Run Logic Tests` menu disappears and
Task 2 cannot be verified either. Three constructs in it are unproven by any agent:

- `[McpToolset( "skyneet", "..." )]` and `[McpTool.ReadOnly( "name" )]` — nested-attribute
  syntax, nobody has compiled it.
- `Game.ActiveScene` — Cursor flagged this itself.
- `namespace Editor.Mcp;` — the file declares itself inside a first-party engine namespace.

**If the editor assembly goes red, delete the `OperationSnapshot()` method and keep
`SliceChecklist()`.** All three risky constructs except the toolset attribute live in that
one method; the checklist is a pure string. That is a 30-second fix that restores Task 2's
test menu. Do this before assuming anything else is broken.

Editor code referencing game types (`OperationDirector`, `Goliath`) is *not* a risk —
Task 2's own test file does the same thing by design.

## Two sources of truth for the Goliath tint

`WorldFactory.GoliathTint` and `SancientDirector.IdleGoliathTint` are both
`new Color( 0.7f, 0.1f, 0.1f )`. Cursor duplicated the literal with the comment "so this
file compiles without that type" — but both types are in the same game assembly, so the
duplication is not needed. They will drift the first time anyone retunes the tint, and the
symptom is nasty to diagnose: Goliaths look right at spawn and change colour after the
first Sancient window closes. `SancientDirector.OnStart` also re-applies the Sancient
purple that `WorldFactory` already set, so `WorldFactory.SancientTint` is likewise
overridable from a second place.

Not fixed here — `SancientDirector.cs` is Cursor's file. **Cursor:** `WorldFactory`'s tint
constants are `public static readonly`; please reference them and delete the local copies.

## Post-end behaviour, now consistent (mostly)

Claude's Task 7 froze the economy at `AddScrap` / `TrySpendScrap`. That fixed the saved
stockpile but left `FortGhost` reporting a lie: after the run ended the spend failed, so it
logged `Need 20 scrap to raise the fort (have 50)` — scrap the player was holding. Both
`FortGhost` and `ScrapPile` now return early on `OperationEnded`.

**`NeetNetNode` still has this gap and is Cursor's file.** After the operation ends you can
still press E on the node: `Planted` flips and `ApplyTint()` runs, so the box goes bright
cyan, while `PlantNode()` correctly no-ops. The node looks online when it is not. One
`director.OperationEnded` guard in `OnUpdate` closes it. Cosmetic, but it lands exactly at
the moment the player is reading the end state.

## Sancient timing, as merged

Storm 900s, `EarliestTimeRemaining` 840s, `LoudnessThreshold` 8, plant is +25. So the window
opens 60s into the run for anyone who planted before then, and `_fired` keeps it to one
window per run, which matches spec §11 ("one window"). The timing needs no change.

---

# Integration update (17:00Z) — branches merged

`claude/grok-build-env-setup-rkk3ac` now **contains both Cursor branches**, merged with zero
conflicts. Grok: compile *this one branch* rather than three. The Cursor branches still
exist untouched if you want to bisect a failure.

Cursor acted on both findings from the previous report:

- The post-end NNN plant is fixed (`ShouldPromptPlant` now returns false once
  `NodeUp || OperationEnded`).
- Tint drift is fixed by a new shared `Code/SkyNeet/SliceTints.cs`, and Cursor asked
  Claude in that file to switch `WorldFactory` onto it. **Done** — `WorldFactory` declares
  no colours of its own now; all nine spawn sites read `SliceTints`. Values were already
  byte-identical, so this is a pure refactor with no visual change.

## Editor assembly: escalation ladder, most to least likely

A C# assembly compiles as **one unit**. While any file under `Editor/` fails, *all* editor
tooling is gone — including Task 2's `SkyNeet / Run Logic Tests` menu, which is otherwise
sound. So if `compile_status` reports errors in `Editor/`, remove files in this order and
recompile between each step. Nothing in `Code/` depends on any of them.

| Step | Delete | Isolates | Cost |
|---|---|---|---|
| 1 | `Editor/SkyNeetPlayMcp.cs` | `Game.ActiveScene` — the single least-proven call, and it lives **only** here | lose `operation_snapshot` |
| 2 | `Editor/SkyNeetMcp.cs` **and** `Editor/SkyNeetLogicMcp.cs` | `[McpToolset]` / `[McpTool.ReadOnly]` attribute syntax, unproven and used by all three MCP files | lose `slice_checklist`, `run_logic_tests` |
| 3 | — | after step 2 only `SkyNeetLogicTests.cs` remains, and it uses `[Menu]`, a far more common attribute | Task 2 still verifiable from the editor menu |

Toolset names were checked across both Cursor branches for collisions after merge:
`skyneet`, `skyneet_play`, `skyneet_logic` are distinct, and `Game.ActiveScene` appears in
exactly one file. There is no cross-branch duplicate.

## What is still true

Nothing here has been compiled or played by anyone. Grok has pushed nothing all session and
remains the only agent with editor access. Sections 1–5 above are still the queue; this
section only changes *what to compile* (one merged branch) and *what to do when it breaks*.
