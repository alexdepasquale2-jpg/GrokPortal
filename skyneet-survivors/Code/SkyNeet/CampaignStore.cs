/// <summary>
/// Site memory on disk. The backpack dies; this does not.
/// The campaign graph is the source of truth as of Task 9; the single-site CampaignSave
/// shape is still how one operation talks about the field it is standing in.
/// </summary>
public static class CampaignStore
{
	/// <summary>The campaign graph. One file for every site (Task 9).</summary>
	public const string GraphFile = "skyneet_campaign.json";

	/// <summary>
	/// Loads the war. Falls back to the v1 three-site opening, and folds a pre-Task-9
	/// single-site save into it once so an in-progress playthrough is not thrown away.
	/// </summary>
	public static CampaignGraph LoadGraph( string path, string legacySitePath = null )
	{
		try
		{
			if ( FileSystem.Data.FileExists( path ) )
			{
				var graph = Json.Deserialize<CampaignGraph>( FileSystem.Data.ReadAllText( path ) );
				if ( graph is not null && graph.Sites is not null && graph.Sites.Count > 0 )
					return graph;
			}
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Campaign load failed: {e.Message}" );
		}

		var fresh = CampaignGraph.DemoThreeSites();

		if ( !string.IsNullOrEmpty( legacySitePath ) && FileSystem.Data.FileExists( legacySitePath ) )
		{
			var legacy = Load( legacySitePath );
			var record = fresh.Get( legacy.SiteId );
			record.Owner = legacy.Owner;
			record.NodeUp = legacy.NodeUp;
			record.Stockpile = legacy.OccupierScrap;
			record.FortStanding = legacy.FortStanding;
			Log.Info( $"[SkyNeet] Folded the old single-site save into the campaign: {legacy.SiteId} is {legacy.Owner}." );
		}

		return fresh;
	}

	public static void WriteGraph( string path, CampaignGraph graph )
	{
		try
		{
			FileSystem.Data.WriteAllText( path, Json.Serialize( graph ) );
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Campaign write failed: {e.Message}" );
		}
	}

	/// <summary>
	/// Reads the pre-Task-9 single-site file. Private and migration-only: the graph is the
	/// source of truth now, and a public writer for this shape would quietly recreate the
	/// two-sources-of-truth problem the graph exists to remove.
	/// </summary>
	static CampaignSave Load( string path )
	{
		try
		{
			if ( FileSystem.Data.FileExists( path ) )
			{
				var save = Json.Deserialize<CampaignSave>( FileSystem.Data.ReadAllText( path ) );
				if ( save is not null )
					return save;
			}
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Save load failed: {e.Message}" );
		}

		return new CampaignSave();
	}

}
