public sealed class ScrapPile : Component
{
	[Property] public int Amount { get; set; } = 10;
	[Property] public float Radius { get; set; } = 60f;
	[HostSync] public bool Taken { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( IsProxy || Taken )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( director is null || player is null )
			return;

		if ( player.WorldPosition.Distance( WorldPosition ) > Radius )
			return;

		Taken = true;
		director.AddScrap( Amount );
		GameObject.Enabled = false;
	}
}
