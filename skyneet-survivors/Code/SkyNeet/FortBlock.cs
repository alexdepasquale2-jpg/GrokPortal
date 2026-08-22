/// <summary>
/// Slice stand-in for "built pieces block pathing" (spec §4).
/// Goliaths write WorldPosition, so BoxCollider never stops them. Until we earn a
/// CharacterController or NavMeshArea, refuse a wish position that overlaps a
/// solidified fort or any other BoxCollider that is not the Goliath or the player.
/// </summary>
public static class FortBlock
{
	public const float DefaultRadius = 48f;

	public static bool Hits( Scene scene, GameObject self, Vector3 wish, float radius = DefaultRadius )
	{
		if ( scene is null )
			return false;

		foreach ( var ghost in scene.GetAllComponents<FortGhost>() )
		{
			if ( !ghost.Solid )
				continue;
			if ( Horizontal( wish, ghost.WorldPosition ) < radius )
				return true;
		}

		foreach ( var col in scene.GetAllComponents<BoxCollider>() )
		{
			var go = col.GameObject;
			if ( !go.IsValid() || go == self )
				continue;
			if ( go.Components.Get<PlayerController>() is not null )
				continue;
			if ( go.Components.Get<Goliath>() is not null )
				continue;
			if ( Horizontal( wish, go.WorldPosition ) < radius )
				return true;
		}

		return false;
	}

	static float Horizontal( Vector3 a, Vector3 b )
	{
		return (a - b).WithZ( 0f ).Length;
	}
}
