/// <summary>
/// Builds the readable slice cavern. Every prop is the same dev box, so tint is the only
/// thing telling the player what a shape means: cyan NNN, green extract, orange scrap,
/// pale fort, red Goliath, purple Sancient.
/// Tints come from SliceTints so spawn cannot drift from the live updates that
/// NeetNetNode and SancientDirector apply.
/// Spawns are guarded so authored scene objects win over runtime ones.
/// </summary>
public static class WorldFactory
{
	public static void Build( OperationDirector director )
	{
		if ( director is null )
			return;

		var scene = director.Scene;

		if ( !scene.GetAllComponents<NeetNetNode>().Any() )
			Spawn( "NeetNetNode", new Vector3( 180, 0, 32 ), SliceTints.NodeTint, go => go.Components.Create<NeetNetNode>() );

		if ( !scene.GetAllComponents<ExtractZone>().Any() )
			Spawn( "Extract", new Vector3( -220, 0, 32 ), SliceTints.ExtractTint, go => go.Components.Create<ExtractZone>() );

		if ( !scene.GetAllComponents<ScrapPile>().Any() )
		{
			Spawn( "Scrap A", new Vector3( 80, 120, 16 ), SliceTints.ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 15 );
			Spawn( "Scrap B", new Vector3( -80, 140, 16 ), SliceTints.ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 15 );
			Spawn( "Scrap C", new Vector3( 40, -160, 16 ), SliceTints.ScrapTint, go => go.Components.Create<ScrapPile>().Amount = 20 );
		}

		BuildFort( scene, director.LastSave );

		if ( !scene.GetAllComponents<Goliath>().Any() )
			Spawn( "Goliath", new Vector3( 300, 200, 40 ), SliceTints.GoliathTint, go => go.Components.Create<Goliath>().Lethality = 0.85f );

		if ( !scene.GetAllComponents<SancientDirector>().Any() )
			Spawn( "Sancient", new Vector3( 400, 400, 40 ), SliceTints.SancientTint, go => go.Components.Create<SancientDirector>() );
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
			Spawn( "Occupied Fort", fortPos, SliceTints.OccupiedFortTint, go =>
			{
				var site = go.Components.Create<OccupiedSite>();
				site.Owner = save.Owner;
				site.Stockpile = save.OccupierScrap;
			} );
			return;
		}

		Spawn( "Fort Ghost", fortPos, SliceTints.FortGhostTint, go => go.Components.Create<FortGhost>().ScrapCost = 20 );
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
