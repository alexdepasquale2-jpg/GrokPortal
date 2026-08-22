# SkyNeet Survivors — after v1 (named, not TBD)

> **Gate:** Task 8 human playtest must pass, then Tasks 9–12 (campaign graph, occupier, storm length, board) from `2026-08-22-skyneet-survivors.md`. Do **not** implement this file until those are in. Cursor wrote this so Claude/Grok have dispatch-ready work if the slice queue empties.

Each section is its own later PR series. File ownership: one agent per section; do not share `OperationDirector.cs` — add new files.

Spec references: design §8.2, §9.4 later, §10 vision, §13 meta, §16 non-goals.

---

## A. Occupier wants the lit node (v1 leftover if Task 10 slipped)

Already specified as Task 10 in the slice plan. If that task is still open after Task 8: implement **exactly** that file set (`Occupier.cs` + `WorldFactory` spawn). No diplomacy.

---

## B. Pyron Chrome bubble

**Goal:** Endgame material is a **site-scale shield**, not a new wallet currency during the run. Scrap remains the only in-run resource. Chrome is campaign-layer and rare.

**Files:** create `PyronChrome.cs`, `ChromeBubble.cs`; do not add a HUD scrap-like counter.

```csharp
public sealed class ChromeBubble : Component
{
	[Property] public float Radius { get; set; } = 400f;
	[Property] public float HitsAbsorbed { get; set; } = 3f;
}
```

- [ ] **Step 1:** `GameResource` or site flag `HasChrome` on `SiteRecord` only. No in-run pickup.
- [ ] **Step 2:** If the selected site has chrome, spawn a bubble that eats N Goliath hits then pops (loud).
- [ ] **Step 3:** Popping does **not** delete captured production. Owner unchanged.
- [ ] **Step 4:** Commit `feat: Pyron Chrome as a site bubble, not a currency`

**Not this:** refined-metal chain, fuel, ammo.

---

## C. Arming Nobots (neutrino leak)

**Goal:** After occupancy is visible **and** occupiers walk to NNN, a later beat: a Nobot that **gets the catalog** is an existential mistake. One prefab, one leak action, no barter tree.

**Files:** create `Nobot.cs`, `NeutrinoLeak.cs`. Do not extend `Goliath` competence math — leak **sets** competence to puppet for that unit only.

- [ ] **Step 1:** Nobot uses Occupier move-to-node. If it holds the node 10s, `LeakCatalog()`.
- [ ] **Step 2:** Leaked Nobot: `Competence = CompetenceRules.PuppetCompetence()`, lethality unchanged.
- [ ] **Step 3:** HUD `THEY HAVE THE NET` once. Not a second Sancient.
- [ ] **Step 4:** Commit `feat: Nobot catalog leak, no diplomacy`

**Not this:** Trojan bait, radicalize, roborrism flavors.

---

## D. Biome procedural depth

**Goal:** Operation scene still **one cavern at a time**. Generation is `biomeId + seed + depth` → layout offsets, not an open-world stream.

**Files:** create `BiomeId.cs`, `CavernGen.cs`; `WorldFactory` stays the spawn API.

- [ ] **Step 1:** Enum `Cavern | Rift | MachineYard` (three names, one implemented).
- [ ] **Step 2:** `CavernGen.Layout( seed, depth )` returns positions for node, extract, scrap, fort, goliath. Depth only stretches distances.
- [ ] **Step 3:** `WorldFactory.Build` consumes the layout. Authored `cavern.scene` still wins if objects exist.
- [ ] **Step 4:** Commit `feat: seeded cavern layout, still one scene`

**Not this:** voxel mesher, loading the archipelago as physics.

---

## E. Flavor mechanics (one locked effect each)

**Goal:** HUD tags are telemetry until each flavor has **exactly one** effect. Implement one flavor per PR.

| Flavor | One effect |
|---|---|
| Militant | AutoNeedler interval * 0.9 after a Goliath kill this operation |
| Logistics | Fort ghost costs 15 instead of 20 |
| Diplomatic | Occupier spawn delayed 30s |
| Subversive | Planting NNN is 10 less loud (still wakes machines) |
| Expedition | Storm clock +60s once per site, campaign only |

- [ ] **Step 1:** `FlavorTracker.cs` counts verbs (plant, extract dark, kill, build). No effects yet.
- [ ] **Step 2:** Pick **one** row. Ship the effect. Spec table in §7.4.
- [ ] **Step 3:** Repeat per flavor in separate PRs.

**Not this:** all five in one PR, or tags that do nothing forever.

---

## F. Leaderboards (`Sandbox.Services`)

**Goal:** Bragging rights. The graph is still the keep. Scores do not unlock production.

**Files:** create `OperationScore.cs`, `LeaderboardSubmit.cs`.

- [ ] **Step 1:** Score = scrap extracted + (NNN planted ? 0 : 50 dark bonus) − deaths. Integer.
- [ ] **Step 2:** Submit only on `extract` or `clock`. Death does not submit a pride number.
- [ ] **Step 3:** Use `Sandbox.Services` **if** `TypeLibrary` has it; otherwise local `FileSystem.Data` `skyneet_scores.json` and stop. Do not invent a web backend.
- [ ] **Step 4:** Commit `feat: operation score as bragging rights`

---

## G. Warfront tick

**Goal:** Time on the campaign board advances when an **operation resolves**, not in a menu idle loop. Occupied sites can tick stockpile +1 scrap per resolved operation elsewhere, capped.

**Files:** create `WarfrontTick.cs`. Call from `CampaignStore` after `Write`.

- [ ] **Step 1:** After each `EndOperation`, for each site with `Owner != "Neet"`, `Stockpile = min(Stockpile + 1, 99)`.
- [ ] **Step 2:** No realtime timer on the board scene.
- [ ] **Step 3:** Log `[SkyNeet] warfront tick` once per resolved operation.
- [ ] **Step 4:** Commit `feat: warfront advances on operation resolve`

**Not this:** MMO simulation, background server, other players’ sessions.

---

## Dispatch rule

When Claude or Grok is idle after Task 8:

1. Remaining slice bugs from the verification queues
2. Tasks 9–12 in the original plan (in that order)
3. Fort ownership plan (`2026-08-22-skyneet-fort-ownership.md`)
4. Then A–G above, **one letter per agent**, new branch each time
