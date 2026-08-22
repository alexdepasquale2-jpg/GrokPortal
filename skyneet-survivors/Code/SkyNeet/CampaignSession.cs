/// <summary>
/// What the next drop is. Static because the choice outlives the operation scene: the board
/// (Task 12) sets it, then the cavern loads and the director reads it.
/// Task 11 adds the storm length here.
/// </summary>
public static class CampaignSession
{
	public static string SelectedSiteId { get; set; } = "cavern_0";
}
