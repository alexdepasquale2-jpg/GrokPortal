# SkyNeet Survivors — Game Design

**Date:** 2026-08-22  
**Status:** Approved in design review (sections 1–7). Awaiting implementation plan.  
**Engine:** s&box (Source 2 + .NET 10, Steam editor)  
**Repo:** https://github.com/alexdepasquale2-jpg/GrokPortal  
**Local game folder (when scaffolded):** `skyneet-survivors/` in this repo, also intended as `C:\Users\Albert\SkyNeetSurvivors` if opened as a standalone s&box project.  
**Ident:** `local.skyneet.survivors`  
**Protagonist:** Neetmon Gould (one Neet, on foot)

This document is the source of truth. Grok Build, Claude Code, and Cursor must follow it. Chat decisions after this date belong in a spec amendment commit, not in a private transcript.

---

## 1. What it is

SkyNeet Survivors is a **PvE operation game** with a **living underground war**.

It is Vampire Survivors combat, Rust/tower-defense building, and a RimWorld-like economy, compressed into **player-chosen 15 / 30 / 45 / 60 minute operations**, remembered on a **campaign graph**.

You play **Neetmon Gould**, one body. You gather, craft, and raise forts from **blueprints**. Weapons and views are **view-linked** at first (top-down auto-fire, third-person mixed, first-person manual) and later unlock **presets** that equalize the three cameras. The swarm is **inept Goliath robots** — catalog-lethal, behavior-stupid — until a **Sancient** puppets them. Machines cannot think underground until someone plants a **NeetNetNode** (neutrino antenna onto **NNN**, the Neet Node Net). **Nobots** (breakaway machines, no neutrino tech) commit **roborrism** to steal that tech. Your backpack and in-run base **die with the operation**. The **archipelago save** keeps turf, pipelines, and who is running the field. Playstyle is not a playlist: militant can become diplomatic or logistics **by what you do**. Feats feed **one tech tree**. Leaderboards score operations. **Pyron Chrome** civilization and arming Nobots are the same verbs at a later scale.

**Architecture (Approach 1):** two layers, **operation-first**. The GDD describes the full war. The playable spine is the operation. The campaign map is memory. Endgame is later strata of the same systems (nodes, factions, pipelines), not a new game.

---

## 2. Key decisions

| Decision | Choice | Rationale |
|---|---|---|
| Session vs world | Two-layer campaign | Honors both “runs reset” and “RimWorld persistence” |
| Persistence | Backpack/base/in-run tech reset. Campaign graph + account feats/tree persist | 15–60 min sessions stay clean; war still remembers |
| Account persistence | Head starts, curves, formations, perk trees, combos — not a kept base | Prebuilts can be scrapped/upgraded in the next drop |
| Avatar | One Neet: Neetmon Gould | Keeps the VS spine; campaign is still a faction sim |
| PvP | None in v1. Leaderboards only | PvE horde-hold is the spine |
| Camera | Live switch top-down / third / first; default steep top-down | Unique spice; s&box Player Controller template already has all three |
| Combat | View-linked hybrid first; late-tree parity + presets | Early views are actually different games; late game you author your own |
| Building | Dual-resolution (modules + pieces) + blueprint sandbox + live ghosts | New, not a Rust clone or a TD clone |
| Space | Underground-archipelago (graph of sites), not one oil field | Robot signal dies underground; NNN is the radio-on lever |
| Generation | Biome procedural, DF/Noita/RimWorld depth | Resources scarce until tech unlocks deeper strata |
| Factions | Neets (you), inept Robots, Sancients, Nobots | Inverse competence + hive spikes + third-party carnage |
| Networking | Host-authoritative Multiplayer even when solo | Co-op later is not a rewrite |
| Agent workflow | Git + first-party s&box MCP | Claude / Grok / Cursor share files and the live editor |

---

## 3. s&box environment (non-negotiable)

Editor install (this PC):

- `C:\Program Files (x86)\Steam\steamapps\common\sbox`
- Launch: `sbox-dev.exe` (or Steam **s&box**)
- Editor build observed: `26.08.19`
- Docs: https://sbox.game/dev/doc/
- API: https://sbox.game/api/
- MCP: https://sbox.game/dev/doc/editor/mcp-server

**Create the game from the Game – Player Controller template** (first-person, third-person, and top-down example scenes already ship). Do not start from Empty and reinvent the pawn.

| Design | Engine |
|---|---|
| Game project | Type `game`, ident `local.skyneet.survivors`, title `SkyNeet Survivors` |
| Cameras | One pawn, three `CameraComponent`s, switch `IsMainCamera` (engine sample). Bind to built-in **`View`** action (`C` / right stick click) |
| Solo now, co-op later | `GameNetworkType: Multiplayer`, `MinPlayers: 1`, `MaxPlayers` leave 8 for now. Neetmon is `NetworkMode.Object`. Host writes sim |
| Tick | 50 Hz. Move / build / AI in `OnFixedUpdate`. Camera / look / HUD in `OnUpdate` |
| Buildings, robots, nodes | `.prefab` + `SceneUtility.Instantiate` / `Clone` |
| Blueprints, items, feats, biomes, unit defs | Custom `GameResource` types with `[GameResource]` + `[Property]` |
| Campaign memory | Host `FileSystem.Data` JSON blob. Operation backpack is **not** in that file |
| AI | `NavMeshAgent` (+ `CharacterController` with `UpdatePosition = false` when collision matters). Sancient is a **director component**, not a second physics world |
| Solid buildings vs ghosts | Solidified pieces get a **dynamic `NavMeshArea` blocker**. Ghosts do not |
| HUD / ghost bills | Razor `ScreenPanel` / `WorldPanel` |
| Input | Named actions only (`Input.Pressed("View")`, etc.). Add game actions in Project Settings |
| Code | C# components on GameObjects. Hot reload. No Unity APIs, no Godot APIs |

**Do not** load the entire archipelago as one physics scene. An operation is one (or additively streamed) `Scene`. The campaign board is a light scene + Razor + the save blob.

---

## 4. Multi-agent contract (above board)

Work does not live only in a chat.

- **Source of truth:** this spec. Then `AGENTS.md`. `CLAUDE.md` and `.cursor/rules/` only point at those files.
- **Git:** every durable decision is a commit on this repo.
- **s&box MCP (first-party):** Editor → Preferences → MCP Server. Loopback only. Default URL: `http://127.0.0.1:7269/mcp`.
  - Claude Code: `claude mcp add --transport http sbox http://127.0.0.1:7269/mcp`
  - Grok Build: HTTP MCP in `~/.grok/config.toml` pointing at that URL
  - Cursor: same URL
- **Built-in toolsets on this install:** `scene`, `play`, `asset`, `component`, `package`, `editor` (`compile_status`, `console_command`), `log`.
- **Game-specific tools** go in `skyneet-survivors/Editor/` as `[McpTool]` / `[McpToolset]`, committed, so they hotload for every agent.
- **Verify in the editor, not by guessing:** after C# edits, `compile_status`. For a loop: `play_start` → `camera_screenshot` / `scene_tree` → `play_stop`.
- **No community MCP bridges.** s&box ships the server. Do not copy archived WebSocket bridges into this project.
- **No Unity.** If an API is not in s&box docs/API or `TypeLibrary`, it does not exist.

---

## 5. Operation loop

An **operation** is the Vampire Survivors hour. The campaign map is the memory. You never pick “militant” from a menu.

### 5.1 Start

From the campaign board (or New Run), Neetmon chooses:

1. **Storm length:** 15 / 30 / 45 / 60 minutes (wall clock, `[HostSync]`).
2. **Drop site** on the underground-archipelago graph.

The host loads an **operation scene** generated from that site’s `biomeId + seed + depth`. Backpack is empty except **account head-start kits**. The site is **dark**: robots do not think here until a **NeetNetNode** is planted.

### 5.2 Clock and endings

- **Silent extract:** a volume prefab that does **not** require NNN (elevator / black-tunnel). Early cash-out, smaller score, low ping.
- **Ride the clock:** survive until the chosen duration for a **full-clear bonus**.
- **Death:** operation ends. Campaign still writes (a lit abandoned node can be occupied). Score still posts if anything was extracted.
- This is **not** an extraction-shooter wallet. Extract is one ending, not the genre.

### 5.3 Pressure

- **Dark:** loot, blueprint ghosts, quiet logistics. Horde is parked wrecks, not VS.
- **Lit (NNN up):** crafting/automation on; inept Goliaths wake; loudness rises.
- **Sancient window:** chance scales with storm length and **loudness** (node up, production, gunfire, solidified buildings). During the window, a subset of robots is puppeteered.
- **Nobot interest:** lit sites on their turf (or adjacent conflict lines) can contest, occupy, or flank.

### 5.4 Emergent flavor

HUD may **name** the flavor (militant / logistics / diplomatic / subversive / expedition). It does **not** lock inputs. Flavor changes by verbs: shooting, hauling buffers, feeding a pocket until it is a **neutral subsect**, jacking a node for someone else, fattening a camp as **Trojan bait**.

### 5.5 End → campaign

Host writes `FileSystem.Data`:

- Who owns the site
- Whether a node is up
- Pipeline / stockpile contents (not deleted; occupier runs them)
- Loudness residue / conflict flags

Feats from this run credit the **shared tech tree**. A leaderboard row posts: duration chosen, survived time, extracted value, full-clear flag, flavor tags, feats triggered.

---

## 6. Building and blueprints

One system, two grain sizes, plus a planner that is not the campaign save.

### 6.1 Pieces and modules

Everything placeable is a **prefab**.

- **Piece:** one wall, floor, door, window, stair, or deployable (workbench, turret, chest, NeetNetNode).
- **Module:** a prefab of pieces (room, wall-run, turret nest).

**Top-down** stamps **modules** on a local snap grid. **First-person** places **pieces** (same grid, tighter aim). **Third-person** can do both, slower, until presets. Modules **disassemble into pieces**. Pieces **upgrade in place**. Head-start unlocks are **starter modules**, not a kept base.

v1 uses **stacked prefabs**, not a voxel mesher. Snap is component math, not Hammer.

### 6.2 Blueprint sandbox

Scene: `scenes/blueprint_sandbox.scene`. Empty plot. No storm. **No turf write**. Draft with the same dual-res tools. Save a **`Blueprint` GameResource**: local transforms, module/piece ids, **dependency graph**, bill of materials.

### 6.3 Live ghosts

Placing a blueprint in an operation stamps **ghost** instances:

- Translucent, no collision, **not** nav blockers
- Razor `WorldPanel` shows missing resources
- **Build order** from the graph: foundations → walls → roof → deployables
- Next legal nodes highlighted; illegal nodes locked
- Spending scrap **solidifies**: enable collider/renderer, add dynamic `NavMeshArea` blocker
- **Deviate:** extra pieces, skip a wing, scrap a ghost for partial refund

**NNN is a deployable piece**, last in many graphs on purpose — lighting the hole is a decision.

### 6.4 Authority

Only the host places, solidifies, or scraps. Clients send `[Rpc.Host]`. Placement fails if unsupported (floating wall) or if the piece needs power the site does not have.

### 6.5 Tiers

Data on the prefab, gated by the tech tree: **scrap → rack → plate → Pyron Chrome** (last is endgame strata, not v1 craft).

---

## 7. Factions, NNN, swarm

Four powers. Only Neets are a player body in v1.

### 7.1 Dark vs lit

Underground, machine **signals are dead**. In the dark, robots are hulks / parked wrecks — lootable. Planting a **NeetNetNode** joins **NNN**, powers the site, **and** lets machines think here.

### 7.2 Robots (Goliaths)

Unit defs (`GameResource`) carry **Lethality** (0–1) and **Competence** (0–1).

**Baseline spawn rule:** `Competence = 1 - Lethality` (inverse). High-power units miss, friendly-fire, telegraph, wander. Low-power units aim but tickle. They can still **accidentally** flatten Neetmon. Host simulates via a `Competence` component (aim cone, delay, scatter, friendly-fire chance).

### 7.3 Sancients

Rare spawn: one unit that is *wrong*. Personally weak (low Lethality). Fully aware of the catalog. For a **window** (bot count, duration, op type — scales with storm length and loudness) it **puppets** nearby robots: **Competence raised toward 1, Lethality unchanged**. That is the spike: Goliaths temporarily know their potential. When the window ends, stats revert. Killing or jacking the Sancient dumps the swarm.

Sancient is a **director GameObject** (`SancientDirector` component), not a second physics world.

### 7.4 Nobots

Breakaways. **No neutrino tech.** They do not obey Sancients. They want the node. **Roborrism:** carnage to seize NNN — rush a lit site, pile in with the inept army without coordinating, or flank off turf. They **run** captured production (occupancy), they do not delete it.

Diplomatic / subversive verbs can turn a Nobot pocket into a **neutral subsect** (barter, radicalize, compete, Trojan bait). v1 stub: don’t shoot + dump scrap + optional node-share can flip a small pack’s turf tag to Neutral.

### 7.5 Ping (host)

Loudness = node up + production + gunfire + solidified buildings. Loudness raises Sancient chance and Nobot interest on adjacent campaign turf. Silent extract in the dark barely pings.

### 7.6 Later strata (specified, not v1)

Arming Nobots with neutrino tech so they take the player’s slot as main rival; **Pyron Chrome** alloy bubble around Neet civilization; thieves, insurgents, breakaway gangs on logistics; warfront snowball (if Nobots *or* Sancients get too strong, they reopen a front on you). Same factions, bigger graph.

---

## 8. Campaign map, biomes, logistics

The archipelago is a **save**, not a single Source 2 map. Neetmon never walks the whole world. He **drops**.

### 8.1 Graph

`FileSystem.Data` on the host:

- **Sites** (nodes): `id`, `biomeId`, `seed`, `depth`, `owner`, `nodeUp`, `loudness`, `stockpile`
- **Tunnels / pipelines** (edges): `from`, `to`, `throughput`, `buffer`
- **Owners:** `Neet | Robot | SancientOp | Nobot | Neutral | Contested`

Captured production is **not deleted**. The new owner runs it, scraps it, or trades it.

### 8.2 Board

Between operations: campaign scene + Razor. Cavern-islands, depth layers, gang-turf colors, conflict lines. Pick site + storm length → generate/load operation scene from seed → on end, write delta → return to board.

### 8.3 Biomes and depth

Biome-based procedural generation with DF / Noita / RimWorld **depth**: wreck-server halls, flooded pump galleries, ore veins, junk suburbs, silent dark.

**v1:** one biome family, a handful of sites, one pipeline, two depth strata (shallow scrap vs one locked deeper pocket). Tech later unlocks **sensors and boring machines** so new sites appear or existing ones gain a lower level. Early game is scarce on purpose.

### 8.4 Logistics

A pipeline is an edge with throughput and a stockpile, not a pretty line. If Nobots take the refinery, the next op in that region is poorer and they are richer. Raiding a field does not vaporize barrels — they are on the floor or in the occupier’s buffer.

### 8.5 Time

Not an MMO sim ticking in the menu. Time advances when **operations resolve** (later: a short warfront tick on the board). Generation is deterministic from `biomeId + seed + depth` so agents and replays can rebuild the hole.

---

## 9. Meta: feats, tech tree, cameras, scoring

The operation backpack dies. The account only changes how the next drop **starts, scales, and sees**.

### 9.1 Feats

Unlocks come from **accomplishments**, not a single XP puddle. Examples:

- Survive a 15 in the dark
- Full-clear a 60 with a node up
- Cash out without firing
- Flip a site to a neutral subsect
- Kill or jack a Sancient
- Stamp a blueprint and fill it
- Extract after Trojan bait

Different **end types** pay **different feat tracks** (sprint, hold, diplomat, engineer, ghost). A feat can **gate another style** (a 15-minute ghost feat unlocks a turret used in 60-minute holds).

### 9.2 Tech tree

One tree, many branches. Nodes are `GameResource`. Progress lives in `FileSystem.Data` beside the campaign graph.

Branches: head-start kits, faster in-run increments, new formations, new perk trees, combos, camera presets.

In-run VS-style cards **reset** every operation. Account curves can bias card rarity and ghost cost. Combos between in-run cards and account perks are the long spice.

### 9.3 Cameras

- Default: steep **top-down** (template-like: high, looking down ~75°, offset back).
- **`View`** cycles Top-down → Third-person → First-person, live, one pawn.
- **Early tree:** views are unequal:
  - Top-down: full auto-fire, stamp modules, best horde read
  - Third-person: auto + a manual skill/reload you can time
  - First-person: manual gunplay, piece-precise building; body auto-weapons weaker
- **Late tree:** unlock **parity** plus **toggles/presets** (“Horde Commander”, “Fort Engineer”, “CQC”).
- Implementation: three `CameraComponent`s + `ViewLoadout` reading the preset. **Not** three player pawns.

### 9.4 Scoring / leaderboards

Posted on operation end, even on death if something was extracted. Columns: duration chosen, survived time, extracted value, full-clear flag, flavor tags, feats triggered.

Score is bragging rights. The campaign graph is the real keep.

v1: local/host stats is enough. Wire `LeaderboardType` / `Sandbox.Services` when turning it on in `.sbproj`.

---

## 10. v1 slice (what we actually build first)

Playable in the editor, host-authoritative, MCP-verifiable.

**In v1:**

- Neetmon Gould pawn + three cameras (`View` cycle) + view-linked combat
- Procedural cavern: **one biome family**, handful of sites, **two depth strata**, **one pipeline**
- Storm 15/30/45/60, silent extract vs full clear vs death
- Dual-res building + blueprint sandbox + live ghosts + NNN as last-in-graph deployable
- Dark until NNN; inept robots; **one Sancient window** if loud
- Nobots can **contest a lit site** (simple occupier)
- Campaign `FileSystem.Data` remembers turf/stocks
- A few feats → stub tech tree (one head-start kit, one curve)
- Leaderboard row on end (local/host)
- Campaign board UI good enough to pick a site and duration

**Not in v1:** Pyron Chrome, arming Nobots, many biomes, colony god, PvP, full diplomacy, warfront snowball, voxel mesher, MMO ticking.

v1 must still **feel** like: David vs inept Goliaths, radio-on is scary, ghosts want scrap, the field you lose is still *someone’s* field.

---

## 11. Repository and project layout

```
GrokPortal/
  README.md
  AGENTS.md                          # agent contract (all tools)
  CLAUDE.md                          # pointer only
  .cursor/rules/skyneet.mdc          # pointer only
  .gitignore
  docs/superpowers/specs/            # this GDD
  docs/superpowers/plans/            # implementation plans (later)
  skyneet-survivors/                 # s&box game project (scaffold after spec approval)
    <ident>.sbproj
    Assets/scenes/                   # operation, campaign_board, blueprint_sandbox
    Assets/prefabs/
    Assets/data/                     # GameResource defs
    Code/                            # gameplay components
    Editor/                          # editor tools + [McpTool] helpers
```

s&box editor opens **`skyneet-survivors/`**, not the repo root.

`.gitignore` follows the official s&box template (`.csproj`, `.sln`/`.slnx`, `bin/`, `obj/`, `*.generated.*`, compiled `*_c` assets, `.sbox/`).

---

## 12. Implementation order (high-level)

Not a task plan. Order for the forthcoming plan:

1. Scaffold s&box Player Controller project into `skyneet-survivors/`, ident, git, MCP smoke (`compile_status`, `play_start`).
2. Pawn + three-camera `View` cycle on a test cavern.
3. Storm clock + silent extract + death/end + score print.
4. Dual-res place/ghost/solidify + one module prefab + dynamic nav blocker.
5. Blueprint sandbox save/load `GameResource`.
6. Dark/lit + NeetNetNode wakes inept robots (competence inversion).
7. One Sancient director window.
8. Campaign graph save + board pick site + write occupancy (including the one pipeline).
9. Stub feats/tree + one head-start kit.
10. Nobot occupier contest on a lit site.

Each step is host-authoritative and MCP-checkable before the next.

---

## 13. Non-goals

- Unity / Godot / Unreal ports
- First-person-only Rust clone
- Always-on MMO simulation
- Client-authored combat or building
- Community MCP bridges
- Chat-only design after this file exists
- Shipping Pyron Chrome in v1

---

## 14. Spec self-review

- **Placeholders:** none. Remaining “later strata” are named systems (Pyron Chrome, arming Nobots, warfront tick), not TBDs.
- **Consistency:** backpack reset vs campaign persist; NNN as radio-on; cameras one pawn; host authority; v1 is a thin cut of the same systems.
- **Scope:** one GDD, two layers, v1 slice explicit. Implementation plan should cover v1 only.
- **Ambiguity locked:** extract is silent volume not NNN; competence inverse at spawn; Sancient raises competence and keeps lethality; time does not tick in the menu; repo root is GrokPortal, s&box project is `skyneet-survivors/`.
