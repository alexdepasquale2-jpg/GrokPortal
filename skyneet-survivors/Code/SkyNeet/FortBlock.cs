/// <summary>
/// Slice stand-in for "built pieces block pathing" (spec §4).
/// Goliaths write WorldPosition directly, so a BoxCollider never stops them. Until we earn
/// a CharacterController or NavMeshArea, refuse a wish position that overlaps a built piece.
///
/// This asks what a thing *is*, not whether it happens to carry a collider. Sweeping every
/// BoxCollider in the scene meant the player's own collider — which sits on a child object,
/// not on the one holding PlayerController — read as a wall, and Goliaths halted 48u out and
/// never closed. A built piece is a solidified FortGhost or an OccupiedSite; nothing else in
/// the cavern blocks, so nothing else can block by accident.
/// </summary>
public static class FortBlock
{
	public const float DefaultRadius = 48f;

	public static bool Hits( Scene scene, GameObject self, Vector3 wish, float radius = DefaultRadius )
	{
		if ( scene is null )
			return false;

		// A ghost you have not bought is not a wall (spec §4: ghosts do not block).
		foreach ( var ghost in scene.GetAllComponents<FortGhost>() )
		{
			if ( !ghost.Solid )
				continue;

			if ( Blocks( ghost.GameObject, self, wish, radius ) )
				return true;
		}

		// Someone else's fort still stands, and still blocks.
		foreach ( var site in scene.GetAllComponents<OccupiedSite>() )
		{
			if ( Blocks( site.GameObject, self, wish, radius ) )
				return true;
		}

		return false;
	}

	static bool Blocks( GameObject piece, GameObject self, Vector3 wish, float radius )
	{
		if ( !piece.IsValid() || piece == self )
			return false;

		return Horizontal( wish, piece.WorldPosition ) < radius;
	}

	static float Horizontal( Vector3 a, Vector3 b )
	{
		return (a - b).WithZ( 0f ).Length;
	}
}
