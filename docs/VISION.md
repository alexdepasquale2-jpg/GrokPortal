# SkyNeet Survivors — The Whole Game

> **What this is.** The game at full scale, described as one thing, in present tense, with
> no slice gating. Every other document in this repo describes a *cut* — what to build
> first, what to defer, what not to touch yet. This one describes what all of that is
> building toward, so progress can be measured against the real target instead of against
> the next task.
>
> **Status of this reconstruction.** Assembled 2026-08-22 from the design spec (including
> every "later / vision / not in slice" section), `FEEDBACK.md`, and the plan's deferred
> list. It is a faithful reassembly of what is written down — but the original concept
> conversation with Grok is **not** in this repository, and the Grok telemetry log does not
> contain it (it records prompt *lengths*, not prompt text). §12 lists what is therefore
> still missing. Fill those in and this becomes the real thing.

---

## 1. The hook

Not the mashup. The mashup — Vampire Survivors combat, survival building, tower defence,
remembered like a small RimWorld — is shorthand for pitching, and it undersells the game.

The hook is one sentence:

> **Every run changes the battlefield for the next run.**

You go into a hole. You turn on the node. You build a refinery. You barely survive. You die.

Next time you come back, **the refinery is still there — and somebody else owns it.**

The game remembers your failures and makes them into terrain. That is the part that makes
this its own game rather than a recombination of its inspirations, and it is the piece that
survives every cut.

## 2. The fantasy

You are **Neetmon Gould**. One body, on foot, underground.

The machines that inherited the surface still carry full Goliath catalogs — sensors, guns,
vehicles, swarms. They have **forgotten how to use them**. You are David, chased by zombie
titans that could end you instantly *if they remembered*.

Underground, machine signal is dead. They cannot think here. Until you give them a radio.

## 3. NNN is the button

A **NeetNetNode** is a neutrino antenna that joins the Neet Node Net. Planting one powers
your site, unlocks production — **and lets the machines think in this hole.**

Every operation therefore has the same shape, and the player never picks it from a menu:

| | **The Hole** (pre-NNN) | **The War** (post-NNN) |
|---|---|---|
| Machines | Parked wrecks | Awake |
| You | Explore, scavenge, plan, build quietly | Defend what you caused |
| Noise | Silent | Loudness climbing — and *building is loud* |
| Sancient | Impossible | Possible |
| Occupiers | Uninterested | Interested |
| Extract | Safe | Dangerous |

> **Explore → prepare → decide when to wake the hole → survive what you caused → leave or hold.**

The fort is not something you build *after* combat. Building is itself a threat vector —
solidifying is loud, and loudness is what brings the Sancient. The intended emergent
sentence is: *"We desperately need this refinery, but turning it on lights the node, and
we're already too loud."*

NNN sits **last in the build graph on purpose.** Lighting the hole is a decision, never a
step.

## 4. Inverse competence, and the enemy that breaks it

Every Goliath has **Lethality** (0–1) and **Competence** (0–1), and at spawn they are
inverted: `Competence = 1 - Lethality`.

- High-power units miss, wander, telegraph, friendly-fire. They can still *accidentally* flatten you.
- Low-power units aim well and tickle.

The joke is the mechanic: **"the robots are idiots" is normally safe.** The game then
periodically violates that assumption, and that violation has a name.

### The Sancient — the signature enemy

Rare. Personally weak. Fully aware of the catalog. It does not fight you; it **remembers
for the swarm**.

While its window is open, nearby robots go to **Competence 1 — Lethality unchanged**. The
same walking artillery that bounced off a wall now turns, acquires, waits, and fires.

The design target is a psychological reversal, not a bigger health bar:

> Giant robot walks into a wall. Player: *"lol."*
> Sancient arrives. Same robot turns, acquires, waits, fires. Player: *"oh shit."*

Window ends, they revert. Kill or jack the Sancient and the swarm dumps. **Preserve this at
all costs** — it is the single most memorable idea in the design.

## 5. The archipelago is a save, not a world

Neetmon **drops**. The archipelago is never loaded as one physics scene: an operation is one
scene, the campaign is data plus light UI.

The campaign graph holds sites, pipelines, and owners:

`Neet | Occupied | Robot | SancientOp | Nobot | Neutral | Contested`

- **Operation state is disposable.** Backpack, in-run tech, and the base die with the run.
- **Campaign state persists.** Site ownership and stockpiles survive.
- **Captured production is never deleted.** The new owner runs it, scraps it, or trades it.
- Time advances when operations resolve — never in a menu.
- Sites generate from `biomeId + seed + depth`.

This is what turns death from *"I lost my run"* into *"I died, and now the Nobots own that
refinery."*

## 6. Building at full scale

**Two resolutions, one grid.**

- **Piece vs module.** Top-down *stamps modules*. First-person *places individual pieces*.
  Third-person sits in between. Disassemble and upgrade both ways.
- **Blueprint sandbox.** A separate scene where construction is a creative, unpressured
  activity. Blueprints are data assets with a build-order graph; live ghosts show what you
  are working toward; you can deviate mid-build and scrap for refund.
- **Ghosts** are translucent, non-colliding, and do not block pathing. **Solid pieces do** —
  and solidifying is loud.
- **Material tiers:** scrap → rack → plate → **Pyron Chrome** (endgame).

Host-only place, solidify, and scrap.

## 7. Four factions

| Faction | Role |
|---|---|
| **Neets** | You. One pawn, on foot. |
| **Robots (Goliaths)** | Inverse competence. Dangerous by accident. |
| **Sancients** | The reversal. Weak alone, catastrophic as a director. |
| **Nobots** | Breakaway machines with no neutrino tech. **They want your node.** |

### Nobots, at full scale

Their strongest idea is the simplest one — *they want your NNN* — and the design is
deliberately staged so that idea is never buried:

1. **Occupancy flag.** Lose a lit site and ownership flips. No AI.
2. **Simple want.** NNN comes online → they come for it → whoever holds it owns the production.
3. **Full vision:** roborrism flavours, barter, radicalisation, Trojan bait, arming Nobots
   with neutrino tech, and the warfront snowball that follows.

The staging is a warning as much as a plan: Nobots are the faction most likely to become
"another system because the design says there should be another faction."

## 8. One pawn, three views

Not three characters — **one pawn with switchable views**, live.

| View | Combat |
|---|---|
| **Top-down** (default, steep) | Auto-fire — the Survivors spine |
| **Third** | Hybrid |
| **First** | Manual |

Views are unequal early and reach parity late, with presets as a meta unlock. Camera and
building are coupled on purpose: how you see is how you build.

This is a genuine differentiator and also, honestly, three combat games sharing one body.
That is why it is late.

## 9. Flavor with teeth

The HUD reads your verbs — shooting, hauling, feeding a pocket, jacking a node — and tags
the run. **You never pick "militant" from a menu.**

Tags are telemetry until each one has exactly one locked mechanical consequence:

| Flavor | Consequence |
|---|---|
| Militant | Combat feats, weapon progression |
| Logistics | Better throughput, cheaper ghosts |
| Diplomatic | Neutral pockets become useful |
| Subversive | Node sabotage, jack tools |
| Expedition | Deeper strata access |

Behaviour then changes what the campaign offers you. Labels without consequences are not a
game.

## 10. Meta — changes how the next drop starts, never a kept base

- **Feats as accomplishments**, on different tracks per end type, gating across playstyles.
- **One tech tree:** head-starts, curves, formations, perk trees, combos — *then* camera presets.
- **In-run VS cards reset.** Always.
- **Leaderboards are bragging rights. The graph is the keep.**

## 11. The five pillars

Every ~30 seconds, the player should feel one of these. A feature that reinforces none of
them should be questioned.

| Pillar | The thought |
|---|---|
| **Greed** | "There's more scrap over there." |
| **Dread** | "Should we turn on NNN?" |
| **Power** | "This fort is becoming ridiculous." |
| **Panic** | "The Sancient made them competent." |
| **Consequence** | "We lost this site, and now somebody else owns it." |

## 12. Known gaps in this reconstruction

Written down so they are visible rather than quietly missing. These are places where the
documents record a *name* but not a design, and where the original conversation would say
more:

- **Pyron Chrome.** Named as the endgame material tier and an "endgame bubble." No
  properties, acquisition, or role.
- **The warfront snowball.** Named repeatedly. No mechanics.
- **Roborrism, barter, radicalise, Trojan bait.** Named as Nobot diplomacy. No systems.
- **Biome families beyond the cavern.** Generation is `biomeId + seed + depth`; the biomes
  themselves are unnamed.
- **Pipelines.** Part of the campaign graph vision; behaviour undefined.
- **The Neet fiction.** Who the Neets are, why Neetmon is down there, what the surface is.
  The game has a strong voice in its log lines and no stated setting behind it.
- **Sancient origin and variety.** One window is specified. Whether there are kinds, where
  they come from, and whether they can be jacked permanently is open.
- **What winning looks like.** There is no described end state for the campaign.

## 13. What this is measured against

`docs/SCOPE-LEDGER.md` maps every system above to what actually exists in the repository
today. This document should change only when the *concept* changes. The ledger changes
every time code does.
