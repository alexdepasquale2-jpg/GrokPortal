/// <summary>
/// The button. Planting this wakes the hole.
/// Cyan while dark; brighter once the net is up so the player can see what they caused.
/// </summary>
public sealed class NeetNetNode : Component
{
	[Property] public float UseRange { get; set; } = 80f;
	[Sync( SyncFlags.FromHost )] public bool Planted { get; set; }

	OperationDirector Director => Scene.GetAllComponents<OperationDirector>().FirstOrDefault();

	protected override void OnStart()
	{
		ApplyTint();
	}

	/// <summary>
	/// Dread prompt: in range of an unplanted node. HUD uses this so the 80u radius
	/// lives on the node, not copied into the panel.
	/// </summary>
	public bool ShouldPromptPlant( Vector3 worldPos )
	{
		if ( Planted )
			return false;

		var director = Director;
		if ( director is not null && ( director.NodeUp || director.OperationEnded ) )
			return false;

		return worldPos.Distance( WorldPosition ) <= UseRange;
	}

	protected override void OnUpdate()
	{
		if ( IsProxy || Planted )
			return;

		var director = Director;
		if ( director is null || director.OperationEnded )
			return;

		if ( !Input.Pressed( "Use" ) )
			return;

		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( player is null )
			return;

		if ( !ShouldPromptPlant( player.WorldPosition ) )
			return;

		Planted = true;
		ApplyTint();
		director.PlantNode();
	}

	void ApplyTint()
	{
		var renderer = GameObject.Components.Get<ModelRenderer>();
		if ( renderer is null )
			return;

		renderer.Tint = Planted ? SliceTints.NodeLitTint : SliceTints.NodeTint;
	}
}
