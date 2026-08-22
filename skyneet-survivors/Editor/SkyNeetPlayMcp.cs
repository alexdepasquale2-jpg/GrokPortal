/// <summary>
/// Play-mode snapshot. Isolated from slice_checklist so a bad scene API cannot take
/// down the whole editor assembly (and with it SkyNeet/Run Logic Tests).
/// If this file fails to compile, delete it. Checklist in SkyNeetMcp.cs stays.
/// </summary>
[McpToolset( "skyneet_play", "Live SkyNeet operation snapshot. Delete this file if the editor assembly goes red." )]
public static class SkyNeetPlayMcp
{
	/// <summary>
	/// Live operation snapshot from the active scene. Requires play mode.
	/// Returns clock, NNN, loudness, Sancient window, nearby plant prompt, Goliath tints.
	/// If this tool is missing, the editor compiled without this file on purpose.
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
