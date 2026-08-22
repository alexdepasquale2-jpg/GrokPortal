/// <summary>
/// The button. Planting this wakes the hole.
/// Cyan while dark; brighter once the net is up so the player can see what they caused.
/// </summary>
public sealed class NeetNetNode : Component
{
	[Property] public float UseRange { get; set; } = 80f;
	[Sync( SyncFlags.FromHost )] public bool Planted { get; set; }

	static readonly Color DarkTint = new Color( 0.2f, 0.9f, 1f );
	static readonly Color LitTint = new Color( 0.7f, 1f, 1f );

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
		return !Planted && worldPos.Distance( WorldPosition ) <= UseRange;
	}

	protected override void OnUpdate()
	{
		if ( IsProxy || Planted )
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
		Director?.PlantNode();
	}

	void ApplyTint()
	{
		var renderer = Components.Get<ModelRenderer>();
		if ( renderer is null )
			return;

		renderer.Tint = Planted ? LitTint : DarkTint;
	}
}
