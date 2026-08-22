/// <summary>
/// One field in the war. The operation resets; this does not.
/// Mirrors <see cref="CampaignSave"/> one site at a time — <c>Stockpile</c> is that site's
/// scrap, which is yours or theirs depending on who holds it.
/// </summary>
public sealed class SiteRecord
{
	public string Id { get; set; } = "cavern_0";
	public string Owner { get; set; } = "Neet";
	public bool NodeUp { get; set; }
	public int Stockpile { get; set; }

	/// <summary>
	/// Not in the plan's sketch, but Task 5 already branches on it: without it a retaken or
	/// re-dropped site forgets whether anything is built on it, and the red Occupied fort
	/// stops appearing.
	/// </summary>
	public bool FortStanding { get; set; }
}

/// <summary>
/// Every site the war remembers. v1 is three caverns in one biome family (spec §12) —
/// no adjacency or travel cost yet, just the set of fields and who is running each one.
/// </summary>
public sealed class CampaignGraph
{
	public List<SiteRecord> Sites { get; set; } = new();

	/// <summary>The v1 starting position: three caverns, all still yours.</summary>
	public static CampaignGraph DemoThreeSites()
	{
		return new CampaignGraph
		{
			Sites = new List<SiteRecord>
			{
				new SiteRecord { Id = "cavern_0" },
				new SiteRecord { Id = "cavern_1" },
				new SiteRecord { Id = "cavern_2" }
			}
		};
	}

	/// <summary>
	/// The record for a site, created if the graph has not seen it. Dropping into a site the
	/// save predates should read as an untouched field, never as a crash.
	/// </summary>
	public SiteRecord Get( string id )
	{
		var record = Sites.FirstOrDefault( s => s.Id == id );
		if ( record is not null )
			return record;

		record = new SiteRecord { Id = id };
		Sites.Add( record );
		return record;
	}
}
