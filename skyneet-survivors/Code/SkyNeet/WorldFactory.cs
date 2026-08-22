/// <summary>
/// Builds the readable slice cavern. Every prop is the same dev box, so tint is the only
/// thing telling the player what a shape means: cyan NNN, green extract, orange scrap,
/// pale fort, red Goliath, purple Sancient.
/// Spawns are guarded so authored scene objects win over runtime ones.
/// </summary>
public static class WorldFactory
{
	public static readonly Color NodeTint = new Color( 0.2f, 0.9f, 1f );
	public static readonly Color ExtractTint = new Color( 0.2f, 1f, 0.3f );
	public static readonly Color ScrapTint = new Color( 0.8f, 0.55f, 0.1f );
	public static readonly Color FortGhostTint = new Color( 1f, 1f, 1f, 0.35f );
	public static readonly Color GoliathTint = new Color( 0.7f, 0.1f, 0.1f );
	public static readonly Color SancientTint = new Color( 0.6f, 0.2f, 1f );

	/// <summary>Enemy red: the fort is standing, it is just not yours any more.</summary>
	public static readonly Color OccupiedFortTint = Color.Red;

	public static void Build( OperationDirector director )
	{
		if ( director is null )
			return;

		var scene = director.Scene;

		if ( !scene.GetAllComponents<NeetNetNode>().Any() )
			Spawn( "NeetNetNode", new Vector3( 180, 0, 32 ), NodeTint, go => go.Components.Create<NeetNetNode>() );

		if ( !scene.GetAllComponents<ExtractZone>().Any() )
			Spawn( "Extract", new Vector3( -220, 0, 32 ), ExtractTint, go => go.Components.Create<ExtractZone>() );

		if ( !scene.GetAllComponents<ScrapPile>().Any() )
		{
			Spawn( "Scrap A", new Vector3( 80, 120, 16 ), ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 15 );
			Spawn( "Scrap B", new Vector3( -80, 140, 16 ), ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 15 );
			Spawn( "Scrap C", new Vector3( 40, -160, 16 ), ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 20 );
		}

		BuildFort( scene, director.LastSave );

		if ( !scene.GetAllComponents<Goliath>().Any() )
			Spawn( "Goliath", new Vector3( 300, 200, 40 ), GoliathTint, go => go.Components.Create<Goliath>().Lethality = 0.85f );

		if ( !scene.GetAllComponents<SancientDirector>().Any() )
			Spawn( "Sancient", new Vector3( 400, 400, 40 ), SancientTint, go => go.Components.Create<SancientDirector>() );
	}

	/// <summary>
	/// Whoever owns the field owns what is built on it. Lose the site with a fort standing
	/// and you come back to a red one you cannot use, holding the scrap you dropped.
	/// </summary>
	static void BuildFort( Scene scene, CampaignSave save )
	{
		if ( scene.GetAllComponents<FortGhost>().Any() || scene.GetAllComponents<OccupiedSite>().Any() )
			return;

		var fortPos = new Vector3( 0, -80, 16 );

		if ( save is not null && save.Owner == "Occupied" && save.FortStanding )
		{
			Spawn( "Occupied Fort", fortPos, OccupiedFortTint, go =>
			{
				var site = go.Components.Create<OccupiedSite>();
				site.Owner = save.Owner;
				site.Stockpile = save.OccupierScrap;
			} );
			return;
		}

		Spawn( "Fort Ghost", fortPos, FortGhostTint, go => go.Components.Create<FortGhost>().ScrapCost = 20 );
	}

	static void Spawn( string name, Vector3 pos, Color tint, Action<GameObject> setup )
	{
		var go = new GameObject( true, name );
		go.WorldPosition = pos;

		var renderer = go.Components.Create<ModelRenderer>();
		renderer.Model = Model.Load( "models/dev/box.vmdl" );
		renderer.Tint = tint;

		setup( go );
	}
}
