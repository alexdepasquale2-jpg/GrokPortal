# GrokPortal

Shared workspace for **SkyNeet Survivors**, an s&box game, and for agents (Grok Build, Claude Code, Cursor) working on it.

## SkyNeet Survivors

Next-gen Vampire Survivors × survival building × tower defense in an **underground-archipelago** war. You are **Neetmon Gould**. Killer robots still have Goliath bodies; they have forgotten how to use them — until a **Sancient** reminds them.

**Design spec (source of truth):** [`docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md`](docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md)

**Agent contract:** [`AGENTS.md`](AGENTS.md)

## Engine

- [s&box docs](https://sbox.game/dev/doc/)
- [s&box API](https://sbox.game/api/)
- [Editor MCP server](https://sbox.game/dev/doc/editor/mcp-server) — `http://127.0.0.1:7269/mcp`
- Local editor (this machine): `C:\Program Files (x86)\Steam\steamapps\common\sbox\sbox-dev.exe`

The s&box project lives in [`skyneet-survivors/`](skyneet-survivors/). Open **that** folder in the editor.

**First playable** is the vertical slice in spec §11 (one 15-minute hole, top-down, scrap, NNN as the button). Not three cameras.

## Documents

| File | What it is |
|---|---|
| [`docs/VISION.md`](docs/VISION.md) | **The whole game at full scale.** Start here for what this is building toward |
| [`docs/SCOPE-LEDGER.md`](docs/SCOPE-LEDGER.md) | Vision vs. what actually exists. The measuring stick |
| `docs/superpowers/specs/2026-08-22-…-design.md` | The design spec — slice-first, the build contract |
| `docs/superpowers/plans/…-v1-original.md` | **Frozen** original v1 plan. Never edited |
| `docs/superpowers/plans/2026-08-22-skyneet-survivors.md` | Living plan, ticked as tasks land |

## Branch

`main` — default and the trunk everything lands on.

`feat/skyneet-survivors-design` is the historical design/implementation branch and is kept
at the same commit; it is no longer where work should start.
