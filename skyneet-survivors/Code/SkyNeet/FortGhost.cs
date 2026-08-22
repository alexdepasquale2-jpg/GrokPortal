/// <summary>
/// One module ghost. Spend scrap to solidify. Solid is loud.
/// </summary>
public sealed class FortGhost : Component
{
	[Property] public int ScrapCost { get; set; } = 20;
	[Property] public float UseRange { get; set; } = 90f;
	[HostSync] public bool Solid { get; set; }

	protected override void OnUpdate()
	{
		if ( IsProxy || Solid )
			return;

		if ( !Input.Pressed( "Use" ) )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( director is null || player is null )
			return;

		if ( player.WorldPosition.Distance( WorldPosition ) > UseRange )
			return;

		if ( !director.TrySpendScrap( ScrapCost ) )
		{
			Log.Info( $"[SkyNeet] Need {ScrapCost} scrap to raise the fort (have {director.Scrap})." );
			return;
		}

		Solid = true;
		director.NotifyBuild();
		Log.Info( "[SkyNeet] Fort solidified. You just made noise." );
	}
}
