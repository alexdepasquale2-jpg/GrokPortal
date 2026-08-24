# Scope ledger — vision vs. what exists

> Measures the repository against `VISION.md`. **This file changes every time code does.**
> Verified against the tree at the commit that introduced it, not from memory.
>
> **State key:** `SHIPPED` compiles and does the thing · `PARTIAL` real but reduced ·
> `STUB` placeholder with no behaviour · `NONE` not started · `PLAYTEST?` exists in code but
> has never been judged for feel.

## Headline

**The causal chain is complete end to end.** Scavenge → build → NNN → wake machines → noise
→ Sancient → leave or die → occupied field all exist and compile. That is the spine of the
whole game, and it is in.

Almost everything else in the vision — the second resolution of building, the blueprint
sandbox, two of three cameras, three of four factions' real behaviour, the entire meta
layer — is `NONE`. That is expected and correct at this stage; the point of the ledger is
that it stays visible.

## The spine (vision §3, §4, §5)

| System | Vision | State | Where |
|---|---|---|---|
| One pawn, on foot | §2 | **SHIPPED** | `TopDownController.cs`, scene `Player Controller` |
| NNN as the button | §3 | **SHIPPED** | `NeetNetNode.cs` — plant gates everything |
| Dark vs lit | §3 | **SHIPPED** | `Goliath` returns while `!NodeUp`, logs dormant once |
| Loudness | §3 | **PARTIAL** | Ticks 0.15/s + 25 on plant + 8 on build. Only the Sancient reads it |
| Building is loud | §3 | **SHIPPED** | `FortGhost` → `NotifyBuild()` |
| Inverse competence | §4 | **SHIPPED** | `CompetenceRules.cs`, unit-tested |
| Sancient reversal | §4 | **SHIPPED** | `SancientDirector.cs` — one window, competence→1, lethality untouched |
| Kill/jack the Sancient to dump the swarm | §4 | **NONE** | It cannot be attacked |
| Operation state is disposable | §5 | **SHIPPED** | Backpack never persists |
| Campaign state persists | §5 | **SHIPPED** | `CampaignStore` → `skyneet_campaign.json` |
| Captured production is not deleted | §5 | **SHIPPED** | `OccupiedSite.cs` — red fort, holds your scrap |
| The five pillars land | §11 | **PLAYTEST?** | Grok reports passing the Task 8 gate; unverified here |

## Operation loop (vision §3)

| System | State | Where |
|---|---|---|
| 15-minute storm | **SHIPPED** | `OperationDirector.StormSeconds` |
| Selectable 15/30/45/60 | **NONE** | Task 11 |
| Silent extract | **SHIPPED** | `ExtractZone.cs`, hold with feedback |
| Death end | **SHIPPED** | `NeetHealth` → `EndOperation("death")` |
| Clock end | **SHIPPED** | `EndOperation("clock")` |
| Occupied end (occupier claims a lit node) | **NONE** | Task 10 |
| Scrap economy | **SHIPPED** | `ScrapPile`, wallet, spend — scrap only, as designed |
| One auto-weapon | **SHIPPED** | `AutoNeedler.cs` |
| One Goliath | **SHIPPED** | `Goliath.cs` |
| HUD | **PARTIAL** | `OperationHud.cs` — one text block, no art |

## Building (vision §6)

| System | State | Notes |
|---|---|---|
| One module + ghost + solidify | **SHIPPED** | `FortGhost.cs` |
| Ghosts don't block, built pieces do | **SHIPPED** | Collider on solidify; `FortBlock` for Goliaths, which move by transform |
| Piece vs module (dual resolution) | **NONE** | Task 14 |
| Snap grid | **NONE** | Task 14 |
| Disassemble / upgrade | **NONE** | — |
| Blueprint sandbox scene | **NONE** | Task 15 |
| Blueprint data asset + build-order graph | **NONE** | Task 15 |
| Deviate mid-build / scrap refund | **NONE** | — |
| Tiers: scrap → rack → plate → Pyron Chrome | **NONE** | Scrap only. Pyron Chrome undefined (VISION §12) |

## Factions (vision §7)

| Faction | State | Notes |
|---|---|---|
| Neets | **SHIPPED** | One pawn |
| Robots / Goliaths | **PARTIAL** | One prefab, one behaviour. No sensors, vehicles, swarms, friendly fire |
| Sancients | **PARTIAL** | Director works. No body, no variety, cannot be killed or jacked |
| Nobots — occupancy flag | **SHIPPED** | Ownership flips; the red fort is theirs |
| Nobots — walk to a lit NNN | **NONE** | Task 10 |
| Nobots — diplomacy, barter, Trojan, arming | **NONE** | Undefined (VISION §12) |

## Campaign (vision §5)

| System | State | Notes |
|---|---|---|
| Site records persist | **SHIPPED** | `SiteRecord`: id, owner, node, stockpile, fort |
| Multi-site graph | **SHIPPED** | `CampaignGraph.DemoThreeSites()` — three caverns |
| Site selection | **PARTIAL** | `CampaignSession.SelectedSiteId`; **SkyNeet / Cycle Drop Site** and DEV 7 set it. No board UI yet (Task 12) |
| Board UI to pick a site | **NONE** | Task 12 |
| Owners beyond Neet/Occupied | **NONE** | Robot, SancientOp, Nobot, Neutral, Contested unused |
| Pipelines | **NONE** | Undefined (VISION §12) |
| Generation from `biomeId + seed + depth` | **PARTIAL** | JSON `LevelDef` layouts in `Assets/levels/` stamped by `WorldFactory`. Not seeded biomes |
| Time advances on resolve | **PARTIAL** | Implicit — resolving writes the graph; no clock |

## Cameras, flavor, meta (vision §8, §9, §10)

| System | State | Notes |
|---|---|---|
| Top-down, steep | **SHIPPED** | `TopDownController.OnPreRender` |
| Third / first person | **NONE** | Task 13 |
| `CameraRig` abstraction | **NONE** | Task 13 — pawn is *not* yet camera-ready |
| View-linked combat | **NONE** | — |
| Flavor tags as telemetry | **NONE** | Not even tagging yet |
| Flavor consequences | **NONE** | Correctly gated behind the above |
| Feats | **NONE** | Task 16 |
| Tech tree, head-starts | **NONE** | — |
| Leaderboards | **NONE** | — |

## Honest risks

1. **Nothing has been played by us.** Everything above marked SHIPPED compiles. Grok reports
   playing it and passing the Task 8 gate, but that work is unpushed (see below) and no
   pillar has been judged in this repository.
2. **Grok's verified branch is stranded.** Commits `5bec998 · 1fdab2c · 6b05450 · 2027540 ·
   b973de6 · 2fa087a · 5ec548d · b85769b` exist only at `C:\Users\Albert\GrokPortal`.
   Tasks 2–9 were implemented twice, independently. Recover or discard them deliberately.
3. **The pawn is not camera-ready**, though the spec assumes it is. Task 13 is therefore
   larger than "add a camera" — it has to retrofit the abstraction.
4. **Loudness has one consumer.** It is described as a pressure system and currently only
   opens the Sancient window.
5. **The cavern is a flat plane with boxes on it.** "One hand-authored cavern" is the
   slice's own requirement and is not yet met in any spatial sense.
