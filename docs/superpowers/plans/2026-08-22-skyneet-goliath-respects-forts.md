# Goliaths respect built forts

**Why:** Claude's merged-tree review (`e5800ee`). `FortGhost`/`OccupiedSite` create `BoxCollider`, but `Goliath` writes `WorldPosition`, so the wall blocks the player and not the only enemy. Task 8 **power** cannot land.

**Choice (slice):** refuse a wish position that overlaps a solid `FortGhost` or any other non-player `BoxCollider`. Goliath stops on the wall. No CharacterController, no NavMesh yet (those stay spec assumptions).

**Not this:** dual-res pathing, nav regen, physics bodies on every scrap pile.

## Files

- Create: `skyneet-survivors/Code/SkyNeet/FortBlock.cs`
- Modify: `Goliath.cs` move block only (competence math unchanged)

## Steps

- [x] **Step 1:** `FortBlock.Hits( scene, self, wish )` — solid ghosts + BoxColliders, skip self and player
- [x] **Step 2:** Goliath applies wish only when `!Hits`
- [ ] **Step 3 (Grok):** buy the fort, kite the Goliath into it. It must stop. You can still walk around. Occupied red fort must also stop it on the follow-up drop.
- [ ] **Step 4:** If radius 48 is wrong vs `models/dev/box.vmdl`, retune `FortBlock.DefaultRadius` only.

Later (after Task 8, own PR): replace this with `NavMeshArea` or a character mover if the editor makes that the better path. Keep the invariant (built pieces block **everyone**).
