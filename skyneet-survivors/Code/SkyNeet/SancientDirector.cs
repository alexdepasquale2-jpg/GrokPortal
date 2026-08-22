/// <summary>
/// Signature enemy: personally weak, makes Goliaths remember the catalog.
/// </summary>
public sealed class SancientDirector : Component
{
	[Property] public float LoudnessThreshold { get; set; } = 28f;
	[Property] public float WindowSeconds { get; set; } = 25f;
	[Property] public float EarliestTimeRemaining { get; set; } = 12f * 60f;

	[Sync( SyncFlags.FromHost )] public bool WindowOpen { get; set; }
	float _windowLeft;
	bool _fired;

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
			g.SetPuppeted( true );
		Log.Info( "[SkyNeet] Sancient on the net. They remember." );
	}

	void CloseWindow( OperationDirector director )
	{
		WindowOpen = false;
		director.SancientActive = false;
		foreach ( var g in Scene.GetAllComponents<Goliath>() )
			g.SetPuppeted( false );
		Log.Info( "[SkyNeet] Sancient window closed. The catalog sleeps again." );
	}
}
