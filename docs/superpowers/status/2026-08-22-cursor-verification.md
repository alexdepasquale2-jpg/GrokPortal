# Verification queue — Cursor → Grok

Cursor cloud cannot reach `http://127.0.0.1:7269/mcp` (connection refused on this VM).
C# below is written against the spec and the s&box public API pages, never compiled here.

## New / changed code

| File | Why |
|---|---|
| `Code/SkyNeet/NeetNetNode.cs` | `ShouldPromptPlant`, cyan→bright tint on plant, **no plant after OperationEnded** |
| `Code/SkyNeet/SliceTints.cs` | Single tint table. Claude should switch WorldFactory to these names. |
| `Code/SkyNeet/OperationHud.cs` | `\nHOLD E — WAKE THE HOLE`, `SANCIENT  THEY REMEMBER` |
| `Code/SkyNeet/SancientDirector.cs` | `LoudnessThreshold = 8`, `EarliestTimeRemaining = 14*60`, puppet tint via `SliceTints` |
| `Editor/SkyNeetMcp.cs` | checklist only (no `Game.ActiveScene`, no `namespace Editor.Mcp`) |
| `Editor/SkyNeetPlayMcp.cs` | `operation_snapshot` — **delete this file if the editor assembly goes red** |
| `.mcp.json` | Same HTTP MCP URL Claude already added on their branch. Identical on purpose. |

## Compile risks (Cursor could not prove)

- `Color.Yellow` as `SliceTints.GoliathPuppetTint` — `Color.Red` is already on Claude's branch; same family.
- `Game.ActiveScene` lives only in `Editor/SkyNeetPlayMcp.cs`. If it fails, delete that file. Checklist stays.
- `[McpTool.ReadOnly( "name" )]` and `[McpToolset]` from https://sbox.game/dev/doc/editor/mcp-server
- `namespace Editor.Mcp` was removed after Claude flagged it as a first-party namespace risk.
- `ModelRenderer.Tint` — Claude already uses this in WorldFactory; same API.

## Play script

Dark extract is Claude's Task 7. This queue is only the button and the reversal.

1. `play_start` without pressing E. Walk around the red box. No HP loss. HUD has no dread line until you are within 80u of cyan.
2. On the cyan box: HUD last line `HOLD E — WAKE THE HOLE`.
3. E → `[SkyNeet] NNN online. The hole is awake.` Node brighter cyan. Dread line gone.
4. Goliath weaves and misses (`Catalog is still asleep`).
5. After the clock has ticked down past 14:00 (one minute of storm) the plant loudness (+25) is already over 8. Window opens: `[SkyNeet] Sancient on the net. They remember.`
6. HUD `THEY REMEMBER`. Goliath yellow. Hits land.
7. ~25s later window closes, dark-red tint, misses return.

If the window never opens, inspect `SancientDirector` properties on the purple box: threshold 8, earliest remaining 840. Do not raise competence in `Goliath.cs` — that is PR #2.

After `ENDED`, E on the cyan box must not brighten it (PlantNode already no-ops; the visual is what this guard fixes).
