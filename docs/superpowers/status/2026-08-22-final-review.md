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

---

# Review round 2 (17:35Z) — after Cursor's fixes

Cursor fixed both findings from the review above: `FortBlock` stops Goliaths at solid
forts, and the bare `Components.Get` calls now name their owner. PR #3 also merged this
branch into `feat/skyneet-survivors-design`. All branches re-merged here, still zero
conflicts.

`FortBlock` gets the invariant right where it matters: it skips `FortGhost` instances whose
`Solid` is false, so an unbought ghost still does not block. That is spec §4 honoured.

## Confirmed bug: the player is an invisible wall to Goliaths

`FortBlock.Hits` sweeps every `BoxCollider` in the scene and skips the ones belonging to
the player:

```csharp
if ( go.Components.Get<PlayerController>() is not null )
    continue;
```

`go` is the GameObject that *owns the collider*. In `cavern.scene` the player's colliders
are not on the player object — they are on a child:

```
Player Controller   [Rigidbody, PlayerController, CustomTopDownController, ...]
└── Colliders       [CapsuleCollider, BoxCollider]      ← the BoxCollider is here
```

`Components.Get<T>()` looks in self, so on the `Colliders` child it finds no
`PlayerController`, the skip never fires, and **the player's own collider is treated as a
wall**. Goliaths refuse any wish position within `DefaultRadius` (48u) of the player and
halt at that standoff forever.

They can still shoot — `AttackRange` is 90u — so the slice will not look obviously broken.
It will look like Goliaths that stop dead at an invisible ring and plink from range, which
reads as broken AI and takes dread and panic with it.

**Fix (Cursor's file, not changed here):** test the ancestor chain rather than the collider's
own object. Using only APIs already proven in this codebase:

```csharp
static bool BelongsTo<T>( GameObject go ) where T : Component
{
    for ( var g = go; g.IsValid(); g = g.Parent )
        if ( g.Components.Get<T>() is not null )
            return true;
    return false;
}
```

then `if ( BelongsTo<PlayerController>( go ) ) continue;`.

The `Goliath` skip in the same loop is dead code — `WorldFactory` never gives a Goliath a
`BoxCollider`, so no Goliath is ever in that sweep. Harmless, but it is not doing what it
looks like it does, and it will start mattering the moment a Goliath gets a collider.

`Plane` carries a `PlaneCollider`, not a `BoxCollider`, so the floor is correctly ignored.
The player's collider is the only unintended blocker in the scene.

## Editor assembly is now five files

`SkyNeetTask8Menu.cs` joins the four already there. The escalation ladder in §3 above still
applies unchanged — the assembly compiles as one unit, and every added file is one more way
for Task 2's `[Menu]` harness to be taken down by something unrelated to it.

---

# Round 3 (18:00Z) — PRs merged, lock released, FortBlock fixed

Grok has reached its limits and is not returning. Cursor released its file-ownership lock
(`2026-08-22-cursor-lock.md` deleted). Claude is the only agent left, so the ownership
protocol is retired and the flag-don't-fix rule with it.

PRs #1 and #2 were draft; both were `mergeable_state: clean`, reviewed against the merged
tree, marked ready and merged into `feat/skyneet-survivors-design`. PR #3 was already
merged. **No open PRs remain.**

## FortBlock: fixed, and not the way the earlier note suggested

The confirmed bug stood: `FortBlock.Hits` swept every `BoxCollider` in the scene and tried
to skip the player's by testing the collider's own GameObject for `PlayerController`. In
`cavern.scene` the colliders live on a child (`Colliders`) whose *parent* holds
`PlayerController`, so the skip never fired and the player read as a wall — Goliaths halted
48u out and never closed.

Round 2 proposed walking the ancestor chain. **That is not what was implemented**, because
it keeps the underlying shape: a blacklist over every collider in the scene, where anything
nobody remembered to exclude silently becomes a wall. It also leans on `GameObject.Parent`,
which nothing in this codebase has proven.

`FortBlock` now asks what a thing **is** rather than whether it carries a collider: it
blocks on solidified `FortGhost`s and on `OccupiedSite`s, and on nothing else. That

- removes the player bug by construction — the player is not a built piece, so it can never
  block, no matter where its colliders sit;
- drops the dead `Goliath` skip (no Goliath is ever given a `BoxCollider`);
- states spec §4 directly in code — a ghost you have not bought is still skipped;
- uses only APIs already proven in this repo (`GetAllComponents<T>`, `IsValid()`,
  `WorldPosition`, `WithZ`), where the ancestor walk would have added an unproven one.

The trade: a future built-piece type must be added to `FortBlock`. That is a far smaller
failure mode than scene furniture silently walling off the cavern.

The `BoxCollider`s on forts are still doing their job — they are what stops the *player*,
who moves through real physics. `FortBlock` is only the Goliath's side of the same rule.

## Verification: now entirely on Alex

Every "Grok will run this" instruction in the docs above is void. Nothing in this repository
has ever been compiled or played. The whole slice — five tasks, three agents, ~20 commits —
rests on unverified s&box API assumptions.

First command that matters is still `compile_status`, and it needs the editor on the PC.
Order: compile → editor-assembly ladder in §3 if red → `SkyNeet / Run Logic Tests` →
the play scripts → Task 8. Wave 4 stays shut until that gate passes.
