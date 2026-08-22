/// <summary>
/// Editor-only inverse-competence checks. Menu: SkyNeet → Run Logic Tests.
/// DisplayDialog needs the okay-button arg on current s&box; the plan's two-arg call would not compile.
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
		EditorUtility.DisplayDialog( "SkyNeet tests", "PASS", "OK" );
	}

	static void Expect( string name, bool ok )
	{
		if ( !ok )
			throw new System.Exception( "FAIL: " + name );
	}
}
