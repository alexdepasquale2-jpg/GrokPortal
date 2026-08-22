namespace Editor.Mcp;

/// <summary>
/// First-party editor tools so Grok (who can reach loopback MCP) can verify Cursor's
/// Task 4 / Task 6 work without guessing log strings. Search with search_tools "skyneet".
/// </summary>
[McpToolset( "skyneet", "SkyNeet Survivors slice helpers for Grok, Claude, and Cursor" )]
public static class SkyNeetMcp
{
	/// <summary>
	/// Returns the playtest checklist for Cursor's NNN + Sancient slice work.
	/// Call this after compile_status is clean, then play_start, then operation_snapshot.
	/// </summary>
	[McpTool.ReadOnly( "slice_checklist" )]
	public static string SliceChecklist()
	{
		return
			"SkyNeet Cursor slice checklist (Tasks 4 + 6)\n" +
			"1. compile_status: 0 errors.\n" +
			"2. Editor menu SkyNeet/Run Logic Tests — Grok owns that menu (Task 2). Skip if missing.\n" +
			"3. play_start. Log: Drop into cavern_0.\n" +
			"4. DARK: walk up to the Goliath. It must not close distance. No damage.\n" +
			"5. Walk to cyan NNN. HUD appends: HOLD E — WAKE THE HOLE\n" +
			"6. Press E. Log: [SkyNeet] NNN online. HUD NNN ONLINE. Cyan box goes brighter.\n" +
			"7. Goliath starts moving and mostly MISSES ([SkyNeet] Goliath missed).\n" +
			"8. Wait ~1 min after storm start (or plant after 60s). LoudnessThreshold 8, plant is +25.\n" +
			"9. Log: [SkyNeet] Sancient on the net. They remember.\n" +
			"10. Goliath tint yellow-white. HUD: SANCIENT  THEY REMEMBER. Hits become frequent.\n" +
			"11. Window ~25s. Log: Sancient window closed. Tint back to dark red. Misses return.\n" +
			"12. play_stop.\n" +
			"Do not start Wave 4 (Tasks 9-16) until Task 8 human playtest.";
	}

	/// <summary>
	/// Live operation snapshot from the active scene. Requires play mode.
	/// Returns clock, NNN, loudness, Sancient window, nearby plant prompt, Goliath tints.
	/// Pass nothing; if this throws, play_start first.
	/// </summary>
	[McpTool.ReadOnly( "operation_snapshot" )]
	public static object OperationSnapshot()
	{
		var scene = Game.ActiveScene;
		if ( scene is null )
			throw new System.Exception( "No active scene. play_start, then call operation_snapshot." );

		var director = scene.GetAllComponents<OperationDirector>().FirstOrDefault();
		if ( director is null )
			throw new System.Exception( "No OperationDirector in the active scene. Is cavern.scene playing?" );

		var player = scene.GetAllComponents<PlayerController>().FirstOrDefault();
		var plantPrompt = false;
		if ( player is not null )
		{
			foreach ( var node in scene.GetAllComponents<NeetNetNode>() )
			{
				if ( node.ShouldPromptPlant( player.WorldPosition ) )
					plantPrompt = true;
			}
		}

		var goliaths = scene.GetAllComponents<Goliath>()
			.Select( g =>
			{
				var renderer = g.Components.Get<ModelRenderer>();
				return new
				{
					Name = g.GameObject.Name,
					g.Competence,
					g.Puppeted,
					Tint = renderer is not null ? renderer.Tint.ToString() : ""
				};
			} )
			.ToArray();

		var sancient = scene.GetAllComponents<SancientDirector>().FirstOrDefault();

		return new
		{
			director.TimeLeft,
			director.NodeUp,
			director.Loudness,
			director.SancientActive,
			director.OperationEnded,
			director.EndReason,
			director.SiteOwner,
			director.Scrap,
			PlantPrompt = plantPrompt,
			WindowOpen = sancient is not null && sancient.WindowOpen,
			LoudnessThreshold = sancient?.LoudnessThreshold,
			EarliestTimeRemaining = sancient?.EarliestTimeRemaining,
			Goliaths = goliaths
		};
	}
}
