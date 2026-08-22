/// <summary>
/// Host-side operation: clock, NNN, loudness, extract, occupancy.
/// Causal chain: scavenge → build → NNN → wake machines → Sancient → leave or die → the field remains.
/// </summary>
public sealed class OperationDirector : Component
{
	public const string SaveFile = "skyneet_site.json";
	public const string SiteId = "cavern_0";

	[Property] public float StormSeconds { get; set; } = 15f * 60f;
	[Property] public int StartingScrap { get; set; } = 0;

	[Sync( SyncFlags.FromHost )] public float TimeLeft { get; set; }
	[Sync( SyncFlags.FromHost )] public bool NodeUp { get; set; }
	[Sync( SyncFlags.FromHost )] public float Loudness { get; set; }
	[Sync( SyncFlags.FromHost )] public bool SancientActive { get; set; }
	[Sync( SyncFlags.FromHost )] public bool OperationEnded { get; set; }
	[Sync( SyncFlags.FromHost )] public string EndReason { get; set; } = "";
	[Sync( SyncFlags.FromHost )] public string SiteOwner { get; set; } = "Neet";
	[Sync( SyncFlags.FromHost )] public int Scrap { get; set; }

	public CampaignSave LastSave { get; private set; }

	protected override void OnStart()
	{
		if ( IsProxy )
			return;

		LastSave = LoadSave();
		SiteOwner = LastSave.Owner;
		TimeLeft = StormSeconds;
		Scrap = StartingScrap;
		NodeUp = false;
		Loudness = 0f;
		SancientActive = false;
		OperationEnded = false;
		EndReason = "";

		Log.Info( $"[SkyNeet] Drop into {SiteId}. Owner from last run: {SiteOwner}. Fort standing: {LastSave.FortStanding}. NNN is the button." );

		EnsureWorld();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy || OperationEnded )
			return;

		TimeLeft -= Time.Delta;
		if ( TimeLeft <= 0f )
		{
			EndOperation( "clock" );
			return;
		}

		if ( NodeUp )
			Loudness += Time.Delta * 0.15f;

		var health = Scene.GetAllComponents<NeetHealth>().FirstOrDefault();
		if ( health is not null && health.IsDead )
			EndOperation( "death" );
	}

	public void AddScrap( int amount )
	{
		if ( IsProxy )
			return;
		Scrap += amount;
		Log.Info( $"[SkyNeet] Scrap {Scrap}" );
	}

	public bool TrySpendScrap( int amount )
	{
		if ( IsProxy || Scrap < amount )
			return false;
		Scrap -= amount;
		return true;
	}

	public void PlantNode()
	{
		if ( IsProxy || NodeUp || OperationEnded )
			return;

		NodeUp = true;
		Loudness += 25f;
		Log.Info( "[SkyNeet] NNN online. The hole is awake." );
	}

	public void NotifyBuild()
	{
		if ( IsProxy )
			return;
		Loudness += 8f;
	}

	public void EndOperation( string reason )
	{
		if ( OperationEnded )
			return;

		OperationEnded = true;
		EndReason = reason;
		TimeLeft = MathF.Max( TimeLeft, 0f );

		var save = new CampaignSave
		{
			SiteId = SiteId,
			NodeUp = NodeUp,
			FortStanding = FortStands(),
			OccupierScrap = Scrap
		};

		// Silent extract in the dark keeps the site. Lighting NNN and failing loses it.
		if ( reason == "extract" && !NodeUp )
			save.Owner = "Neet";
		else if ( reason is "death" or "clock" && NodeUp )
			save.Owner = "Occupied";
		else if ( reason == "extract" && NodeUp )
			save.Owner = "Neet";
		else
			save.Owner = SiteOwner;

		SiteOwner = save.Owner;
		WriteSave( save );
		LastSave = save;

		Log.Info( $"[SkyNeet] Operation ended ({reason}). Site owner is now {save.Owner}. NodeUp={save.NodeUp}." );
	}

	/// <summary>
	/// A fort stands if you raised one this run, or if one was already standing here
	/// under someone else's flag. Occupancy has to survive the run that lost it.
	/// </summary>
	bool FortStands()
	{
		if ( Scene.GetAllComponents<FortGhost>().Any( f => f.Solid ) )
			return true;

		return Scene.GetAllComponents<OccupiedSite>().Any();
	}

	CampaignSave LoadSave()
	{
		return CampaignStore.Load( SaveFile );
	}

	void WriteSave( CampaignSave save )
	{
		CampaignStore.Write( SaveFile, save );
	}

	void EnsureWorld()
	{
		// Props, tints and placement live in WorldFactory so the cavern stays readable.
		WorldFactory.Build( this );

		var player = Scene.GetAllComponents<PlayerController>().FirstOrDefault();
		if ( player is not null )
		{
			if ( player.Components.Get<NeetHealth>() is null )
				player.Components.Create<NeetHealth>();
			if ( player.Components.Get<AutoNeedler>() is null )
				player.Components.Create<AutoNeedler>();
		}

		if ( Scene.GetAllComponents<OperationHud>().FirstOrDefault() is null )
		{
			var hud = new GameObject( true, "SkyNeet HUD" );
			hud.Components.Create<ScreenPanel>();
			hud.Components.Create<OperationHud>();
		}

		if ( LastSave.Owner == "Occupied" )
			Log.Info( "[SkyNeet] This field is Occupied. The fort you raised last time is not yours." );
	}
}
