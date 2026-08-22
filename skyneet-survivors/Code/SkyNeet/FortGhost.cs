/// <summary>
/// One module ghost. Spend scrap to solidify. Solid is loud.
/// Invariant (spec §4): a ghost is not solid and does not block pathing; a built piece does.
/// The collider is therefore created on solidify, not at spawn.
/// </summary>
public sealed class FortGhost : Component
{
	[Property] public int ScrapCost { get; set; } = 20;
	[Property] public float UseRange { get; set; } = 90f;
	[Sync( SyncFlags.FromHost )] public bool Solid { get; set; }

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

		// A fort that blocks nothing is a tinted box you paid for. This is what makes it a wall.
		if ( GameObject.Components.Get<BoxCollider>() is null )
			GameObject.Components.Create<BoxCollider>();

		director.NotifyBuild();
		Log.Info( "[SkyNeet] Fort solidified. It blocks the hole now. You just made noise." );
	}
}
