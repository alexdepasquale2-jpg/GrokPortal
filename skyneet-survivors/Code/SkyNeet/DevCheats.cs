/// <summary>
/// Host-only iterate keys. Slot1–9 are unused in the slice (no inventory), so they jump
/// the causal chain instead of waiting 15 minutes. Toggle off via
/// <see cref="CampaignSession.DevLoop"/> for Task 8 playtests.
/// </summary>
public sealed class DevCheats : Component
{
	protected override void OnUpdate()
	{
		if ( IsProxy || !CampaignSession.DevLoop )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null )
			return;

		if ( Input.Pressed( "Slot1" ) )
			GiveScrap( director );
		else if ( Input.Pressed( "Slot2" ) )
			PlantNode( director );
		else if ( Input.Pressed( "Slot3" ) )
			ForceSancient( director );
		else if ( Input.Pressed( "Slot4" ) )
			SkipClock( director );
		else if ( Input.Pressed( "Slot5" ) )
			ExtractNow( director );
		else if ( Input.Pressed( "Slot6" ) )
			ReloadLayout( director );
		else if ( Input.Pressed( "Slot7" ) )
			NextSite( director );
		else if ( Input.Pressed( "Slot8" ) )
			DieNow( director );
		else if ( Input.Pressed( "Slot9" ) )
			director.DevResetHole();
	}

	static void GiveScrap( OperationDirector director )
	{
		director.AddScrap( 50 );
		Log.Info( "[SkyNeet] DEV +50 scrap." );
	}

	static void PlantNode( OperationDirector director )
	{
		var any = false;
		foreach ( var node in director.Scene.GetAllComponents<NeetNetNode>() )
		{
			node.Plant();
			any = true;
		}

		if ( !any )
			director.PlantNode();

		Log.Info( "[SkyNeet] DEV planted NNN." );
	}

	static void ForceSancient( OperationDirector director )
	{
		PlantNode( director );
		var sancient = director.Scene.GetAllComponents<SancientDirector>().FirstOrDefault();
		if ( sancient is null )
		{
			Log.Info( "[SkyNeet] DEV no Sancient to force." );
			return;
		}

		sancient.ForceOpen();
		Log.Info( "[SkyNeet] DEV Sancient forced." );
	}

	static void SkipClock( OperationDirector director )
	{
		if ( director.OperationEnded )
			return;
		director.TimeLeft = 30f;
		Log.Info( "[SkyNeet] DEV clock → 30s." );
	}

	static void ExtractNow( OperationDirector director )
	{
		director.EndOperation( "extract" );
		Log.Info( "[SkyNeet] DEV extract." );
	}

	static void ReloadLayout( OperationDirector director )
	{
		WorldFactory.Rebuild( director );
		Log.Info( $"[SkyNeet] DEV reloaded {OperationDirector.SiteId}." );
	}

	static void NextSite( OperationDirector director )
	{
		var next = LevelCatalog.NextAfter( CampaignSession.SelectedSiteId );
		CampaignSession.SelectedSiteId = next;
		director.DevResetHole();
		Log.Info( $"[SkyNeet] DEV drop into {next}." );
	}

	static void DieNow( OperationDirector director )
	{
		director.EndOperation( "death" );
		Log.Info( "[SkyNeet] DEV death." );
	}
}
