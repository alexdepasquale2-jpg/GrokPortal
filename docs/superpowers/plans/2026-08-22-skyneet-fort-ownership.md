# Fort ownership on retake

> **Gate:** do not implement until Task 8 (slice playtest) says the 15-minute hole is fun, and Task 9 (campaign graph) is in flight. Claude flagged the gap; Cursor wrote the plan so nobody invents a third save shape.

**Goal:** If you lose a site with a standing fort, then later take it back, you get **that fort**, not a ghost you re-buy. The field you lose is still someone’s — including when it becomes yours again.

**Why this is not Task 5:** `CampaignSave` today can say a fort stands and who owns the site, but not “the standing fort is mine.” Task 5 correctly only spawns a solid red fort when `Owner == "Occupied"`. Closing the hole by special-casing `Owner == "Neet" && FortStanding` without a dedicated field will lie the first time a Neet extracts in the dark after an occupied drop.

## File ownership

- Create: `skyneet-survivors/Code/SkyNeet/FortRecord.cs` (or fields on `SiteRecord` once Task 9 exists)
- Modify: `CampaignSave` / `CampaignStore` / `WorldFactory.BuildFort` — **only the agent who owns Task 9**, or a follow-up that takes those files after Task 9 merges
- Do not edit `FortGhost.cs` or `OccupiedSite.cs` unless that agent still owns them

## Shape

```csharp
public sealed class FortRecord
{
	public bool Standing { get; set; }
	public bool Solid { get; set; }
	public string BuiltBy { get; set; } = "Neet";
	public Vector3 Position { get; set; } = new Vector3( 0, -80, 16 );
}
```

Spawn rules:

| Owner | FortStanding | What you see |
|---|---|---|
| Occupied | true | Red `OccupiedSite`, blocks, not usable (already Task 5) |
| Neet | true, Solid | Your solid fort, Neet tint, blocks, no ghost cost |
| Neet | false | Cheap ghost as today |
| anyone | false | Ghost |

Retake path: extract (or clock/death **without** flipping Occupied — only extract-in-the-dark / extract-while-lit-Neet-keeps-it per existing `EndOperation` rules) after an occupied drop must write `Owner = Neet` **and** keep `FortStanding` if the red fort was still there.

## Steps

- [ ] **Step 1:** Add `FortRecord` (or equivalent on `SiteRecord`) without changing spawn yet. Logic test: round-trip JSON through `CampaignStore`.
- [ ] **Step 2:** `WorldFactory.BuildFort` reads the record. Occupied still red; Neet+standing is a solid Neet fort, not a ghost.
- [ ] **Step 3:** Play: lose the site with a bought fort → Occupied red fort. Next drop, silent extract in the dark → third drop shows **your** solid fort, no 20-scrap ghost.
- [ ] **Step 4:** Commit `feat: retaken sites keep the fort you already paid for`

## Out of scope

Nobot AI, dual-res pieces, blueprint stamps, extra currencies. This is persistence fidelity for the one slice module.
