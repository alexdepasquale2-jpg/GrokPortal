/// <summary>
/// Snappy iterate menus. [Menu] only — do not add MCP attributes here.
/// New Level writes a FileSystem.Data override and selects it; Play (or DEV 7/9) drops in.
/// Copy the logged JSON into Assets/levels/{id}.json when you want the hole in git.
/// </summary>
public static class SkyNeetDevLoopMenu
{
	[Menu( "Editor", "SkyNeet/Print Dev Flow" )]
	public static void PrintFlow()
	{
		Log.Info( "[SkyNeet] SNAPPY LOOP" );
		Log.Info( "[SkyNeet] C# save → editor hot reload. Change a number, play, see it." );
		Log.Info( "[SkyNeet] Levels: Assets/levels/{id}.json (JSON wins) or LevelCatalog builtin (fallback)." );
		Log.Info( "[SkyNeet] New Level writes a Data override and selects it. Copy the log JSON into Assets to keep it." );
		Log.Info( "[SkyNeet] DEV keys 1-9: scrap, NNN, Sancient, clock, extract, reload layout, next site, die, reset." );
		Log.Info( "[SkyNeet] Task 8 playtest: SkyNeet / Dev Loop Off." );
		EditorUtility.DisplayDialog( "SkyNeet Dev Flow", "C# hot reload + JSON levels + DEV 1-9. New Level is instant. Details in the log.", "OK" );
	}

	[Menu( "Editor", "SkyNeet/New Level" )]
	public static void NewLevel()
	{
		var id = LevelCatalog.NextNewId();
		var def = LevelCatalog.FromTemplate( id );
		LevelCatalog.WriteOverride( def );

		if ( !CampaignSession.ExtraSiteIds.Contains( id ) )
			CampaignSession.ExtraSiteIds.Add( id );
		CampaignSession.SelectedSiteId = id;

		Log.Info( $"[SkyNeet] New level {id} selected. Play (or DEV 7 then 9) to drop in." );
		Log.Info( "[SkyNeet] To keep it in git, save this as Assets/levels/" + id + ".json :" );
		Log.Info( Json.Serialize( def ) );
		EditorUtility.DisplayDialog( "SkyNeet New Level", "Created " + id + " and selected it. Press Play. JSON is in the log if you want it in git.", "OK" );
	}

	[Menu( "Editor", "SkyNeet/Cycle Drop Site" )]
	public static void CycleDropSite()
	{
		var next = LevelCatalog.NextAfter( CampaignSession.SelectedSiteId );
		CampaignSession.SelectedSiteId = next;
		Log.Info( $"[SkyNeet] Next drop is {next}. Play, or DEV 7 in-session to restamp now." );
		EditorUtility.DisplayDialog( "SkyNeet Drop Site", "Next drop: " + next, "OK" );
	}

	[Menu( "Editor", "SkyNeet/Dev Loop Off" )]
	public static void DevLoopOff()
	{
		CampaignSession.DevLoop = false;
		Log.Info( "[SkyNeet] Dev loop off. Slot keys are inventory again. Task 8 playtest mode." );
		EditorUtility.DisplayDialog( "SkyNeet Dev Loop", "Off. Slot 1-9 will not cheat.", "OK" );
	}

	[Menu( "Editor", "SkyNeet/Dev Loop On" )]
	public static void DevLoopOn()
	{
		CampaignSession.DevLoop = true;
		Log.Info( "[SkyNeet] Dev loop on. Slot 1-9 iterate the causal chain." );
		EditorUtility.DisplayDialog( "SkyNeet Dev Loop", "On. HUD shows the DEV key line.", "OK" );
	}
}
