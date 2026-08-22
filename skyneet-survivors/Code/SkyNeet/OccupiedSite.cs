/// <summary>
/// A structure that belongs to whoever holds this field now. Marks the fort you raised
/// last run and then lost: it still stands, it is just not yours, and your scrap is in it.
/// Inert on purpose — you cannot use it, which is the whole point.
/// </summary>
public sealed class OccupiedSite : Component
{
	[Property] public string Owner { get; set; } = "Occupied";
	[Property] public int Stockpile { get; set; }

	protected override void OnStart()
	{
		// A standing fort is a built piece, so it blocks - whoever's flag is on it.
		if ( GameObject.Components.Get<BoxCollider>() is null )
			GameObject.Components.Create<BoxCollider>();

		if ( IsProxy )
			return;

		Log.Info( $"[SkyNeet] {Owner} hold this fort. {Stockpile} scrap of yours is inside it." );
	}
}
