/// <summary>
/// Signature enemy: personally weak, makes Goliaths remember the catalog.
/// Window is a [Property] so design can retune without a code edit.
/// Tint live-updates go through SliceTints so they cannot drift from spawn.
/// </summary>
public sealed class SancientDirector : Component
{
	[Property] public float LoudnessThreshold { get; set; } = 8f;
	[Property] public float WindowSeconds { get; set; } = 25f;
	[Property] public float EarliestTimeRemaining { get; set; } = 14f * 60f;

	[Sync( SyncFlags.FromHost )] public bool WindowOpen { get; set; }
	float _windowLeft;
	bool _fired;

	protected override void OnStart()
	{
		var renderer = GameObject.Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Tint = SliceTints.SancientTint;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null || director.OperationEnded )
			return;

		if ( WindowOpen )
		{
			_windowLeft -= Time.Delta;
			if ( _windowLeft <= 0f )
				CloseWindow( director );
			return;
		}

		if ( _fired || !director.NodeUp )
			return;

		if ( director.Loudness < LoudnessThreshold )
			return;

		if ( director.TimeLeft > EarliestTimeRemaining )
			return;

		OpenWindow( director );
	}

	void OpenWindow( OperationDirector director )
	{
		_fired = true;
		WindowOpen = true;
		_windowLeft = WindowSeconds;
		director.SancientActive = true;
		foreach ( var g in Scene.GetAllComponents<Goliath>() )
		{
			g.SetPuppeted( true );
			Tint( g, SliceTints.GoliathPuppetTint );
		}
		Log.Info( "[SkyNeet] Sancient on the net. They remember." );
	}

	/// <summary>DEV: open the window now. Plants NNN if the hole is still dark, because Goliaths sleep until then.</summary>
	public void ForceOpen()
	{
		if ( IsProxy || WindowOpen )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null || director.OperationEnded )
			return;

		if ( !director.NodeUp )
		{
			foreach ( var node in Scene.GetAllComponents<NeetNetNode>() )
				node.Plant();
			if ( !director.NodeUp )
				director.PlantNode();
		}

		if ( WindowOpen )
			return;

		_fired = false;
		OpenWindow( director );
	}

	void CloseWindow( OperationDirector director )
	{
		WindowOpen = false;
		director.SancientActive = false;
		foreach ( var g in Scene.GetAllComponents<Goliath>() )
		{
			g.SetPuppeted( false );
			Tint( g, SliceTints.GoliathTint );
		}
		Log.Info( "[SkyNeet] Sancient window closed. The catalog sleeps again." );
	}

	static void Tint( Goliath goliath, Color tint )
	{
		var renderer = goliath.GameObject.Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Tint = tint;
	}
}
