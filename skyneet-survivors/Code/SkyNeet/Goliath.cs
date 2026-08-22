/// <summary>
/// Inept Goliath. High lethality, low competence — until a Sancient puppets it.
/// Movement is unchanged: weave by competence, host-only, sleeps while the hole is dark.
/// </summary>
public sealed class Goliath : Component
{
	[Property, Range( 0, 1 )] public float Lethality { get; set; } = 0.8f;
	[Property] public float MoveSpeed { get; set; } = 80f;
	[Property] public float AttackRange { get; set; } = 90f;
	[Property] public float AttackInterval { get; set; } = 1.2f;

	[Property] public float MaxHealth { get; set; } = 80f;
	[Sync( SyncFlags.FromHost )] public float Health { get; set; }
	[Sync( SyncFlags.FromHost )] public float Competence { get; set; }
	[Sync( SyncFlags.FromHost )] public bool Puppeted { get; set; }

	float _attackCd;
	float _baseCompetence;
	bool _loggedDormant;

	protected override void OnStart()
	{
		_baseCompetence = CompetenceRules.SpawnCompetence( Lethality );
		if ( !IsProxy )
		{
			Competence = _baseCompetence;
			Health = MaxHealth;
		}
	}

	public void TakeDamage( float amount )
	{
		if ( IsProxy || Health <= 0f )
			return;
		Health -= amount;
		if ( Health <= 0f )
		{
			Health = 0f;
			Log.Info( "[SkyNeet] Goliath down." );
			GameObject.Enabled = false;
		}
	}

	public void SetPuppeted( bool on )
	{
		if ( IsProxy )
			return;
		Puppeted = on;
		Competence = on ? CompetenceRules.PuppetCompetence() : _baseCompetence;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy )
			return;

		var director = Scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null || director.OperationEnded )
			return;

		if ( !director.NodeUp )
		{
			if ( !_loggedDormant )
			{
				_loggedDormant = true;
				Log.Info( "[SkyNeet] Goliath dormant (dark)" );
			}
			return;
		}

		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		var health = Scene.GetAllComponents<NeetHealth>().FirstOrDefault();
		if ( player is null )
			return;

		var toPlayer = player.WorldPosition - WorldPosition;
		var dist = toPlayer.Length;
		if ( dist > 8f )
		{
			// Inept wander: competence 1 walks straight; low competence weaves.
			var dir = toPlayer.Normal;
			var weave = Vector3.Up.Cross( dir ) * MathF.Sin( Time.Now * 1.7f ) * (1f - Competence) * 40f;
			WorldPosition += (dir * MoveSpeed + weave) * Time.Delta;
			WorldPosition = WorldPosition.WithZ( 40f );
		}

		_attackCd -= Time.Delta;
		if ( dist > AttackRange || _attackCd > 0f || health is null )
			return;

		_attackCd = AttackInterval;

		// Inverse: lethality is damage IF the shot lands. Competence is hit chance.
		var hit = CompetenceRules.RollHit( Competence, Game.Random.Float( 0f, 1f ) );
		if ( !hit )
		{
			Log.Info( "[SkyNeet] Goliath missed. Catalog is still asleep." );
			return;
		}

		var damage = 8f + Lethality * 42f;
		health.Hurt( damage );
		Log.Info( $"[SkyNeet] Goliath hit for {damage:0} (competence {Competence:0.00})." );
	}
}
