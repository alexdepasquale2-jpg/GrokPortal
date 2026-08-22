/// <summary>
/// The button. Planting this wakes the hole.
/// </summary>
public sealed class NeetNetNode : Component
{
	[Property] public float UseRange { get; set; } = 80f;
	[HostSync] public bool Planted { get; set; }

	OperationDirector Director => Scene.GetAllComponents<OperationDirector>().FirstOrDefault();

	protected override void OnUpdate()
	{
		if ( IsProxy || Planted )
			return;

		if ( !Input.Pressed( "Use" ) )
			return;

		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( player is null )
			return;

		if ( player.WorldPosition.Distance( WorldPosition ) > UseRange )
			return;

		Planted = true;
		Director?.PlantNode();
	}
}
