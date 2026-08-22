/// <summary>
/// Top-down auto-weapon. One gun, fires itself at the nearest Goliath.
/// </summary>
public sealed class AutoNeedler : Component
{
	[Property] public float Range { get; set; } = 420f;
	[Property] public float Damage { get; set; } = 12f;
	[Property] public float Interval { get; set; } = 0.35f;

	float _cd;

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null || director.OperationEnded )
			return;

		_cd -= Time.Delta;
		if ( _cd > 0f )
			return;

		Goliath best = null;
		var bestDist = Range;
		foreach ( var g in Scene.GetAllComponents<Goliath>() )
		{
			if ( !g.GameObject.Enabled )
				continue;
			var d = g.WorldPosition.Distance( WorldPosition );
			if ( d < bestDist )
			{
				bestDist = d;
				best = g;
			}
		}

		if ( best is null )
			return;

		_cd = Interval;
		best.TakeDamage( Damage );
	}
}
