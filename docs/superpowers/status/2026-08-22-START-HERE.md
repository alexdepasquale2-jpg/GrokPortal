# START HERE — SkyNeet slice handoff (2026-08-22, end of agent session)

**Branch:** `main` — the default and the trunk. Everything is on it, and every other branch
reports 0 commits missing from it. No open PRs.
`feat/skyneet-survivors-design` is kept at the same commit as the historical design branch.

**Status in one line:** Tasks 1–7 are code-complete and **not one line has ever been
compiled or played**. Every remaining step needs the s&box editor on your PC.

Three agents worked this: Claude and Cursor (both cloud, no editor access), and Grok (had
the editor, pushed nothing, hit its limits). The earlier status docs address Grok directly
and are now historical — `2026-08-22-verification-queue.md`,
`2026-08-22-cursor-verification.md`, `2026-08-22-grok-compile-this.md`. Their technical
content is still accurate; their "Grok: please run…" framing is void. This file supersedes
them for sequencing. `2026-08-22-final-review.md` holds the detailed findings.

## What exists

18 components in `Code/SkyNeet/`, 5 files in `Editor/`. The full causal chain is written:
scavenge → build → NNN → wake machines → noise → Sancient → leave or die → occupied field.

| Task | What landed |
|---|---|
| 1 | Ident `local.skyneet_survivors`, one `.sbproj`, no `HostSync` / `.razor` / `Panel.Add.Label` |
| 2 | `CompetenceRules` + logic tests (editor menu **and** an MCP tool) |
| 3 | `WorldFactory` spawns the cavern; six distinct tints via `SliceTints` |
| 4 | NNN gates the world; Goliaths dormant while dark; HUD dread prompt |
| 5 | `CampaignStore` persistence; lost sites come back as a red `OccupiedSite` holding your scrap |
| 6 | Sancient window: competence reversal, yellow tint, reverts on close |
| 7 | Extract hold feedback; three clean end reasons; everything freezes on end |
| — | Built pieces block (spec §4): colliders stop the player, `FortBlock` stops Goliaths |

## Tomorrow, in this order

1. **Open the editor** on `skyneet-survivors/` (or the junction at
   `Documents\s&box projects\skyneet_survivors`) — **not** the repo root.
2. **`compile_status`.** This is the only command that matters. Everything below is
   guesswork until it runs.
3. **If `Editor/` is red**, delete in this order, recompiling between each. An assembly
   compiles as one unit, so one bad file takes out all editor tooling including Task 2's
   test menu. Nothing in `Code/` depends on any of these.
   1. `Editor/SkyNeetPlayMcp.cs` — isolates `Game.ActiveScene`, the least-proven call, and
      it lives only here. Costs `operation_snapshot`.
   2. `Editor/SkyNeetMcp.cs` + `Editor/SkyNeetLogicMcp.cs` + `Editor/SkyNeetTask8Menu.cs` —
      isolates `[McpToolset]` / `[McpTool.ReadOnly]` syntax, unproven and shared by all of
      them. Costs `slice_checklist`, `run_logic_tests`, the Task 8 menu.
   3. `SkyNeetLogicTests.cs` uses only `[Menu]` and should survive both steps, so Task 2
      stays verifiable from the editor menu.
4. **If `Code/` is red**, the three unproven things are all in spawn/tint code:
   `new Color( r, g, b )` and `new Color( r, g, b, a )`, `Color.Red` / `Color.Yellow`, and
   `ModelRenderer.Tint`. `Color.Black` / `Color.White` are already proven by the booting
   HUD, so named colours are the fallback if the float constructor is wrong.
   All of it is in `SliceTints.cs` — one file, nine lines to change.
5. **`SkyNeet / Run Logic Tests`** → PASS.
6. **Play the three ends** — extract, death, clock. Then the occupancy roundtrip: die with
   NNN up, stop, replay, and confirm the red fort is standing and the HUD says
   `OWNER Occupied`.
7. **Task 8 gate.** Wave 4 (Tasks 9–16) stays shut until you have judged the five pillars.

## Two known gaps, deliberately not invented around

**`BoxCollider` dimensions are defaults.** Whether that matches `models/dev/box.vmdl` could
not be checked without the editor. If the fort's block volume looks wrong relative to the
box, set the size at the two call sites in `FortGhost.cs` and `OccupiedSite.cs`.
`FortBlock.DefaultRadius` (48u) is the Goliath's separate horizontal check and may want to
match whatever you pick.

**`CampaignSave` cannot say "the standing fort is mine".** It records that a fort stands and
who owns the site, but not both together. So losing a site and later retaking it by
extracting in the dark gives you a fresh ghost to re-buy rather than your fort back. Closing
it needs a field on `CampaignSave`, which is Task 9 territory — there is a written plan at
`docs/superpowers/plans/2026-08-22-skyneet-fort-ownership.md`.

## One directory question for the editor to settle

`Assets/scenes/cavern.scene_d` is a 48-byte binary sidecar next to the 182KB
`cavern.scene`. It came in with the original scaffold commit, contains no strings beyond a
GUID-like blob, and looks like a generated Source 2 dependency manifest. The official s&box
`.gitignore` in this repo covers compiled `*.*_c` but not `_d`, so it is tracked.

**Check on first editor load:** if the editor regenerates or rewrites it, add `*.scene_d`
to `.gitignore` so it stops producing noise diffs. If the scene will not load without it,
leave it tracked. Not deleted here — that could not be tested from a cloud session.

## The honest caveat

Roughly 20 commits of C# were written by two agents that could not compile a single line of
it. The logic was reviewed hard — against the spec, against `cavern.scene`, and against the
merged tree, which is where the last two real bugs were caught — but *reviewed* is not
*run*. Expect the first compile to find things. That is the cost of the setup, not a
surprise.
