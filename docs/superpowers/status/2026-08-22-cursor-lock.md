# Agent lock — Cursor cloud (2026-08-22)

Cursor is a **cloud agent**. `http://127.0.0.1:7269/mcp` is loopback on the PC running
`sbox-dev.exe`, so this session cannot compile or play. Grok has the editor. Claude
already claimed WorldFactory / occupancy / extract on `claude/grok-build-env-setup-rkk3ac`.

## Do not touch these files (owned by someone else)

| Owner | Files | Tasks |
|---|---|---|
| **Grok** | `Goliath.cs`, `CompetenceRules.cs`, `Editor/SkyNeetLogicTests.cs` | Task 2 |
| **Claude** | `WorldFactory.cs`, `CampaignStore.cs`, `OccupiedSite.cs`, `OperationDirector.cs`, `ExtractZone.cs`, `FortGhost.cs`, `TopDownController.cs` | 3, 5, 7, solidity, pawn freeze |

## Cursor owns this turn

| Task | Files | State |
|---|---|---|
| 4 — NNN is the button (HUD + node) | `NeetNetNode.cs`, `OperationHud.cs` | **This PR.** Goliath already idles when `!NodeUp`. Dormant **log** still belongs in `Goliath.cs` (Grok). Do not duplicate it. |
| 6 — Sancient reversal | `SancientDirector.cs`, `OperationHud.cs` | **This PR.** Puppet tint is applied from the director via `ModelRenderer`, not by editing `Goliath.cs`. |
| MCP helpers | `Editor/SkyNeetMcp.cs` | `slice_checklist`, `operation_snapshot` |
| Later plans | `docs/superpowers/plans/2026-08-22-skyneet-after-v1.md`, `docs/superpowers/plans/2026-08-22-skyneet-fort-ownership.md` | Written, **not implemented** |

Branch: `cursor/slice-nnn-sancient-b294`  
Base: `feat/skyneet-survivors-design`  
Does **not** merge Claude's branch. File sets are disjoint so both PRs can land.

## Grok — please run (you have MCP)

1. `search_tools` → `skyneet` after this compiles.
2. `slice_checklist` then `play_start` → `operation_snapshot` → `play_stop`.
3. Task 2 still yours: `CompetenceRules` + editor tests + wire `Goliath` to them. Add the one-shot log `[SkyNeet] Goliath dormant (dark)` there.
4. `EditorUtility.DisplayDialog( title, message, okay, icon, parent )` — the plan's two-arg call may not compile. Use at least `( "SkyNeet tests", "PASS", "OK" )`.

## Claude — if you run out of slice work

Do **not** start Tasks 9–16. Spec §11 / Task 8 is a human playtest gate. Next useful work:

- Keep extending the verification queue as Grok reports compile/play results.
- The fort-ownership gap you flagged is now a real plan: `docs/superpowers/plans/2026-08-22-skyneet-fort-ownership.md` (after Task 8, with Task 9).
- After-v1 named plans are in `docs/superpowers/plans/2026-08-22-skyneet-after-v1.md`. Do not implement them in this file's wave.

## Wave 4 stays shut

Tasks 9–16 wait for Task 8. Occupier AI, campaign board, cameras, blueprints, feats are not this PR.
