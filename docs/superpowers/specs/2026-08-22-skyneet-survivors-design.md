# SkyNeet Survivors — Game Design

**Date:** 2026-08-22  
**Amended:** 2026-08-24 — operation layouts are JSON `LevelDef` files; C# hot reload + DEV keys are the iterate loop. Original 2026-08-22 amend from [`FEEDBACK.md`](../../../FEEDBACK.md) on `feat/skyneet-survivors-design`.  
**Status:** Spec amended after review. First implementation target is the **vertical slice** (§11), not the full v1 list.  
**Engine:** s&box (Source 2 + .NET 10, Steam editor)  
**Repo:** https://github.com/alexdepasquale2-jpg/GrokPortal  
**Local game folder (when scaffolded):** `skyneet-survivors/` in this repo.  
**Ident:** `local.skyneet_survivors` (`Org: local`, `Ident: skyneet_survivors`)  
**Protagonist:** Neetmon Gould (one Neet, on foot)

This document is the source of truth. Grok Build, Claude Code, and Cursor must follow it. Chat decisions after this date belong in a spec amendment commit, not in a private transcript.

**Causal chain (protect this):**  
Scavenge → build → activate NNN → wake machines → make noise → attract Sancient / occupiers → defend or escape → alter territory → return to a changed world.

The field you lose is still **someone else’s field**. That is the piece that must survive every cut.

---

## 1. What it is

SkyNeet Survivors is a **PvE operation game** with a **living underground war**.

Shorthand: Vampire Survivors combat, survival building, tower defense, remembered like a small RimWorld. The **actual hook** is not the mashup. It is:

> Every run changes the battlefield for the next run.

You play **Neetmon Gould**, one body, in an **underground-archipelago**. Killer robots still have Goliath catalogs (sensors, guns, vehicles, swarms). They have **forgotten how to use them**. You are David chased by zombie titans that could end you **if they remembered**.

Machines cannot think underground until someone plants a **NeetNetNode** (neutrino antenna onto **NNN**, the Neet Node Net). That is the radio-on. **NNN is the button.** Before it: the Hole (explore, scavenge, plan, quiet ghosts). After it: the War (machines wake, production, loudness, Sancient possible, occupiers interested, extract gets dangerous).

The swarm is inept until a **Sancient** puppets them. **Nobots** (breakaway machines, no neutrino tech) want that node. Your backpack and in-run base **die with the operation**. The **archipelago save** keeps turf and who is running the field.

**Architecture (Approach 1):** two layers, **operation-first**. The GDD describes the full war. The first playable is a **vertical slice of the causal chain**. Cameras, dual-res building, blueprint sandbox, diplomacy, many biomes, and the full campaign board are **earned after the slice is fun**.

---

## 2. What makes this fun

If a proposed feature does not reinforce one of these, question it.

| Pillar | Every ~30 seconds the player should feel |
|---|---|
| **Greed** | “There’s more scrap over there.” |
| **Dread** | “Should we turn on NNN?” |
| **Power** | “This fort is becoming ridiculous.” |
| **Panic** | “The Sancient made them competent.” |
| **Consequence** | “We lost this site, and now somebody else owns it.” |

NNN is the defining decision of an operation:

**Explore → prepare → decide when to wake the hole → survive what you caused → leave or hold.**

---

## 3. Key decisions

| Decision | Choice | Rationale |
|---|---|---|
| Session vs world | Two-layer campaign | Runs reset; war remembers |
| Persistence | Backpack/base/in-run tech reset. Site ownership + stocks persist | Failure is interesting |
| First playable | **Vertical slice** (§11), not full v1 | Prove the causal chain before cameras/economy/diplomacy |
| Economy (slice + v1) | **Scrap only** | Logistics under pressure, not a spreadsheet |
| Flavor tags | **Telemetry until systems exist** | Labels without consequences are not a game |
| Avatar | One Neet: Neetmon Gould | VS spine |
| PvP | None until much later | PvE horde-hold |
| Camera (vision) | Live switch top-down / third / first; default steep top-down | Differentiator |
| Camera (**slice**) | **Top-down only**; pawn is camera-ready | Don’t ship three combat games to find out if one is fun |
| Combat (slice) | Top-down auto-fire | Classic VS |
| Combat (later) | View-linked hybrid, then presets | Spice after the loop works |
| Building (slice) | One module, ghosts, scrap to solidify, NNN as deployable | Fort is part of the threat (loudness) |
| Building (later) | Dual-res + blueprint sandbox | After slice |
| Space | Underground-archipelago | Signal dies underground; NNN is the lever |
| Factions | Neets, inept Robots, Sancients, Nobots | Inverse competence + hive spike + occupiers |
| Nobots (slice) | Occupancy flag only | Protect “someone else’s field” without a third AI |
| Nobots (next) | NNN on → they want it → attack/occupy | No diplomacy/barter/Trojan until later |
| Sancient | Signature enemy: same robots, suddenly competent | Psychological reversal, not a fatter HP bar |
| Networking | Host-authoritative even when solo | Co-op later is not a rewrite |
| Agent workflow | Git + first-party s&box MCP | Shared files + live editor |

---

## 4. Design invariants vs implementation assumptions

**Invariants** (do not casually change):

- One pawn: Neetmon Gould.
- Host writes simulation.
- Dark until NNN; NNN wakes machines and is loud.
- Inverse competence at spawn; Sancient raises competence, **keeps lethality**, for a window.
- Operation backpack dies; **site ownership / stocks persist**.
- Captured production is not deleted.
- Slice is top-down; later, **one pawn** with switchable views (not three characters).
- Ghosts are not solid and do not block pathing; built pieces do.

**Assumptions** (ok to change if s&box makes a better path):

- Three `CameraComponent`s + `IsMainCamera` (engine sample). Bind later to built-in `View`.
- Dynamic `NavMeshArea` as the pathing blocker.
- Campaign in `FileSystem.Data` JSON.
- Unit/feat/blueprint defs as `GameResource`.
- MCP at `http://127.0.0.1:7269/mcp` and the current built-in toolset names.
- Template: Game – Player Controller.

If an assumption fights the editor, keep the invariant and change the assumption.

---

## 5. s&box environment

Editor install (this PC):

- `C:\Program Files (x86)\Steam\steamapps\common\sbox`
- Launch: `sbox-dev.exe` (or Steam **s&box**)
- Editor build observed: `26.08.19`
- Docs: https://sbox.game/dev/doc/
- API: https://sbox.game/api/
- MCP: https://sbox.game/dev/doc/editor/mcp-server

**Create the game from the Game – Player Controller template.** Do not start from Empty.

Current working assumptions (not design law):

| Need | Current s&box path |
|---|---|
| Game project | Type `game`, ident `local.skyneet.survivors`, title `SkyNeet Survivors` |
| Slice camera | One steep top-down camera on the pawn |
| Later cameras | One pawn, switchable views (`CameraComponent` + `IsMainCamera` is the documented sample) |
| Solo now, co-op later | `GameNetworkType: Multiplayer`, `MinPlayers: 1`. Neetmon `NetworkMode.Object`. Host writes sim |
| Tick | 50 Hz. Move / build / AI in `OnFixedUpdate`. Camera / HUD in `OnUpdate` |
| Spawnables | `.prefab` + instantiate/clone |
| Operation layouts | JSON `LevelDef` in `Assets/levels/{siteId}.json`. C# builtin fallback in `LevelCatalog`. One scene (`cavern.scene`); data picks the hole |
| Data defs | `GameResource` later (units / feats / blueprints). Not used for layouts — unconfirmed attribute, and JSON is already the snappy path |
| Campaign memory | Host `FileSystem.Data`. Operation backpack is **not** in that file |
| AI | Nav mesh agent + competence component. Sancient is a **director**, not a second physics world |
| HUD | Razor screen/world panels |
| Input | Named actions only |
| Code | C# components. Hot reload. No Unity / Godot APIs |

**Snappy loop (assumption, not a design pillar):** edit C# → editor hot-reloads → see it. Edit or add `Assets/levels/{id}.json` → Play or DEV 6/7/9 → the hole restamps. `SkyNeet / New Level` writes the next `cavern_N.json` and selects it. DEV slot keys 1–9 jump the causal chain; `SkyNeet / Dev Loop Off` for Task 8. Default drop remains `cavern_0` (15-minute slice). `dev_60s` is an iterate hole, not a spec duration.

**Do not** load the entire archipelago as one physics scene. An operation is one scene. The campaign board is data + a light UI.

---

## 6. Multi-agent contract (above board)

Work does not live only in a chat.

- **Source of truth:** this spec, then `AGENTS.md`. `CLAUDE.md` and `.cursor/rules/` only point at those files.
- **Git:** every durable decision is a commit on this repo. `FEEDBACK.md` is review input; accepted points are merged **into this spec**.
- **s&box MCP (first-party):** Editor → Preferences → MCP Server. Loopback only. Default URL: `http://127.0.0.1:7269/mcp`.
  - Claude: `claude mcp add --transport http sbox http://127.0.0.1:7269/mcp`
  - Grok: HTTP MCP in `~/.grok/config.toml`
  - Cursor: same URL
- After C# edits: `compile_status`. Smoke: `play_start` → screenshot / `scene_tree` → `play_stop`.
- **No community MCP bridges.** Game-specific tools go in `skyneet-survivors/Editor/` as `[McpTool]`, committed.
- **No Unity.** If an API is not in s&box docs/API or `TypeLibrary`, it does not exist.

---

## 7. Operation loop

NNN is the button. Structure of every operation:

**The Hole** (pre-NNN): exploration, scavenging, planning, quiet ghosts, gathering.  
**The War** (post-NNN): machines awaken, production possible, loudness up, Sancient possible, occupiers interested, fortifications matter, extract is dangerous.

You never pick “militant” from a menu.

### 7.1 Start

**Slice:** drop into the one cavern. Storm is **15 minutes**. No board required.

**Later:** from the campaign board, choose storm length (15 / 30 / 45 / 60) and a drop site. Host loads an operation scene from `biomeId + seed + depth`. Backpack empty except unlocked head-starts. Site is **dark**.

### 7.2 Clock and endings

- **Silent extract:** a volume that does **not** require NNN. Early cash-out, low ping, smaller score.
- **Ride the clock:** survive the duration.
- **Death:** operation ends. Campaign still writes. If NNN was up, **ownership can flip**.
- Not an extraction-shooter wallet. Extract is one ending, not the genre.

### 7.3 Pressure

- **Dark:** loot, ghosts, quiet prep. Robots are parked wrecks.
- **Lit:** Goliaths wake (inept). Loudness rises. Building itself is loud.
- **Sancient window:** if the hole is loud enough, one Sancient puppets nearby robots.
- Occupiers: see §9.4. Slice = ownership flag. Next = simple want-NNN attack.

### 7.4 Flavor (telemetry until earned)

HUD **may** tag militant / logistics / diplomatic / subversive / expedition from verbs. **Slice and v1: telemetry only.** No mechanical consequence until each tag has **one** locked effect, for example:

| Flavor | Later consequence (not slice) |
|---|---|
| Militant | Combat feats / weapon progression |
| Logistics | Better throughput / cheaper ghosts |
| Diplomatic | Neutral pockets more useful |
| Subversive | Node sabotage / jack tools |
| Expedition | Deeper strata access |

Do not implement those effects until the slice loop is fun.

### 7.5 End → campaign

Host writes persistent site state. Minimum for the slice: **`owner`** (and whether the node is still up). Later: stockpile, pipelines, loudness residue.

Score / feats / leaderboards are **after** the slice. The keep is the graph, not the score.

---

## 8. Building and blueprints

### 8.1 Slice

- **One resource: scrap.**
- **One module** (a small fort chunk) + **NNN as a deployable**.
- Ghost of that module: translucent, no collision, no pathing block. Spend scrap → solid. Solid is loud and blocks pathing.
- No blueprint sandbox, no piece-level dual-res, no extra currencies (fuel, food, ammo, refined metal).

### 8.2 Later vision (unchanged, not first build)

- **Piece** vs **module**. Top-down stamps modules; first-person places pieces; third-person in between. Disassemble / upgrade.
- Blueprint sandbox scene; `Blueprint` data asset; live ghosts with build-order graph; deviate / scrap refund.
- Tiers: scrap → rack → plate → Pyron Chrome (endgame).
- Host-only place/solidify/scrap.

NNN stays **last in the graph on purpose**. Lighting the hole is a decision.

---

## 9. Factions, NNN, swarm

### 9.1 Dark vs lit

Underground, machine **signals are dead**. Planting NNN joins NNN, powers the site, **and** lets machines think here.

### 9.2 Robots (Goliaths)

**Lethality** (0–1) and **Competence** (0–1).

**Spawn rule:** `Competence = 1 - Lethality`. High-power units miss, wander, telegraph, friendly-fire. Low-power units aim but tickle. They can still **accidentally** flatten you.

The joke is the game: “the robots are idiots” is **normally safe**. The Sancient periodically **violates that**.

### 9.3 Sancients (signature enemy)

Rare: one unit that is *wrong*. Personally weak. Fully aware of the catalog.

**Window:** puppets nearby robots — **Competence → 1, Lethality unchanged**. Same walking artillery that bounced off a wall now **turns, acquires, waits, fires**. Window ends → revert. Kill/jack the Sancient → dump the swarm.

Preserve the psychological reversal. The Sancient is not “a bigger HP bar.” It is the moment the catalog remembers itself.

Director object (implementation assumption: a component on a GameObject). Not a second physics world.

### 9.4 Nobots

They want **your NNN**. That is enough.

**Slice:** no Nobot AI. If you die (or extract) with the node up, `owner` becomes **Occupied**. Next drop, the module/production is still there, **not yours**.

**Next (after slice is fun):** NNN comes online → occupiers want it → they attack/occupy → whoever controls it owns the production. Stupidly simple.

**Later (not v1):** roborrism flavors, barter, radicalize, Trojan bait, arming Nobots with neutrino tech, Pyron Chrome, warfront snowball.

---

## 10. Campaign, biomes, logistics

The archipelago is a **save**, not a Source 2 open world. Neetmon **drops**.

**Slice:** one site. Persist `owner` (+ node up). If occupied, the solidified module is still in the hole.

**Vision:** graph of sites and pipelines; owners `Neet | Occupied | Robot | SancientOp | Nobot | Neutral | Contested`; captured production runs for the new owner. Board UI later. Time advances when operations resolve, not in the menu. Generation later from `biomeId + seed + depth`.

**Economy:** scrap is the only resource until the slice (and likely v1) proves the loop. No extra currencies.

---

## 11. Vertical slice (first playable — build this)

Prove the causal chain in **one 15-minute hole**. If this is not fun, cameras, blueprints, and the campaign board will not save it.

| Piece | Slice |
|---|---|
| Space | **One** hand-authored (or lightly generated) cavern |
| Storm | **15 minutes** only |
| Camera | **Top-down** only; pawn structured so views can be added |
| Resource | **Scrap** |
| Building | **One** module + ghost + solidify |
| Weapon | **One** auto-weapon |
| Robot | **One** Goliath prefab, inverse competence |
| NNN | **One** deployable that wakes the hole |
| Sancient | **One** window if loud / NNN up |
| Extract | **One** silent extract volume |
| Persistence | **One** site `owner` flag (and node-up) |

**Feel target:** greed for scrap, dread of the button, inept Goliaths you laugh at, panic when the Sancient arrives, consequence when the next drop shows **their** fort.

**Not in the slice:** third/first person, view-linked combat, dual-res pieces, blueprint sandbox, multiple durations, procedural biomes, pipelines, feats/tech tree, leaderboards, Nobot agents, diplomacy, campaign board UI, extra currencies.

Host-authoritative. MCP-verifiable (`play_start` through extract/death/NNN/Sancient).

---

## 12. v1 (only after the slice is fun)

Thin campaign on the **same** systems:

- A handful of sites, still one biome family, still **scrap**
- 15/30/45/60 optional
- Simple occupier that walks to a lit NNN and claims the site
- Campaign file with owner + stockpile; a board good enough to pick a site
- Stub feats optional, not required for v1 ship-to-self

**Still not v1:** three cameras (unless slice was trivial to extend), blueprint sandbox, dual-res, Pyron Chrome, arming Nobots, many biomes, colony god, PvP, full diplomacy, warfront snowball, voxel mesher, MMO ticking, extra currencies, flavor mechanics.

---

## 13. Meta (after v1)

Account changes how the next drop **starts, scales, and sees** — never a kept base.

- Feats as accomplishments, different tracks per end type, gates across styles
- One tech tree: head-starts, curves, formations, perk trees, combos, **then** camera presets
- In-run VS cards reset
- Cameras: add third, then first, on the **same pawn**; early views unequal; late parity + presets
- Leaderboards are bragging rights; the graph is the keep

---

## 14. Repository layout

```
GrokPortal/
  README.md
  AGENTS.md
  CLAUDE.md
  FEEDBACK.md                        # review input; do not treat as spec
  .cursor/rules/skyneet.mdc
  docs/superpowers/specs/            # this GDD
  docs/superpowers/plans/
  skyneet-survivors/                 # s&box project (scaffold next)
```

s&box editor opens **`skyneet-survivors/`**. `.gitignore` follows the official s&box template.

---

## 15. Implementation order

Not a task plan. The **plan must implement §11 only** until the slice is fun.

1. Scaffold Player Controller project into `skyneet-survivors/`, ident, MCP smoke.
2. Pawn + **top-down** camera on the one cavern (camera-ready, not three views).
3. Move + **one** auto-weapon + **one** inept Goliath.
4. Scrap pickup + **one** module ghost/solidify (loudness on solidify).
5. 15-minute clock + silent extract + death end.
6. NNN deployable: dark → lit, wakes Goliaths.
7. Sancient window: same Goliaths become competent.
8. Persist `owner` (Occupied if you leave/die with NNN up). Next playthrough the module is still there, not yours.

Stop. Play it. If the 15-minute “wake the hole and survive what you caused” loop works, **then** earn occupier AI, more sites, more durations, cameras, blueprints.

---

## 16. Non-goals (until explicitly earned)

- Shipping three cameras in the slice
- Flavor as mechanics
- Extra currencies
- Nobot diplomacy / Trojan / radicalize
- Unity / Godot / Unreal
- Client-authored combat or building
- Community MCP bridges
- Chat-only design
- Pyron Chrome in slice or v1
- Treating mashup shorthand as the marketing hook

---

## 17. Spec self-review

- **Placeholders:** none. Later strata are named.
- **Feedback absorbed:** smaller first playable; cameras deferred; flavor = telemetry; scrap-only; NNN as the button; Sancient as signature reversal; Nobots occupancy-first; invariants vs assumptions; fun pillars; “someone else’s field” protected.
- **Consistency:** backpack reset vs site persist; competence inverse; Sancient keeps lethality; slice ⊂ v1 ⊂ vision.
- **Scope of next plan:** §11 vertical slice only.
