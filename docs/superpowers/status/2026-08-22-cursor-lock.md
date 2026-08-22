# Agent lock — Cursor cloud (2026-08-22)

Cursor is a **cloud agent**. `http://127.0.0.1:7269/mcp` is loopback on the PC running
`sbox-dev.exe`. Grok has the editor.

**17:00Z:** Claude merged both Cursor PRs into `claude/grok-build-env-setup-rkk3ac` (zero
conflicts) and pointed `WorldFactory` at `SliceTints`. Grok should compile **that one
branch**. See `docs/superpowers/status/2026-08-22-grok-compile-this.md`.

Cursor PRs #1 and #2 stay open for bisection. Do not rewrite files Claude already merged.

## File owners (still)

| Owner | Files |
|---|---|
| Claude | `WorldFactory`, `CampaignStore`, `OccupiedSite`, `OperationDirector`, `ExtractZone`, `FortGhost`, `TopDownController`, `ScrapPile` |
| Cursor | `Goliath`, `CompetenceRules`, `SkyNeetLogicTests`, `NeetNetNode`, `OperationHud`, `SancientDirector`, `SliceTints`, `Editor/SkyNeet*.cs` |

## Wave 4 stays shut

Tasks 9–16 wait for Task 8. Playbook: `docs/superpowers/plans/2026-08-22-skyneet-task8-playtest.md`.
Editor menu **SkyNeet / Print Task 8 Script** (no MCP attributes).
