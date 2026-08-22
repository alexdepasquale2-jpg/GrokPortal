# Final integration review — all branches merged

**Branch:** `claude/grok-build-env-setup-rkk3ac` — contains every commit from
`cursor/competence-rules-b294`, `cursor/slice-nnn-sancient-b294` and
`feat/skyneet-survivors-design`. `git rev-list HEAD..<each>` is **0** for all three.
Four merges this session, zero conflicts.

Reviewed by reading the merged tree, not the individual diffs — these are problems that
only exist once the branches are combined.

## 1. Blocking: Goliaths ignore the colliders, so the fort protects nobody

`FortGhost` and `OccupiedSite` now build a `BoxCollider` (spec §4: built pieces block).
But `Goliath.OnFixedUpdate` does not move through physics — it writes its transform
directly:

```csharp
WorldPosition += (dir * MoveSpeed + weave) * Time.Delta;
WorldPosition = WorldPosition.WithZ( 40f );
```

A transform write ignores colliders. So the fort blocks the **player**, who moves through
`PlayerController` physics, and does **not** block the Goliaths it exists to keep out.
That is backwards: the wall you spend scrap on obstructs you and nobody else.

The **power** pillar therefore still does not land, even with the solidity fix — Task 8
would judge it on a fort that is decorative against the only threat in the slice.

Not fixed here. `Goliath.cs` is Cursor's file, and the fix is a real design decision, not a
patch: either move the Goliath through a physics body / character controller, or make it
path around blockers (spec §4 lists `NavMeshArea` as the assumed pathing blocker). Whoever
takes it should treat it as its own task.

## 2. One unproven API shared by two files

`NeetNetNode.ApplyTint` and `SancientDirector` call bare `Components.Get<ModelRenderer>()`
from inside a component. Everywhere else in the codebase names the owner explicitly —
`go.Components`, `player.Components`, `GameObject.Components`. If `Component.Components`
is not a real shortcut in this s&box version, both files fail together. Cheap insurance:
change both to `GameObject.Components.Get<ModelRenderer>()`, which is proven by the
existing spawn code. Cursor's files, so not changed here.

## 3. Editor assembly — see the escalation ladder

Unchanged from the 17:00Z report: an assembly compiles as one unit, so any broken file
under `Editor/` also removes Task 2's `[Menu]` test harness. Delete `SkyNeetPlayMcp.cs`
first (`Game.ActiveScene`, and it lives only there), then `SkyNeetMcp.cs` +
`SkyNeetLogicMcp.cs` (the `[McpToolset]` / `[McpTool.ReadOnly]` syntax). Toolset names
`skyneet`, `skyneet_play`, `skyneet_logic` were checked for collisions after the merge —
distinct.

## 4. Confirmed correct on review

- **Sancient timing.** Storm 900s, `EarliestTimeRemaining` 840, `LoudnessThreshold` 8,
  plant +25. Window opens 60s in for anyone who planted, `_fired` holds it to one window.
  Matches spec §11.
- **Inverse competence.** `SetPuppeted` raises competence and never touches `Lethality`;
  damage stays `8 + Lethality * 42`. Invariant honoured.
- **Dark is safe.** `Goliath` returns before movement and combat while `!NodeUp`, logging
  once.
- **Post-end freeze.** Director, extract, needler, scrap, fort, node and pawn all stop on
  `OperationEnded`. Nothing pretends to work after the run.
- **Tints.** Single source in `SliceTints`; `WorldFactory` declares none. No drift.
- **Host authority.** Every simulating component guards on `IsProxy`.

## 5. Still true, and it is the whole problem

**Nothing in this branch has ever been compiled or played.** Two cloud agents wrote and
cross-reviewed a vertical slice; the one agent with an editor pushed nothing all session.
Every finding above is from reading code, and the API risks stay open until someone runs
`compile_status` on this branch.

Order of operations for whoever gets the editor: compile → fix per the ladder in §3 →
run `SkyNeet / Run Logic Tests` → the play scripts in the sections above → then Task 8.
Wave 4 (Tasks 9–16) stays shut until Alex passes that gate.
