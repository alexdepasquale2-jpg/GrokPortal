/// <summary>
/// Builds the readable slice cavern from a <see cref="LevelDef"/>. Every prop is the same
/// dev box, so tint is the only thing telling the player what a shape means: cyan NNN,
/// green extract, orange scrap, pale fort, red Goliath, purple Sancient.
/// Tints come from SliceTints so spawn cannot drift from the live updates that
/// NeetNetNode and SancientDirector apply.
/// Spawns are guarded so authored scene objects win over runtime ones.
/// Factory props carry <see cref="LevelProp"/> so Rebuild can restamp without leaving play.
/// </summary>
public static class WorldFactory
{
	public static void Build( OperationDirector director )
	{
		if ( director is null )
			return;

		var scene = director.Scene;
		var level = LevelCatalog.Resolve( OperationDirector.SiteId );

		if ( !scene.GetAllComponents<NeetNetNode>().Any() && level.Node is not null )
			Spawn( "NeetNetNode", level.Node.Vec, SliceTints.NodeTint, go => go.Components.Create<NeetNetNode>() );

		if ( !scene.GetAllComponents<ExtractZone>().Any() && level.Extract is not null )
			Spawn( "Extract", level.Extract.Vec, SliceTints.ExtractTint, go => go.Components.Create<ExtractZone>() );

		if ( !scene.GetAllComponents<ScrapPile>().Any() && level.Scrap is not null )
		{
			for ( var i = 0; i < level.Scrap.Count; i++ )
			{
				var pile = level.Scrap[i];
				var amount = pile.Amount;
				Spawn( "Scrap " + i, pile.Vec, SliceTints.ScrapTint, go => go.Components.Create<ScrapPile>().Amount = amount );
			}
		}

		BuildFort( scene, director.LastSave, level );

		if ( !scene.GetAllComponents<Goliath>().Any() && level.Goliaths is not null )
		{
			for ( var i = 0; i < level.Goliaths.Count; i++ )
			{
				var spawn = level.Goliaths[i];
				var lethality = spawn.Lethality;
				Spawn( "Goliath " + i, spawn.Vec, SliceTints.GoliathTint, go => go.Components.Create<Goliath>().Lethality = lethality );
			}
		}

		if ( !scene.GetAllComponents<SancientDirector>().Any() && level.Sancient is not null )
		{
			var loudness = level.SancientLoudness;
			var window = level.SancientWindow;
			Spawn( "Sancient", level.Sancient.Vec, SliceTints.SancientTint, go =>
			{
				var directorComp = go.Components.Create<SancientDirector>();
				directorComp.LoudnessThreshold = loudness;
				directorComp.WindowSeconds = window;
			} );
		}
	}

	/// <summary>
	/// Snap the hole back to the current LevelDef. Used by DEV slot 6 and after cycling sites.
	/// Does not touch the pawn, HUD, or campaign file.
	/// </summary>
	public static void Rebuild( OperationDirector director )
	{
		if ( director is null )
			return;

		foreach ( var marker in director.Scene.GetAllComponents<LevelProp>().ToList() )
		{
			if ( marker.GameObject.IsValid() )
				marker.DestroyGameObject();
		}

		Build( director );
	}

	/// <summary>
	/// Whoever owns the field owns what is built on it. Lose the site with a fort standing
	/// and you come back to a red one you cannot use, holding the scrap you dropped.
	/// </summary>
	static void BuildFort( Scene scene, CampaignSave save, LevelDef level )
	{
		if ( scene.GetAllComponents<FortGhost>().Any() || scene.GetAllComponents<OccupiedSite>().Any() )
			return;

		var fortPos = level.Fort is not null ? level.Fort.Vec : new Vector3( 0, -80, 16 );
		var cost = level.FortCost > 0 ? level.FortCost : 20;

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

		Spawn( "Fort Ghost", fortPos, SliceTints.FortGhostTint, go => go.Components.Create<FortGhost>().ScrapCost = cost );
	}

	static void Spawn( string name, Vector3 pos, Color tint, Action<GameObject> setup )
	{
		var go = new GameObject( true, name );
		go.WorldPosition = pos;
		go.Components.Create<LevelProp>();

		var renderer = go.Components.Create<ModelRenderer>();
		renderer.Model = Model.Load( "models/dev/box.vmdl" );
		renderer.Tint = tint;

		setup( go );
	}
}
