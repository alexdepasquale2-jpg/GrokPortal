/// <summary>
/// Slice cavern tints. One place so spawn (WorldFactory) and live updates (node plant,
/// Sancient puppet) cannot drift. Claude: please switch WorldFactory's copies to these.
/// </summary>
public static class SliceTints
{
	public static readonly Color NodeTint = new Color( 0.2f, 0.9f, 1f );
	public static readonly Color NodeLitTint = new Color( 0.7f, 1f, 1f );
	public static readonly Color ExtractTint = new Color( 0.2f, 1f, 0.3f );
	public static readonly Color ScrapTint = new Color( 0.8f, 0.55f, 0.1f );
	public static readonly Color FortGhostTint = new Color( 1f, 1f, 1f, 0.35f );
	public static readonly Color GoliathTint = new Color( 0.7f, 0.1f, 0.1f );
	public static readonly Color GoliathPuppetTint = Color.Yellow;
	public static readonly Color SancientTint = new Color( 0.6f, 0.2f, 1f );
	public static readonly Color OccupiedFortTint = Color.Red;
}
