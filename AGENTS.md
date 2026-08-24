# Agent contract — GrokPortal

Two games live here. Use the one the user is asking for.

1. **Wick** (`wick/`) — browser game. This is the **rapid iterate** loop: an agent can run it, change it, and show a result in the same session. How-to: `wick/README.md`.
2. **SkyNeet Survivors** (`skyneet-survivors/`) — s&box. A cloud agent cannot compile or play it. Spec below.

If the user wants “add this, boom I can see it”, work in **Wick** unless they named SkyNeet.

Development of SkyNeet is **Claude Code, solo**, as of 2026-08-22. The old multi-agent file-ownership protocol is retired.

## Wick — snappy loop (browser)

```
cd wick && npm install && npm run dev
```

Open `http://127.0.0.1:5173`.

1. **Feature** — edit `wick/src/*.js`, save. Vite reloads.
2. **Level** — edit `wick/public/levels/{id}.json` or `npm run new-level -- id`. The game polls JSON (~1s) or press **4**.
3. **Jump** — DEV keys 1–9. `?playtest=1` turns them off.

Do not port Wick to s&box. It exists because a cloud session can play a browser game and cannot play s&box.

## Source of truth (SkyNeet)

1. `docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md` — game design
2. This file — how to work
3. `docs/superpowers/plans/2026-08-22-skyneet-survivors.md` — task-by-task implementation plan

Do not invent SkyNeet systems that contradict the SkyNeet spec. If that spec must change, amend it in git.

## Engine (SkyNeet)

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

**Custom MCP tools are possible, but `[McpToolset]` / `[McpTool.ReadOnly]` are the wrong
names.** Three files using that syntax were written on 2026-08-22 and produced the only red
build of the session — nine `CS0246`s, `McpToolsetAttribute` / `McpToolset` / `McpTool` not
found — and were deleted.

They are not impossible, though: Grok registered a working custom tool,
`skyneet_run_logic_tests`, and called it successfully twice (its own tools appear as
`mcp_*`). A working example therefore exists in Grok's unpushed checkout at
`C:\Users\Albert\GrokPortal` — **copy the attribute from there** rather than guessing.
Confirm against the editor's API browser or `TypeLibrary` before re-adding, because a cloud
session cannot compile-check it.

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
