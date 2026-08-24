/// <summary>
/// What the next drop is. Static because the choice outlives the operation scene: the board
/// (Task 12) sets it, then the cavern loads and the director reads it.
/// Task 11 adds the storm length here.
/// The editor New Level menu and DEV slot 7 write SelectedSiteId so a hole can be chosen
/// before the board exists.
/// </summary>
public static class CampaignSession
{
	public static string SelectedSiteId { get; set; } = "cavern_0";

	/// <summary>Ids created this editor session that may not have mounted JSON yet.</summary>
	public static List<string> ExtraSiteIds { get; set; } = new();

	/// <summary>Slot1–9 iterate keys. Turn off for Task 8 playtests (SkyNeet / Dev Loop Off).</summary>
	public static bool DevLoop { get; set; } = true;
}
