/// <summary>
/// Safe-to-compile SkyNeet MCP helpers. Pure strings, no play-mode scene access.
/// If the editor assembly goes red, keep this file and delete SkyNeetPlayMcp.cs.
/// Search with search_tools "skyneet".
/// </summary>
[McpToolset( "skyneet", "SkyNeet Survivors slice helpers for Grok, Claude, and Cursor" )]
public static class SkyNeetMcp
{
	/// <summary>
	/// Returns the playtest checklist for Cursor's NNN + Sancient slice work.
	/// Call this after compile_status is clean. Live scene state is skyneet_play.operation_snapshot
	/// in SkyNeetPlayMcp.cs — delete that file if the editor assembly goes red.
	/// </summary>
	[McpTool.ReadOnly( "slice_checklist" )]
	public static string SliceChecklist()
	{
		return
			"SkyNeet Cursor slice checklist (Tasks 4 + 6)\n" +
			"1. compile_status: 0 errors. If the editor assembly is red, delete Editor/SkyNeetPlayMcp.cs and recompile.\n" +
			"2. Editor menu SkyNeet/Run Logic Tests (PR #2). Dialog PASS.\n" +
			"3. play_start. Log: Drop into cavern_0.\n" +
			"4. DARK: walk up to the Goliath. It must not close distance. No damage. Log: Goliath dormant (dark) once.\n" +
			"5. Walk to cyan NNN. HUD appends: HOLD E — WAKE THE HOLE\n" +
			"6. Press E. Log: [SkyNeet] NNN online. HUD NNN ONLINE. Cyan box goes brighter.\n" +
			"7. Goliath starts moving and mostly MISSES ([SkyNeet] Goliath missed).\n" +
			"8. Wait ~1 min after storm start (or plant after 60s). LoudnessThreshold 8, plant is +25.\n" +
			"9. Log: [SkyNeet] Sancient on the net. They remember.\n" +
			"10. Goliath tint yellow-white. HUD: SANCIENT  THEY REMEMBER. Hits become frequent.\n" +
			"11. Window ~25s. Log: Sancient window closed. Tint back to dark red. Misses return.\n" +
			"12. After ENDED, E on the node must NOT brighten it. Pawn must not move (Claude).\n" +
			"13. play_stop.\n" +
			"Do not start Wave 4 (Tasks 9-16) until Task 8 human playtest.";
	}
}
