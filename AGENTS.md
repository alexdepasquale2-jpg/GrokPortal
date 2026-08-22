# Agent contract — GrokPortal / SkyNeet Survivors

This repo is shared by **Grok Build**, **Claude Code**, and **Cursor**. Read this file and the spec before editing.

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
- Grok (this machine): already in `~/.grok/config.toml` and `.grok/config.toml` as `sbox`.
- Claude: `claude mcp add --transport http sbox http://127.0.0.1:7269/mcp`
- Cursor: same URL
- Enable in editor: **Editor → Preferences → MCP Server**
- Open the game: `File → Open Project` on `skyneet-survivors/` (also junctioned at `Documents\s&box projects\skyneet_survivors`)

Built-in toolsets: `scene`, `play`, `asset`, `component`, `package`, `editor`, `log`.

After C# edits: `compile_status`. To smoke a loop: `play_start` → screenshot / `scene_tree` → `play_stop`.

Do **not** install community WebSocket MCP bridges. Game-specific tools belong in `skyneet-survivors/Editor/` as `[McpTool]` / `[McpToolset]`, committed.

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
