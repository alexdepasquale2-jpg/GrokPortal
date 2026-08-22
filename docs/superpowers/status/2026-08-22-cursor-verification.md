# Verification queue — Cursor → Grok

Cursor cloud cannot reach `http://127.0.0.1:7269/mcp` (connection refused on this VM).
C# below is written against the spec and the s&box public API pages, never compiled here.

## New / changed code

| File | Why |
|---|---|
| `Code/SkyNeet/NeetNetNode.cs` | `ShouldPromptPlant`, cyan→bright tint on plant |
| `Code/SkyNeet/OperationHud.cs` | `\nHOLD E — WAKE THE HOLE`, `SANCIENT  THEY REMEMBER` |
| `Code/SkyNeet/SancientDirector.cs` | `LoudnessThreshold = 8`, `EarliestTimeRemaining = 14*60`, yellow puppet tint, revert to `(0.7, 0.1, 0.1)` |
| `Editor/SkyNeetMcp.cs` | `[McpToolset("skyneet")]` `slice_checklist`, `operation_snapshot` |
| `.mcp.json` | Same HTTP MCP URL Claude already added on their branch. Identical on purpose. |

## Compile risks (Cursor could not prove)

- `Color.Yellow` — `Color.Red` is already on Claude's branch; same family.
- `Game.ActiveScene` in the editor MCP tool — if the name differs, drop `operation_snapshot` and keep `slice_checklist` (pure string).
- `[McpTool.ReadOnly( "name" )]` and `[McpToolset( "skyneet", "..." )]` from https://sbox.game/dev/doc/editor/mcp-server
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

If the window never opens, inspect `SancientDirector` properties on the purple box: threshold 8, earliest remaining 840. Do not "fix" by raising competence in `Goliath.cs` — that file is Grok's.
