# Agent contract — GrokPortal / SkyNeet Survivors

Development is **Claude Code, solo**, as of 2026-08-22. Grok Build (hit its limits) and
Cursor (disabled) are no longer working this repo. The multi-agent file-ownership protocol
they used is retired — the lock file is deleted and the dispatch map in the plan is history,
not instruction. Read this file and the spec before editing.

## Source of truth

1. `docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md` — game design
2. This file — how to work
3. `docs/superpowers/plans/2026-08-22-skyneet-survivors.md` — task-by-task implementation plan (dispatch one agent per task; Wave 0 is serial)

Do not invent systems that contradict the spec. If the spec must change, amend it in git.

## Engine

- **s&box**, not Unity, not Godot.
- Editor: `C:\Program Files (x86)\Steam\steamapps\common\sbox` (`sbox-dev.exe`).
- Docs: https://sbox.game/dev/doc/
- API: https://sbox.game/api/
- MCP: https://sbox.game/dev/doc/editor/mcp-server
- Game project folder: `skyneet-survivors/` (open **that** folder in the s&box editor).
- Ident: `local.skyneet.survivors`
- Template: **Game – Player Controller** (FPS / TPS / top-down examples already exist).
- C# components, `GameResource`, prefabs, Razor UI, host-authoritative networking.
- Tick 50. Move/build/AI in `OnFixedUpdate`.

If an API is not in the s&box docs/API or `TypeLibrary`, it does not exist.

## MCP (first-party only)

The editor hosts MCP at **Editor → Preferences → MCP Server**.

- URL: `http://127.0.0.1:7269/mcp` (loopback only). The **s&box editor must be running** or this is dead.
- Config: `.mcp.json` at the repo root (also `.grok/config.toml`, now unused).
- Enable in editor: **Editor → Preferences → MCP Server**
- **A cloud agent cannot reach this.** Loopback means the machine running `sbox-dev.exe`.
  A Claude Code session on claude.ai/code is in a container and gets connection-refused;
  `sbox.game` is blocked by its egress proxy too, so the API docs are unreachable from
  there. Compile and play verification has to come from a session on the PC, or from Alex.
- Open the game: `File → Open Project` on `skyneet-survivors/` (also junctioned at `Documents\s&box projects\skyneet_survivors`)

Built-in toolsets: `scene`, `play`, `asset`, `component`, `package`, `editor`, `log`.

After C# edits: `compile_status`. To smoke a loop: `play_start` → screenshot / `scene_tree` → `play_stop`.

Do **not** install community WebSocket MCP bridges.

**Custom MCP tools: `[McpTool]` and `[McpToolset]` do not exist in this s&box build.**
Three files using them were written on 2026-08-22 and produced the only red build of the
session — nine `CS0246`s, `McpToolsetAttribute` / `McpToolset` / `McpTool` not found. They
were deleted. Do not re-add them from this file's earlier advice, from the s&box docs, or
from a web search: confirm the real attribute in the editor's API browser or `TypeLibrary`
**first**, because you cannot compile-check it from a cloud session.

`[Menu( "Editor", "SkyNeet/..." )]` **is** proven and compiles — `Editor/SkyNeetLogicTests.cs`
and `Editor/SkyNeetTask8Menu.cs` both use it. Prefer a menu entry over an MCP tool.

`Editor/` compiles as **one assembly**: any single bad file there takes out every editor
menu, including the Task 2 test harness. Add editor code one file at a time and recompile.

## Product spine (do not “simplify away”)

- PvE, one body: **Neetmon Gould**
- Causal chain: scavenge → build → **NNN** → wake machines → noise → Sancient/occupiers → leave or hold → **the field you lose is still someone’s**
- Fun pillars: greed, dread, power, panic, consequence
- **First build is the vertical slice (spec §11):** one cavern, 15 min, top-down only, scrap only, one module, one weapon, one robot, one NNN, one Sancient, one extract, one `owner` flag
- Do **not** start with three cameras, dual-res, blueprint sandbox, flavor mechanics, extra currencies, or Nobot diplomacy
- Full vision stays in the spec; it is not the current implementation target

## Spec vs assumptions

Design **invariants** (one pawn, host sim, dark until NNN, inverse competence, Sancient keeps lethality, occupancy persists) outrank **implementation assumptions** (`CameraComponent`, `NavMeshArea`, MCP URL, `FileSystem.Data`). If the editor fights an assumption, keep the invariant.

## Git

- Branch from the current design/implementation branch, not random local-only work.
- Commit durable decisions.
- Follow s&box `.gitignore` (no `.csproj`, `.sln`, compiled `*_c`, `bin/`, `obj/`).
