/// <summary>
/// Silent extract. Does not require NNN. Low ping, smaller keep.
/// </summary>
public sealed class ExtractZone : Component
{
	[Property] public float Radius { get; set; } = 70f;
	[Property] public float HoldSeconds { get; set; } = 2f;

	float _hold;

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
			_hold = 0f;
			return;
		}

		_hold += Time.Delta;
		if ( _hold >= HoldSeconds )
			director.EndOperation( "extract" );
	}
}
