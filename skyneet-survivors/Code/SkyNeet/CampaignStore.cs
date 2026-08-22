/// <summary>
/// Site memory on disk. The backpack dies; this does not.
/// Kept out of OperationDirector so save shape can grow (campaign graph, per-site records)
/// without the director learning about IO.
/// </summary>
public static class CampaignStore
{
	public static CampaignSave Load( string path )
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

	public static void Write( string path, CampaignSave save )
	{
		try
		{
			FileSystem.Data.WriteAllText( path, Json.Serialize( save ) );
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Save write failed: {e.Message}" );
		}
	}
}
