public sealed class NeetHealth : Component
{
	[Property] public float MaxHealth { get; set; } = 100f;
	[HostSync] public float Health { get; set; } = 100f;

	public bool IsDead => Health <= 0f;

	protected override void OnStart()
	{
		if ( !IsProxy )
			Health = MaxHealth;
	}

	public void Hurt( float amount )
	{
		if ( IsProxy || IsDead )
			return;

		Health -= amount;
		if ( Health <= 0f )
		{
			Health = 0f;
			Log.Info( "[SkyNeet] Neetmon is down." );
		}
	}
}
