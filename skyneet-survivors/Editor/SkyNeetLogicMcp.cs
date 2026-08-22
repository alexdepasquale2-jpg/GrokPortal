/// <summary>
/// MCP entry for inverse-competence tests so Grok does not need the editor menu.
/// Does not call DisplayDialog (that would block the MCP thread). Throws on FAIL.
/// Search with search_tools "skyneet_logic".
/// </summary>
[McpToolset( "skyneet_logic", "SkyNeet CompetenceRules checks. Safe if SkyNeetPlayMcp.cs is deleted." )]
public static class SkyNeetLogicMcp
{
	/// <summary>
	/// Runs the same assertions as SkyNeet/Run Logic Tests. Returns PASS or throws FAIL: name.
	/// Call after compile_status is clean. Does not need play mode.
	/// </summary>
	[McpTool.ReadOnly( "run_logic_tests" )]
	public static string RunLogicTests()
	{
		Expect( "high lethality is stupid", CompetenceRules.SpawnCompetence( 0.9f ) < 0.2f );
		Expect( "low lethality aims", CompetenceRules.SpawnCompetence( 0.1f ) > 0.8f );
		Expect( "clamp low", CompetenceRules.SpawnCompetence( 2f ) <= CompetenceRules.Max );
		Expect( "hit", CompetenceRules.RollHit( 1f, 0.5f ) );
		Expect( "miss", !CompetenceRules.RollHit( 0.1f, 0.9f ) );
		Expect( "puppet is 1", CompetenceRules.PuppetCompetence() == 1f );
		return "PASS";
	}

	static void Expect( string name, bool ok )
	{
		if ( !ok )
			throw new System.Exception( "FAIL: " + name );
	}
}
