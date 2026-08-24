# SkyNeet Survivors

s&box game. Open **this folder** in the s&box editor (`File → Open Project`).

- Ident: `local` / `skyneet_survivors` (`local.skyneet_survivors`)
- Startup scene: `Assets/scenes/cavern.scene`
- Editor: `C:\Program Files (x86)\Steam\steamapps\common\sbox\sbox-dev.exe`

Play: WASD move, E plants **NNN** or raises the fort ghost (costs scrap), stand on the extract pad in the dark to leave. Scrap piles are the boxes around the plane. Lighting NNN wakes the Goliath. Make enough noise and a **Sancient** window makes it hit.

Occupancy: die or timeout with NNN up and the next drop the site is **Occupied**.

## Snappy loop

This is the fastest iterate path. One operation scene; data picks the hole.

| You want | You do | You see |
|---|---|---|
| Change a rule | Edit C#, save | Editor hot-reloads. Still in play. |
| Move scrap / add a Goliath | Edit `Assets/levels/{id}.json` (or the matching method in `LevelCatalog`) | DEV **6** restamps, or stop and Play |
| New hole | **SkyNeet → New Level** | Next Play drops into `cavern_N`. Copy the log JSON into `Assets/levels` to keep it in git |
| Skip the 15-minute wait | DEV keys **1–9** (HUD lists them) | Scrap, NNN, Sancient, extract, next site, reset, now |
| Task 8 playtest | **SkyNeet → Dev Loop Off** | Slot keys stop cheating |

Default drop is `cavern_0` (slice). `dev_60s` is a one-minute iterate hole. Cycle sites with **SkyNeet → Cycle Drop Site** or DEV **7**.

JSON wins when it mounts; C# builtins in `LevelCatalog` keep the game playable if it does not.
