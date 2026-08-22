/// <summary>
/// Silent extract. Does not require NNN. Low ping, smaller keep.
/// The hold is reported while it runs so leaving is a decision you watch yourself make,
/// not something that happens to you.
/// </summary>
public sealed class ExtractZone : Component
{
	[Property] public float Radius { get; set; } = 70f;
	[Property] public float HoldSeconds { get; set; } = 2f;
	[Property] public float ReportInterval { get; set; } = 0.5f;

	float _hold;
	float _nextReport;

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null || director.OperationEnded )
			return;

		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( player is null )
			return;

		if ( player.WorldPosition.Distance( WorldPosition ) > Radius )
		{
			if ( _hold > 0f )
				Log.Info( "[SkyNeet] Off the pad. Extract reset." );

			_hold = 0f;
			_nextReport = 0f;
			return;
		}

		_hold += Time.Delta;

		if ( _hold >= _nextReport )
		{
			_nextReport = _hold + ReportInterval;
			Log.Info( $"[SkyNeet] extracting {_hold:0.0}/{HoldSeconds:0.0}" );
		}

		if ( _hold >= HoldSeconds )
			director.EndOperation( "extract" );
	}
}
