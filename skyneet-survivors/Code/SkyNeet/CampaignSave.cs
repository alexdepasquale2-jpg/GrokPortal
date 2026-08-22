/// <summary>
/// Persistent site memory. The backpack dies; this does not.
/// </summary>
public sealed class CampaignSave
{
	public string SiteId { get; set; } = "cavern_0";
	public string Owner { get; set; } = "Neet";
	public bool NodeUp { get; set; }
	public int OccupierScrap { get; set; }
	public bool FortStanding { get; set; }
}
