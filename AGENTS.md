# Agent contract — GrokPortal / SkyNeet Survivors

This repo is shared by **Grok Build**, **Claude Code**, and **Cursor**. Read this file and the spec before editing.

## Source of truth

1. `docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md` — game design
2. This file — how to work
3. `docs/superpowers/plans/` — implementation plans when they exist

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

- URL: `http://127.0.0.1:7269/mcp` (loopback only)
- Claude: `claude mcp add --transport http sbox http://127.0.0.1:7269/mcp`
- Grok: HTTP MCP in `~/.grok/config.toml`
- Cursor: same URL

Built-in toolsets: `scene`, `play`, `asset`, `component`, `package`, `editor`, `log`.

After C# edits: `compile_status`. To smoke a loop: `play_start` → screenshot / `scene_tree` → `play_stop`.

Do **not** install community WebSocket MCP bridges. Game-specific tools belong in `skyneet-survivors/Editor/` as `[McpTool]` / `[McpToolset]`, committed.

## Product spine (do not “simplify away”)

- PvE, one body: **Neetmon Gould**
- 15/30/45/60 operations; backpack resets; campaign graph persists
- Live cameras; view-linked combat (presets later)
- Dual-res building + blueprint ghosts
- Dark until **NeetNetNode**; inept Goliaths; Sancients puppet; Nobots occupy
- v1 is a thin cut of those systems (see spec §10), not a different game

## Git

- Branch from the current design/implementation branch, not random local-only work.
- Commit durable decisions.
- Follow s&box `.gitignore` (no `.csproj`, `.sln`, compiled `*_c`, `bin/`, `obj/`).
