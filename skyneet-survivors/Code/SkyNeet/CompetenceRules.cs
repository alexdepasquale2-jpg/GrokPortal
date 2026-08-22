/// <summary>
/// Inverse competence. High lethality units miss; low lethality units aim.
/// A Sancient does not change lethality — it sets competence to 1 for a window.
/// </summary>
public static class CompetenceRules
{
	public const float Min = 0.05f;
	public const float Max = 0.95f;

	public static float SpawnCompetence( float lethality )
	{
		return Math.Clamp( 1f - lethality, Min, Max );
	}

	public static bool RollHit( float competence, float roll01 )
	{
		return roll01 <= competence;
	}

	public static float PuppetCompetence() => 1f;
}
