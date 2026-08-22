# SkyNeet Survivors Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make SkyNeet Survivors a playable s&box game whose causal chain (scavenge → NNN → inept Goliaths → Sancient → extract/death → occupied field) works in-editor, then layer v1 campaign and later systems without rewriting the spine.

**Architecture:** Host-authoritative operation scene (`cavern.scene`) plus `FileSystem.Data` campaign save. One pawn (Neetmon Gould). NNN is the button that wakes machines. Inverse competence until a Sancient puppets Goliaths. Backpack dies; site `owner` persists. Later systems (cameras, dual-res, blueprints, feats) attach to this spine; they do not replace it.

**Tech Stack:** s&box 26.08.19 (Source 2 + .NET 10 C# components), Razor avoided for HUD, `[Sync( SyncFlags.FromHost )]`, `GameResource` later, first-party editor MCP at `http://127.0.0.1:7269/mcp`.

## Global Constraints

- Spec: `docs/superpowers/specs/2026-08-22-skyneet-survivors-design.md` (amended). Do not invent systems that contradict it.
- Game folder: `skyneet-survivors/` in this repo. Editor also sees `C:\Users\Albert\Documents\s&box projects\skyneet_survivors` (junction). Open **that project**, not repo root.
- Package ident: **`local.skyneet_survivors`** (`Org: local`, `Ident: skyneet_survivors`, file `skyneet_survivors.sbproj`). Never `local.skyneet.survivors` (three-part ident fails to mount).
- Startup scene: `scenes/cavern.scene`.
- Host writes sim. Clients only send `[Rpc.Host]` later. Solo is still Multiplayer `MinPlayers: 1`.
- `[Sync( SyncFlags.FromHost )]` only. Do not use `[HostSync]`.
- `global using System;` stays in `Code/Assembly.cs`.
- HUD is C# `PanelComponent` + `Panel.AddChild<Label>()`. Do **not** add `.razor` HUD files (Razor gen is a different class and crashed bootstrap).
- No Unity/Godot APIs. If it is not in s&box docs/API/`TypeLibrary`, it does not exist.
- Economy in slice/v1: **scrap only**. Flavor tags: telemetry only.
- Slice camera: top-down only. Pawn may be camera-ready; do not ship three combat games in Phase 1.
- Verify every task: s&box editor compile (0 errors) + the task's play/log check. MCP tools: `compile_status`, `play_start`, `play_stop`, `list_scenes`, `scene_tree`, `camera_screenshot`.
- Git branch: `main` (default; `feat/skyneet-survivors-design` was the original and is kept in sync). Commit per task. Do not commit `.sbox/`, `*.csproj`, `*.sln`, `*_c`.
- Fun pillars: greed, dread, power, panic, consequence. If a change hits none, drop it.

## MCP (every agent)

Editor must be running (`sbox-dev`). Log line: `[MCP] MCP server listening on http://127.0.0.1:7269/mcp`.

```
# Grok (project)
# .grok/config.toml already has:
# [mcp_servers.sbox]
# url = "http://127.0.0.1:7269/mcp"

# Claude
claude mcp add --transport http sbox http://127.0.0.1:7269/mcp

# This Grok chat started before sbox MCP was attached; new agent sessions in a trusted GrokPortal folder pick it up.
```

Editor → Preferences → MCP Server enabled. After C# edits: `compile_status` until 0 errors, then `play_start`.

## Current code (do not re-scaffold)

Already in `skyneet-survivors/Code/SkyNeet/`:

| Type | Role |
|---|---|
| `OperationDirector` | Clock, scrap wallet, NNN flag, loudness, end reasons, `EnsureWorld` spawn, save load/write |
| `CampaignSave` | `SiteId`, `Owner`, `NodeUp`, `OccupierScrap`, `FortStanding` |
| `NeetNetNode` | E to plant |
| `ExtractZone` | Hold to end `extract` |
| `ScrapPile` | Walk-over scrap |
| `FortGhost` | E + scrap → solid + loudness |
| `Goliath` | Inverse competence, miss/hit, Sancient puppet |
| `SancientDirector` | Loudness + time window |
| `NeetHealth` / `AutoNeedler` | Player HP + auto gun |
| `OperationHud` | C# `PanelComponent` |
| `CustomTopDownController` | In `TopDownController.cs`, used by `cavern.scene` |

Phase 1 **finishes** this slice (boot, readability, occupancy that you can see). Do not rewrite from empty.

## Dispatch map

| Wave | Tasks | Parallel? |
|---|---|---|
| **0** | Task 1 (boot) | **No.** One agent. |
| **1** | Tasks 2, 3 | **Yes**, if they keep to listed files. |
| **2** | Tasks 4, 5 | Sequential (both touch save/NNN feel). Prefer one agent, or 4 then 5. |
| **3** | Tasks 6, 7 | **Yes** (Goliath/Sancient vs extract/clock logs). |
| **Gate** | Task 8 | Human playtest. No parallel. |
| **4** | Tasks 9–12 | 9 first, then **10 ∥ 11**, then 12. |
| **5** | Tasks 13–16 | After 12. **13 ∥ 15** if they do not share pawn files; 14 after 13; 16 last. |

**File ownership:** an in-flight agent must not edit a file listed as another in-flight task's Files. If you need a shared type, add a new file instead of enlarging `OperationDirector.cs`.

## File map (target)

```
skyneet-survivors/
  skyneet_survivors.sbproj
  Code/Assembly.cs
  Code/SkyNeet/
    CompetenceRules.cs          # Task 2
    CampaignStore.cs            # Task 5
    WorldFactory.cs             # Task 3
    OccupiedSite.cs             # Task 5
    OperationDirector.cs
    ...existing components...
  Editor/SkyNeetLogicTests.cs   # Task 2
  Assets/scenes/cavern.scene
```

---

### Task 1: Editor boots `local.skyneet_survivors`

**Files:**
- Modify: `skyneet-survivors/skyneet_survivors.sbproj` (ident must be `skyneet_survivors`)
- Modify: any leftover `skyneet.survivors.sbproj` — delete it
- Verify: `Code/Assembly.cs` has `global using System;`
- Verify: no `OperationHud.razor`, no `[HostSync]`

**Interfaces:**
- Consumes: existing project tree
- Produces: editor opens without Bootstrap::Init Error; compile 0 errors

- [ ] **Step 1: Confirm ident**

Open `skyneet-survivors/skyneet_survivors.sbproj` and ensure:

```json
"Org": "local",
"Ident": "skyneet_survivors",
"Type": "game",
"Metadata": {
  "StartupScene": "scenes/cavern.scene"
}
```

If `skyneet.survivors.sbproj` still exists, delete it. Package name must be `local.skyneet_survivors`, not `local.skyneet.survivors`.

- [ ] **Step 2: Grep forbidden APIs**

```
HostSync
OperationHud.razor
Panel.Add.Label
```

Must be zero hits under `skyneet-survivors/Code`.

- [ ] **Step 3: Compile in editor**

Open **File → Open Project** on `C:\Users\Albert\Documents\s&box projects\skyneet_survivors`. MCP `compile_status` (or editor Output): 0 errors. Warnings about unrelated engine packages are fine.

- [ ] **Step 4: Play**

MCP `play_start` or editor Play. Console must contain `[SkyNeet] Drop into cavern_0`. No Bootstrap dialog.

- [ ] **Step 5: Commit**

```bash
git add skyneet-survivors/skyneet_survivors.sbproj
git commit -m "fix: ensure local.skyneet_survivors package boots in editor"
```

---

### Task 2: CompetenceRules + editor logic tests

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/CompetenceRules.cs`
- Create: `skyneet-survivors/Editor/SkyNeetLogicTests.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/Goliath.cs` (use CompetenceRules; do not change movement)

**Interfaces:**
- Consumes: `Goliath.Lethality`
- Produces:

```csharp
public static class CompetenceRules
{
	public const float Min = 0.05f;
	public const float Max = 0.95f;
	public static float SpawnCompetence( float lethality );
	public static bool RollHit( float competence, float roll01 );
	public static float PuppetCompetence() => 1f;
}
```

- [ ] **Step 1: Write the failing tests (editor menu)**

Create `skyneet-survivors/Editor/SkyNeetLogicTests.cs`:

```csharp
public static class SkyNeetLogicTests
{
	[Menu( "Editor", "SkyNeet/Run Logic Tests" )]
	public static void Run()
	{
		Expect( "high lethality is stupid", CompetenceRules.SpawnCompetence( 0.9f ) < 0.2f );
		Expect( "low lethality aims", CompetenceRules.SpawnCompetence( 0.1f ) > 0.8f );
		Expect( "clamp low", CompetenceRules.SpawnCompetence( 2f ) <= CompetenceRules.Max );
		Expect( "hit", CompetenceRules.RollHit( 1f, 0.5f ) );
		Expect( "miss", !CompetenceRules.RollHit( 0.1f, 0.9f ) );
		Expect( "puppet is 1", CompetenceRules.PuppetCompetence() == 1f );
		EditorUtility.DisplayDialog( "SkyNeet tests", "PASS" );
	}

	static void Expect( string name, bool ok )
	{
		if ( !ok )
			throw new System.Exception( "FAIL: " + name );
	}
}
```

This will fail to compile until `CompetenceRules` exists.

- [ ] **Step 2: Confirm compile fails**

MCP `compile_status` — error: `CompetenceRules` not found.

- [ ] **Step 3: Implement CompetenceRules**

Create `skyneet-survivors/Code/SkyNeet/CompetenceRules.cs`:

```csharp
public static class CompetenceRules
{
	public const float Min = 0.05f;
	public const float Max = 0.95f;

	public static float SpawnCompetence( float lethality )
	{
		return Math.Clamp( 1f - lethality, Min, Max );
	}

	public static bool RollHit( float competence, float roll01 )
	{
		return roll01 <= competence;
	}

	public static float PuppetCompetence() => 1f;
}
```

In `Goliath.OnStart` replace the clamp line with `CompetenceRules.SpawnCompetence( Lethality )`. In attack roll use `CompetenceRules.RollHit( Competence, Game.Random.Float( 0f, 1f ) )`. In `SetPuppeted(true)` set `Competence = CompetenceRules.PuppetCompetence()`.

- [ ] **Step 4: Run tests**

Editor menu **SkyNeet → Run Logic Tests**. Dialog **PASS**. Compile 0 errors.

- [ ] **Step 5: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/CompetenceRules.cs skyneet-survivors/Editor/SkyNeetLogicTests.cs skyneet-survivors/Code/SkyNeet/Goliath.cs
git commit -m "feat: extract CompetenceRules and editor logic tests"
```

---

### Task 3: Readable cavern (WorldFactory)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/WorldFactory.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/OperationDirector.cs` — `EnsureWorld` becomes `WorldFactory.Build( this )`
- Do not change competence math (Task 2 owns Goliath combat)

**Interfaces:**
- Consumes: `OperationDirector` instance, `CampaignSave LastSave`
- Produces: `WorldFactory.Build( OperationDirector director )` spawns named objects with distinct tints

- [ ] **Step 1: Failing check**

Play now: every spawned prop is the same `models/dev/box.vmdl` default tint. Screenshot should not be able to tell NNN from extract. That is the fail.

- [ ] **Step 2: Add WorldFactory**

```csharp
public static class WorldFactory
{
	public static void Build( OperationDirector director )
	{
		Spawn( "NeetNetNode", new Vector3( 180, 0, 32 ), new Color( 0.2f, 0.9f, 1f ), go => go.Components.Create<NeetNetNode>() );
		Spawn( "Extract", new Vector3( -220, 0, 32 ), new Color( 0.2f, 1f, 0.3f ), go => go.Components.Create<ExtractZone>() );
		Spawn( "Scrap A", new Vector3( 80, 120, 16 ), new Color( 0.8f, 0.55f, 0.1f ), go => go.Components.Create<ScrapPile>().Amount = 15 );
		Spawn( "Scrap B", new Vector3( -80, 140, 16 ), new Color( 0.8f, 0.55f, 0.1f ), go => go.Components.Create<ScrapPile>().Amount = 15 );
		Spawn( "Scrap C", new Vector3( 40, -160, 16 ), new Color( 0.8f, 0.55f, 0.1f ), go => go.Components.Create<ScrapPile>().Amount = 20 );
		Spawn( "Fort Ghost", new Vector3( 0, -80, 16 ), new Color( 1f, 1f, 1f, 0.35f ), go => go.Components.Create<FortGhost>().ScrapCost = 20 );
		Spawn( "Goliath", new Vector3( 300, 200, 40 ), new Color( 0.7f, 0.1f, 0.1f ), go => { var g = go.Components.Create<Goliath>(); g.Lethality = 0.85f; } );
		Spawn( "Sancient", new Vector3( 400, 400, 40 ), new Color( 0.6f, 0.2f, 1f ), go => go.Components.Create<SancientDirector>() );
	}

	static void Spawn( string name, Vector3 pos, Color tint, Action<GameObject> setup )
	{
		var go = new GameObject( true, name );
		go.WorldPosition = pos;
		var renderer = go.Components.Create<ModelRenderer>();
		renderer.Model = Model.Load( "models/dev/box.vmdl" );
		renderer.Tint = tint;
		setup( go );
	}
}
```

Replace `OperationDirector.EnsureWorld` spawn block with `WorldFactory.Build( this );` Keep attaching `NeetHealth`, `AutoNeedler`, HUD on the player in the director.

- [ ] **Step 3: Play + screenshot**

MCP `play_start`, `camera_screenshot`. Cyan = NNN, green = extract, orange = scrap, pale = fort, red = Goliath, purple = Sancient.

- [ ] **Step 4: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/WorldFactory.cs skyneet-survivors/Code/SkyNeet/OperationDirector.cs
git commit -m "feat: WorldFactory tints so the cavern is readable"
```

---

### Task 4: NNN is the button

**Files:**
- Modify: `skyneet-survivors/Code/SkyNeet/NeetNetNode.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/OperationHud.cs` (show DREAD line when in range and not planted)
- Modify: `skyneet-survivors/Code/SkyNeet/Goliath.cs` — must not chase/attack unless `director.NodeUp`

**Interfaces:**
- Consumes: `OperationDirector.PlantNode()`, `NodeUp`
- Produces: Goliaths idle (enabled, no move) until NNN; HUD says `HOLD E — WAKE THE HOLE` within 80u of node

- [ ] **Step 1: Prove dark is safe**

Play without pressing E. Goliath must not close distance. If it walks, that is the fail this task fixes.

- [ ] **Step 2: Gate Goliath on NodeUp**

Keep the existing `if ( !director.NodeUp ) return;` as the first combat/move guard. Add `Log.Info` once: `[SkyNeet] Goliath dormant (dark)`.

- [ ] **Step 3: HUD dread**

In `OperationHud.OnUpdate`, if player within 80u of a `NeetNetNode` with `!Planted`, append `\nHOLD E — WAKE THE HOLE`.

- [ ] **Step 4: Play**

Dark: walk around Goliath, no damage. E on cyan box: log `[SkyNeet] NNN online`. Goliath starts missing shots. Compile 0 errors.

- [ ] **Step 5: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/NeetNetNode.cs skyneet-survivors/Code/SkyNeet/OperationHud.cs skyneet-survivors/Code/SkyNeet/Goliath.cs
git commit -m "feat: NNN is the button; Goliaths sleep in the dark"
```

---

### Task 5: Occupancy you can see

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/CampaignStore.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/OperationDirector.cs` load/save through CampaignStore
- Modify: `skyneet-survivors/Code/SkyNeet/WorldFactory.cs` — if `LastSave.Owner == "Occupied"` and `FortStanding`, spawn a solid fort tinted enemy-red, skip ghost

**Interfaces:**
- Consumes: `CampaignSave`, `OperationDirector.SaveFile` (`"skyneet_site.json"`), `SiteId` (`"cavern_0"`)
- Produces:

```csharp
public static class CampaignStore
{
	public static CampaignSave Load( string path );
	public static void Write( string path, CampaignSave save );
}
```

`OperationDirector.EndOperation` rules stay:

- `extract` && !NodeUp → Owner `Neet`
- (`death` or `clock`) && NodeUp → Owner `Occupied`
- `extract` && NodeUp → Owner `Neet`

- [ ] **Step 1: Move IO**

```csharp
public static class CampaignStore
{
	public static CampaignSave Load( string path )
	{
		try
		{
			if ( FileSystem.Data.FileExists( path ) )
			{
				var save = Json.Deserialize<CampaignSave>( FileSystem.Data.ReadAllText( path ) );
				if ( save is not null ) return save;
			}
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Save load failed: {e.Message}" );
		}
		return new CampaignSave();
	}

	public static void Write( string path, CampaignSave save )
	{
		FileSystem.Data.WriteAllText( path, Json.Serialize( save ) );
	}
}
```

Director `LoadSave`/`WriteSave` call these.

- [ ] **Step 2: Occupied spawn**

If `director.LastSave.Owner == "Occupied"`: spawn fort at `(0,-80,16)` with `FortGhost.Solid = true` (or a new `OccupiedFort` tag) tint `Color.Red`. Do not spawn a cheap ghost the player still owns.

- [ ] **Step 3: Roundtrip test**

1. Play, plant NNN, die or wait (or temporarily set `StormSeconds = 10` on the SkyNeet object, then revert).
2. Stop. Play again.
3. Log must contain `Owner from last run: Occupied`.
4. Red fort present. HUD OWNER Occupied.

- [ ] **Step 4: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/CampaignStore.cs skyneet-survivors/Code/SkyNeet/OperationDirector.cs skyneet-survivors/Code/SkyNeet/WorldFactory.cs
git commit -m "feat: visible occupancy so the lost field stays someone else's"
```

---

### Task 6: Sancient reversal

**Files:**
- Modify: `skyneet-survivors/Code/SkyNeet/SancientDirector.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/Goliath.cs` (tint while puppeted)
- Modify: `skyneet-survivors/Code/SkyNeet/OperationHud.cs`

**Interfaces:**
- Consumes: `CompetenceRules.PuppetCompetence()`, `OperationDirector.Loudness`, `NodeUp`, `TimeLeft`
- Produces: while `WindowOpen`, Goliath tint yellow-white, HUD `THEY REMEMBER`; on close, revert tint and competence

- [ ] **Step 1: Faster window for playtest**

Set `LoudnessThreshold = 8f` and `EarliestTimeRemaining` to `14f * 60f` so a 15-minute storm can fire after ~1 minute of NNN (loudness ticks 0.15/s plus plant +25). Keep values as `[Property]` so design can retune without a code edit.

- [ ] **Step 2: Visual + log**

On open: `Log.Info( "[SkyNeet] Sancient on the net. They remember." )` (already exists). Set each Goliath `ModelRenderer.Tint = Color.Yellow`. On close: tint back to `(0.7, 0.1, 0.1)`.

- [ ] **Step 3: Play**

Plant NNN, wait. HUD shows SANCIENT / THEY REMEMBER. Goliath hits become frequent. Window ends, misses return. Compile 0 errors.

- [ ] **Step 4: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/SancientDirector.cs skyneet-survivors/Code/SkyNeet/Goliath.cs skyneet-survivors/Code/SkyNeet/OperationHud.cs
git commit -m "feat: Sancient window is a visible competence reversal"
```

---

### Task 7: Extract, clock, death logs

**Files:**
- Modify: `skyneet-survivors/Code/SkyNeet/ExtractZone.cs`
- Modify: `skyneet-survivors/Code/SkyNeet/OperationDirector.cs` (`EndOperation` only)

**Interfaces:**
- Consumes: `EndOperation( string reason )` reasons `"extract" | "clock" | "death"`
- Produces: HUD `ENDED extract|clock|death`; save written once; player movement ignored after end (director already stops sim)

- [ ] **Step 1: Extract hold feedback**

`ExtractZone`: if player in radius, HUD/log every 0.5s `[SkyNeet] extracting {hold}/{HoldSeconds}`. Green box is the pad.

- [ ] **Step 2: Freeze after end**

In `OperationDirector.OnFixedUpdate`, existing `OperationEnded` return stays. Also set `Time.Delta` consumers to no-op. Do not load a new scene.

- [ ] **Step 3: Play three ends**

1. Walk to green, hold 2s → `ENDED extract`, owner Neet if dark.
2. New run, plant NNN, let Goliath kill → `ENDED death`, owner Occupied.
3. Optional: `StormSeconds = 8` once → `ENDED clock`.

- [ ] **Step 4: Commit**

```bash
git add skyneet-survivors/Code/SkyNeet/ExtractZone.cs skyneet-survivors/Code/SkyNeet/OperationDirector.cs
git commit -m "feat: extract hold feedback and clear operation end reasons"
```

---

### Task 8: Slice playtest gate (no code unless broken)

**Files:** none unless a pillar fails.

**Interfaces:** none.

- [ ] **Step 1: Play a full dark extract** (greed + leave)
- [ ] **Step 2: Play plant NNN, raise fort, get hit, die** (dread, power, panic, consequence)
- [ ] **Step 3: Play the Occupied follow-up drop**
- [ ] **Step 4: MCP screenshot each**
- [ ] **Step 5: Write 5 lines in the PR/commit message: which pillars landed.** If a pillar is missing, file a fix task — do not start Phase 2.

```bash
git commit --allow-empty -m "test: slice playtest gate (greed/dread/power/panic/consequence)"
```

---

### Task 9: Campaign graph (v1)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/CampaignGraph.cs`
- Modify: `CampaignSave` / `CampaignStore` to store `List<SiteRecord>`
- Create: `skyneet-survivors/Assets/scenes/board.scene` (empty + `CampaignBoard` component)

**Interfaces:**

```csharp
public sealed class SiteRecord
{
	public string Id { get; set; }
	public string Owner { get; set; } = "Neet";
	public bool NodeUp { get; set; }
	public int Stockpile { get; set; }
}

public sealed class CampaignGraph
{
	public List<SiteRecord> Sites { get; set; } = new();
	public static CampaignGraph DemoThreeSites();
}
```

`DemoThreeSites()` returns `cavern_0`, `cavern_1`, `cavern_2` all Owner Neet.

- [x] **Step 1: Types + store path `skyneet_campaign.json`** — `CampaignGraph.cs`, `CampaignStore.LoadGraph/WriteGraph`
- [x] **Step 2: Logic test** — in `SkyNeet/Run Logic Tests`, incl. a JSON round trip
- [x] **Step 3: OperationDirector reads `SiteId` from `CampaignSession.SelectedSiteId`** — `CampaignSession.cs` created here rather than in Task 11, which needed it first; Task 11 adds the storm length to it
- [x] **Step 4: Commit** `feat: campaign graph with three sites`

Not done here: `Assets/scenes/board.scene`, listed in this task's Files but in none of its
steps. It needs `CampaignBoard`, which is Task 12 — an empty scene referencing a component
that does not exist yet would break the editor. Task 12 creates both.

Two deviations from the sketch above, both to avoid regressing Task 5:
`SiteRecord` carries `FortStanding` (without it a re-drop forgets the red Occupied fort),
and the graph — not the old single-site file — is now the source of truth. A pre-Task-9
`skyneet_site.json` is folded in once on first load, and the writer for that dead shape was
removed so nothing can quietly write it again.

---

### Task 10: Occupier walks to NNN

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/Occupier.cs`
- Modify: `WorldFactory.cs`

**Interfaces:**

```csharp
public sealed class Occupier : Component
{
	[Property] public float Speed { get; set; } = 70f;
	// OnFixedUpdate: if director.NodeUp && !director.OperationEnded, move toward NeetNetNode; if < 60u, EndOperation("occupied")
}
```

`EndOperation("occupied")` sets Owner `Occupied` like death.

- [ ] **Step 1: Add reason `"occupied"` to director save rules**
- [ ] **Step 2: Spawn Occupier only if LastSave.Owner != "Occupied"` (they already own it)
- [ ] **Step 3: Play, plant NNN, occupier reaches node, HUD ENDED occupied**
- [ ] **Step 4: Commit** `feat: occupier claims a lit NNN`

---

### Task 11: Storm length 15/30/45/60

**Files:**
- Modify: `OperationDirector.StormSeconds` already `[Property]`
- Create: `skyneet-survivors/Code/SkyNeet/CampaignSession.cs`

```csharp
public static class CampaignSession
{
	public static string SelectedSiteId { get; set; } = "cavern_0";
	public static float SelectedStormSeconds { get; set; } = 15f * 60f;
}
```

Director `OnStart`: `StormSeconds = CampaignSession.SelectedStormSeconds`.

Editor menu `SkyNeet/Storm/15` … `60` sets the static and logs it.

- [ ] **Step 1: CampaignSession statics**
- [ ] **Step 2: Four editor menus**
- [ ] **Step 3: Play 15 vs 60 — HUD clock matches**
- [ ] **Step 4: Commit** `feat: selectable storm duration`

---

### Task 12: Board pick site

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/CampaignBoard.cs` (PanelComponent, C# labels, click via buttons)
- Create: `skyneet-survivors/Assets/scenes/board.scene` with SkyNeet board object
- Modify: `skyneet_survivors.sbproj` — do **not** change StartupScene yet; add editor menu `SkyNeet/Open Board` that `Scene.Load( "scenes/board.scene" )` **or** document Play from cavern only if scene load API differs. Prefer: board is a GameObject overlay in cavern, not a scene swap, if `Scene.Load` is awkward.

**Interfaces:** Board lists `CampaignGraph.Sites`; Use/click sets `CampaignSession.SelectedSiteId` and loads cavern (or disables overlay).

- [ ] **Step 1: Overlay in cavern** (safer than scene swap): `CampaignBoard` on a ScreenPanel, hidden by default, `Menu` action toggles
- [ ] **Step 2: Three site buttons**
- [ ] **Step 3: Selecting cavern_1 then Play next drop logs `Drop into cavern_1`**
- [ ] **Step 4: Commit** `feat: campaign board overlay to pick a site`

---

### Task 13: CameraRig (after v1)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/CameraRig.cs`
- Modify: `CustomTopDownController` to **not** write camera if `CameraRig` present

**Interfaces:**

```csharp
public enum SkyNeetView { TopDown, Third, First }

public sealed class CameraRig : Component
{
	[Property] public SkyNeetView View { get; set; } = SkyNeetView.TopDown;
	[Property] public bool AllowThird { get; set; }
	[Property] public bool AllowFirst { get; set; }
}
```

Slice/v1: `AllowThird`/`AllowFirst` false. `Input.Pressed( "View" )` cycles only unlocked views. Top-down pose stays `Up * 1024 + Backward * 256`, `Angles( 75, 0, 0 )`.

- [ ] **Step 1: CameraRig drives `Scene.Camera` in `OnPreRender`**
- [ ] **Step 2: CustomTopDownController skips camera if CameraRig exists**
- [ ] **Step 3: Play — View key does nothing extra until flags true**
- [ ] **Step 4: Commit** `feat: CameraRig with top-down default`

---

### Task 14: Dual-res building (after 13)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/BuildPiece.cs`
- Create: `skyneet-survivors/Code/SkyNeet/BuildModule.cs`
- Modify: `FortGhost.cs` to be a `BuildModule` of one piece

One snap grid, host-only place. Top-down stamps module; first-person places piece **only if** `CameraRig.View == First` and `AllowFirst`. Until then, keep module stamp.

- [ ] **Step 1: Grid snap `MathF.Round( x / 32 ) * 32`**
- [ ] **Step 2: Module contains piece list**
- [ ] **Step 3: Play stamp still costs scrap and is loud**
- [ ] **Step 4: Commit** `feat: snap-grid build module`

---

### Task 15: Blueprint sandbox (after 14)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/BlueprintResource.cs` (`GameResource`, extension `skbprint`)
- Create: `skyneet-survivors/Assets/scenes/blueprint_sandbox.scene`
- Create: `skyneet-survivors/Code/SkyNeet/BlueprintSandbox.cs`

Sandbox: no storm, no save to campaign. Save a `BlueprintResource` with one module transform. Operation: placing it stamps ghosts (existing FortGhost).

- [ ] **Step 1: GameResource type**
- [ ] **Step 2: Sandbox scene + menu `SkyNeet/Blueprint Sandbox`**
- [ ] **Step 3: Save/load one blueprint, stamp in cavern as ghost**
- [ ] **Step 4: Commit** `feat: blueprint resource and sandbox scene`

---

### Task 16: Feats stub (after 12)

**Files:**
- Create: `skyneet-survivors/Code/SkyNeet/FeatRecord.cs`
- Create: `skyneet-survivors/Code/SkyNeet/FeatTracker.cs`

```csharp
public sealed class FeatRecord
{
	public string Id { get; set; }
	public string Track { get; set; } // sprint|hold|ghost
}

public sealed class FeatTracker : Component
{
	public void Consider( OperationDirector d )
	{
		if ( d.EndReason == "extract" && !d.NodeUp )
			Unlock( "ghost_extract" );
		if ( d.EndReason == "clock" && d.NodeUp )
			Unlock( "hold_clear" );
	}
}
```

Call from `EndOperation`. Persist list in `FileSystem.Data` `skyneet_feats.json`. HUD prints unlocked id once.

- [ ] **Step 1: Tracker + save**
- [ ] **Step 2: Two feats only**
- [ ] **Step 3: Dark extract unlocks ghost_extract**
- [ ] **Step 4: Commit** `feat: two accomplishment feats on operation end`

---

## Later (own plans, do not implement in this file’s tasks)

Named so they are not TBDs: Pyron Chrome bubble; arming Nobots; biome procedural depth; flavor mechanics; leaderboards/`Sandbox.Services`; warfront tick. Each gets a new plan under `docs/superpowers/plans/` after Task 8 says the slice is fun.

## Spec coverage

| Spec | Tasks |
|---|---|
| Causal chain / NNN button | 4, 8 |
| Inverse competence / Sancient | 2, 6 |
| Scrap only | 3, 4 |
| Occupancy / someone else’s field | 5, 10 |
| 15 min slice cavern | 1, 7, 8 |
| Top-down first | 1, 13 |
| Campaign graph / durations / board | 9, 11, 12 |
| Dual-res / blueprints | 14, 15 |
| Feats | 16 |
| Host authority / MCP / ident | Global + 1 |
| No razor HUD | Global + 1 |
| Pyron Chrome, diplomacy, extra currencies | Explicitly later |

## Placeholder scan

No TBD/TODO in task steps. Ident is `skyneet_survivors`. HUD API is `AddChild<Label>()`. Sync is `SyncFlags.FromHost`.
