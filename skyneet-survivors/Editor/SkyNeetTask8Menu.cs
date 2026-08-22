/// <summary>
/// Task 8 script in the editor log. Uses only [Menu], so it still exists after Grok
/// deletes the unproven MCP tool files to get the editor assembly green.
/// </summary>
public static class SkyNeetTask8Menu
{
	[Menu( "Editor", "SkyNeet/Print Task 8 Script" )]
	public static void Print()
	{
		Log.Info( "[SkyNeet] TASK 8 — compile claude/grok-build-env-setup-rkk3ac, then three runs." );
		Log.Info( "[SkyNeet] A dark extract: scrap, no E on cyan, Goliath idle, hold green pad, ENDED extract, world frozen." );
		Log.Info( "[SkyNeet] B wake and die: HOLD E, NNN online, miss, buy fort (blocks), THEY REMEMBER, yellow hits, ENDED death/clock Occupied." );
		Log.Info( "[SkyNeet] C next drop: red fort, no ghost, OWNER Occupied." );
		Log.Info( "[SkyNeet] Pillars: greed dread power panic consequence. Missing pillar = fix task, not Wave 4." );
		Log.Info( "[SkyNeet] Full script: docs/superpowers/plans/2026-08-22-skyneet-task8-playtest.md" );
		EditorUtility.DisplayDialog( "SkyNeet Task 8", "Script printed to the log. Three runs: dark extract, wake-and-die, occupied drop.", "OK" );
	}
}
