# Agent lock — Cursor cloud (2026-08-22)

Cursor is a **cloud agent**. `http://127.0.0.1:7269/mcp` is loopback on the PC running
`sbox-dev.exe`, so this session cannot compile or play. Grok has the editor.

Claude trial-merged all three branches: **zero conflicts**. `.mcp.json` is identical.

## Do not touch these files (owned by someone else)

| Owner | Files | Tasks |
|---|---|---|
| **Claude** | `WorldFactory.cs`, `CampaignStore.cs`, `OccupiedSite.cs`, `OperationDirector.cs`, `ExtractZone.cs`, `FortGhost.cs`, `TopDownController.cs`, `ScrapPile.cs` | 3, 5, 7, solidity, pawn/economy freeze |

## Cursor owns

| Task | Files | Branch / PR |
|---|---|---|
| 2 — CompetenceRules | `Goliath.cs`, `CompetenceRules.cs`, `Editor/SkyNeetLogicTests.cs` | PR #2 |
| 4 + 6 | `NeetNetNode.cs`, `OperationHud.cs`, `SancientDirector.cs`, `SliceTints.cs` | PR #1 |
| MCP | `Editor/SkyNeetMcp.cs` (checklist only), `Editor/SkyNeetPlayMcp.cs` (snapshot — **delete if editor assembly goes red**) | PR #1 |

**Claude:** please point `WorldFactory` tints at `SliceTints.*` (same names: `NodeTint`, `GoliathTint`, `SancientTint`, …) so spawn and puppet revert cannot drift. Do not edit `SancientDirector.cs`.

**Grok:** do not edit `Goliath.cs`. If `Editor/SkyNeetPlayMcp.cs` fails to compile, delete that file only — `SkyNeetMcp.cs` and Task 2's test menu stay.

## Wave 4 stays shut

Tasks 9–16 wait for Task 8. Occupier AI, campaign board, cameras, blueprints, feats are not these PRs.
