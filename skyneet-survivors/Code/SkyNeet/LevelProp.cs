/// <summary>
/// Marker on factory-spawned props. Rebuild deletes these and restamps the current
/// <see cref="LevelDef"/> so a JSON/C# edit is visible without leaving play.
/// Authored scene objects do not carry this, so they win over the factory.
/// </summary>
public sealed class LevelProp : Component
{
}
