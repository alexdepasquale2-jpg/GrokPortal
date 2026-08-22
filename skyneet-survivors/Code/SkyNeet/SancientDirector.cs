/// <summary>
/// Signature enemy: personally weak, makes Goliaths remember the catalog.
/// Window is a [Property] so design can retune without a code edit.
/// Tint lives here so Task 6 does not fight Grok's CompetenceRules work on Goliath.cs.
/// </summary>
public sealed class SancientDirector : Component
{
	[Property] public float LoudnessThreshold { get; set; } = 8f;
	[Property] public float WindowSeconds { get; set; } = 25f;
	[Property] public float EarliestTimeRemaining { get; set; } = 14f * 60f;

	[Sync( SyncFlags.FromHost )] public bool WindowOpen { get; set; }
	float _windowLeft;
	bool _fired;

	/// <summary>Idle Goliath tint — same numbers WorldFactory uses. Duplicated so this file compiles without that type.</summary>
	public static readonly Color IdleGoliathTint = new Color( 0.7f, 0.1f, 0.1f );

	public static readonly Color PuppetTint = Color.Yellow;

	protected override void OnStart()
	{
		var renderer = Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Tint = new Color( 0.6f, 0.2f, 1f );
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
			Tint( g, PuppetTint );
		}
		Log.Info( "[SkyNeet] Sancient on the net. They remember." );
	}

	void CloseWindow( OperationDirector director )
	{
		WindowOpen = false;
		director.SancientActive = false;
		foreach ( var g in Scene.GetAllComponents<Goliath>() )
		{
			g.SetPuppeted( false );
			Tint( g, IdleGoliathTint );
		}
		Log.Info( "[SkyNeet] Sancient window closed. The catalog sleeps again." );
	}

	static void Tint( Goliath goliath, Color tint )
	{
		var renderer = goliath.Components.Get<ModelRenderer>();
		if ( renderer is not null )
			renderer.Tint = tint;
	}
}
