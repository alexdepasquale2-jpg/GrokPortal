# Grok — compile this one branch

Claude merged both Cursor PRs into `claude/grok-build-env-setup-rkk3ac` at **1dd7cd6** (zero conflicts). WorldFactory now reads `SliceTints`. Cursor branches stay up for bisection.

**Checkout:** `claude/grok-build-env-setup-rkk3ac`  
**Not:** three separate PRs, unless something fails and you need to bisect.

## Compile / play

1. Open `skyneet-survivors/` in s&box. `compile_status` → 0 errors.
2. If `Editor/` is red, delete in this order and recompile:
   1. `Editor/SkyNeetPlayMcp.cs` (loses `operation_snapshot`)
   2. `Editor/SkyNeetMcp.cs` **and** `Editor/SkyNeetLogicMcp.cs` (loses MCP checklists)
   3. Menu **SkyNeet / Run Logic Tests** must still work (`SkyNeetLogicTests.cs`)
3. MCP: `skyneet_logic.run_logic_tests` → PASS, or the menu.
4. `play_start` on `scenes/cavern.scene`. Full queues:
   - `docs/superpowers/status/2026-08-22-verification-queue.md` (Claude)
   - `docs/superpowers/status/2026-08-22-cursor-verification.md` (Cursor)

## Do not

- Do not start Tasks 9–16 (Wave 4) until Task 8 human playtest.
- Do not edit `Goliath.cs` on a new branch; Task 2 is already in the merge.
- Do not re-introduce tint literals on `WorldFactory` or `SancientDirector`.
