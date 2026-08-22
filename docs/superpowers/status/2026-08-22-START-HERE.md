# START HERE — SkyNeet slice handoff (2026-08-22, end of agent session)

**Branch:** `main` — the default and the trunk. Everything is on it, and every other branch
reports 0 commits missing from it. No open PRs.
`feat/skyneet-survivors-design` is kept at the same commit as the historical design branch.

**Status in one line:** Tasks 1–7 are code-complete and **the editor compiles clean**.
Nothing has been *played* yet. The remaining steps are the playtest, and they need hands on
the editor.

Three agents worked this: Claude and Cursor (both cloud, no editor access), and Grok (had
the editor, pushed nothing, hit its limits). **Grok and Cursor are now disabled; development
is Claude Code solo.** The file-ownership protocol they shared is retired. The earlier status docs address Grok directly
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

## FIRST COMPILE RESULT (resolved)

The first `compile_status` returned 9 errors, all `CS0246` and all in `Editor/`:
`McpToolsetAttribute` / `McpToolset` / `McpTool` could not be found. That is **ladder step 2**
below — the attribute syntax — not step 1. `Game.ActiveScene` was never reached, because the
attributes fail before any method body is resolved.

**Resolved by deleting the three MCP files** (`SkyNeetMcp.cs`, `SkyNeetPlayMcp.cs`,
`SkyNeetLogicMcp.cs`). No `McpTool` reference remains in the repo.

Two corrections to the ladder as it was written:

- `SkyNeetTask8Menu.cs` was listed in step 2. It should not have been — it uses `[Menu]`,
  not the MCP attributes, and threw no errors. It survives, and so does
  `SkyNeetLogicTests.cs`. Both `SkyNeet/` menu entries still work.
- **`Code/` compiled clean.** Zero errors across all 18 game components. That clears every
  API this session could not verify: `ModelRenderer.Tint`, `new Color( r, g, b )` and the
  4-arg form, `Color.Red` / `Color.Yellow`, `BoxCollider`, `Components.Get/Create`,
  `GetAllComponents<T>`, `IsValid()`, `WithZ`. The step-4 fallback below is not needed.

What was lost: the `skyneet` / `skyneet_play` / `skyneet_logic` MCP toolsets. Nothing in the
game depends on them, and both editor menus cover what they were for — `SkyNeet / Run Logic
Tests` for Task 2, `SkyNeet / Print Task 8 Script` for the playtest. If the MCP tools are
wanted back later, the real attribute name has to come from the editor's API browser or
`TypeLibrary`; no agent in this session could reach the s&box docs to confirm it.

## Tomorrow, in this order

1. ~~**Open the editor**~~ — done.
2. ~~**`compile_status`**~~ — done, and green after step 3 below.
3. ~~**Editor red**~~ — resolved. The three `[McpToolset]` files were deleted; see the
   compile-result section above. Both `[Menu]` files survived.
4. ~~**`Code/` red**~~ — never happened. `Code/` compiled clean on the first try.
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
it. The first compile found exactly one class of problem — the speculative MCP attributes —
and all 18 game components in `Code/` built clean. So the *compiler* is satisfied.

Nothing has been **played**. Compiling proves the API calls exist; it proves nothing about
whether Goliaths path sensibly, whether the fort blocks what it should, whether the tints
read at top-down distance, or whether any of the five pillars land. That is what Task 8 is
for, and it is still ahead.
