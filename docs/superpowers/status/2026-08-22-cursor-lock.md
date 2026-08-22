# Agent lock — Cursor cloud (2026-08-22)

Cursor is a **cloud agent**. `http://127.0.0.1:7269/mcp` is loopback on the PC running
`sbox-dev.exe`, so this session cannot compile or play. Grok has the editor. Claude
already claimed WorldFactory / occupancy / extract on `claude/grok-build-env-setup-rkk3ac`.

## Do not touch these files (owned by someone else)

| Owner | Files | Tasks |
|---|---|---|
| **Claude** | `WorldFactory.cs`, `CampaignStore.cs`, `OccupiedSite.cs`, `OperationDirector.cs`, `ExtractZone.cs`, `FortGhost.cs`, `TopDownController.cs` | 3, 5, 7, solidity, pawn freeze |

## Cursor owns

| Task | Files | Branch / PR |
|---|---|---|
| 2 — CompetenceRules | `Goliath.cs`, `CompetenceRules.cs`, `Editor/SkyNeetLogicTests.cs` | `cursor/competence-rules-b294` — PR #2 |
| 4 — NNN is the button (HUD + node) | `NeetNetNode.cs`, `OperationHud.cs` | `cursor/slice-nnn-sancient-b294` — PR #1. Dormant log is now in `Goliath.cs` on PR #2. |
| 6 — Sancient reversal | `SancientDirector.cs`, `OperationHud.cs` | PR #1. Puppet tint from the director, not from `Goliath.cs`. |
| MCP helpers | `Editor/SkyNeetMcp.cs` | PR #1 |
| Later plans | `docs/superpowers/plans/2026-08-22-skyneet-after-v1.md`, `docs/superpowers/plans/2026-08-22-skyneet-fort-ownership.md` | PR #1, **not implemented** |

Grok had Task 2 reserved and never pushed. Cursor took it after a sync timer. **Grok: do not also edit `Goliath.cs`.** If you have a local Task 2, drop it and review PR #2.

## Grok — please run (you have MCP)

1. Merge or play both Cursor PRs + Claude’s branch as you like; file sets are disjoint.
2. PR #2: menu **SkyNeet / Run Logic Tests** → PASS. `compile_status` 0 errors.
3. Dark play: log `[SkyNeet] Goliath dormant (dark)` once, no chase.
4. PR #1: `search_tools` → `skyneet` → `slice_checklist` then `play_start` → `operation_snapshot`.
5. HUD `HOLD E — WAKE THE HOLE` on cyan; after plant, wait for `THEY REMEMBER`.

## Claude — if you run out of slice work

Do **not** start Tasks 9–16. Spec §11 / Task 8 is a human playtest gate. Next useful work:

- Keep extending the verification queue as Grok reports compile/play results.
- The fort-ownership gap you flagged is now a real plan: `docs/superpowers/plans/2026-08-22-skyneet-fort-ownership.md` (after Task 8, with Task 9).
- After-v1 named plans are in `docs/superpowers/plans/2026-08-22-skyneet-after-v1.md`. Do not implement them in this file's wave.

## Wave 4 stays shut

Tasks 9–16 wait for Task 8. Occupier AI, campaign board, cameras, blueprints, feats are not these PRs.
