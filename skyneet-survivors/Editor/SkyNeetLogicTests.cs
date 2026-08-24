/// <summary>
/// Editor-only inverse-competence checks. Menu: SkyNeet → Run Logic Tests.
/// DisplayDialog needs the okay-button arg on current editor API; the plan's two-arg call would not compile.
/// </summary>
public static class SkyNeetLogicTests
{
	[Menu( "Editor", "SkyNeet/Run Logic Tests" )]
	public static void Run()
	{
		Expect( "high lethality is stupid", CompetenceRules.SpawnCompetence( 0.9f ) < 0.2f );
		Expect( "low lethality aims", CompetenceRules.SpawnCompetence( 0.1f ) > 0.8f );
		Expect( "clamp low", CompetenceRules.SpawnCompetence( 2f ) <= CompetenceRules.Max );
		Expect( "hit", CompetenceRules.RollHit( 1f, 0.5f ) );
		Expect( "miss", !CompetenceRules.RollHit( 0.1f, 0.9f ) );
		Expect( "puppet is 1", CompetenceRules.PuppetCompetence() == 1f );

		RunCampaignTests();
		RunLevelTests();

		EditorUtility.DisplayDialog( "SkyNeet tests", "PASS", "OK" );
	}

	/// <summary>Task 9: the campaign graph survives a round trip and grows on demand.</summary>
	static void RunCampaignTests()
	{
		var demo = CampaignGraph.DemoThreeSites();
		Expect( "three sites", demo.Sites.Count == 3 );
		Expect( "sites are the caverns", demo.Get( "cavern_0" ) is not null && demo.Get( "cavern_2" ) is not null );
		Expect( "sites start Neet", demo.Sites.All( s => s.Owner == "Neet" ) );
		Expect( "unknown site is created, not fatal", demo.Get( "cavern_9" ).Owner == "Neet" && demo.Sites.Count == 4 );
		Expect( "Get is stable", ReferenceEquals( demo.Get( "cavern_1" ), demo.Get( "cavern_1" ) ) );

		// The whole point of the graph is that it outlives the process.
		var before = CampaignGraph.DemoThreeSites();
		var lost = before.Get( "cavern_1" );
		lost.Owner = "Occupied";
		lost.FortStanding = true;
		lost.Stockpile = 42;
		lost.NodeUp = true;

		var after = Json.Deserialize<CampaignGraph>( Json.Serialize( before ) );
		Expect( "round trip keeps every site", after is not null && after.Sites.Count == 3 );

		var restored = after.Get( "cavern_1" );
		Expect( "round trip keeps owner", restored.Owner == "Occupied" );
		Expect( "round trip keeps the fort", restored.FortStanding );
		Expect( "round trip keeps the stockpile", restored.Stockpile == 42 );
		Expect( "round trip keeps node state", restored.NodeUp );
		Expect( "untouched sites stay Neet", after.Get( "cavern_0" ).Owner == "Neet" );
	}

	/// <summary>Snappy loop: a LevelDef is a complete hole, and a new id is just another def.</summary>
	static void RunLevelTests()
	{
		foreach ( var id in LevelCatalog.BuiltinIds )
			Expect( "builtin " + id + " is a complete hole", LevelCatalog.IsComplete( LevelCatalog.Builtin( id ) ) );

		var slice = LevelCatalog.Cavern0();
		Expect( "slice NNN is where WorldFactory used to hardcode it", slice.Node.X == 180f && slice.Node.Y == 0f );
		Expect( "slice fort cost is 20 scrap", slice.FortCost == 20 );
		Expect( "dev_60s is one minute", LevelCatalog.Dev60s().StormSeconds == 60f );

		var round = Json.Deserialize<LevelDef>( Json.Serialize( slice ) );
		Expect( "level json round trip", round is not null && round.Scrap.Count == slice.Scrap.Count && MathF.Abs( round.Goliaths[0].Lethality - 0.85f ) < 0.001f );

		var stamped = LevelCatalog.FromTemplate( "cavern_9" );
		Expect( "template keeps the causal pieces", LevelCatalog.IsComplete( stamped ) && stamped.Id == "cavern_9" );

		var ids = LevelCatalog.AllIds();
		Expect( "catalog lists the slice", ids.Contains( "cavern_0" ) && ids.Contains( "cavern_2" ) );
		Expect( "catalog lists the iterate hole", ids.Contains( "dev_60s" ) );
		Expect( "next new id is unused", !ids.Contains( LevelCatalog.NextNewId() ) );

		var cycle = LevelCatalog.NextAfter( "cavern_0" );
		Expect( "cycle moves", cycle != "cavern_0" && ids.Contains( cycle ) );
		Expect( "incomplete is rejected", !LevelCatalog.IsComplete( new LevelDef { Id = "empty" } ) );
	}

	static void Expect( string name, bool ok )
	{
		if ( !ok )
			throw new System.Exception( "FAIL: " + name );
	}
}
