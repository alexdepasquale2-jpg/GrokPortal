/// <summary>
/// One hole. Not a scene, not C# — a layout the factory stamps into <c>cavern.scene</c>.
/// Add a file at <c>Assets/levels/{Id}.json</c> (or a builtin in <see cref="LevelCatalog"/>)
/// and the next drop is that field. JSON wins when it mounts; C# is the fallback so a
/// missing FileSystem path cannot blank the slice.
/// </summary>
public sealed class LevelDef
{
	public string Id { get; set; } = "cavern_0";
	public string Title { get; set; } = "Cavern";
	public float StormSeconds { get; set; } = 15f * 60f;
	public int StartingScrap { get; set; }
	public int FortCost { get; set; } = 20;
	public float SancientLoudness { get; set; } = 8f;
	public float SancientWindow { get; set; } = 25f;

	public LevelPoint Node { get; set; }
	public LevelPoint Extract { get; set; }
	public LevelPoint Fort { get; set; }
	public LevelPoint Sancient { get; set; }
	public List<ScrapSpawn> Scrap { get; set; } = new();
	public List<GoliathSpawn> Goliaths { get; set; } = new();
}

public sealed class LevelPoint
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }

	public Vector3 Vec => new Vector3( X, Y, Z );

	public static LevelPoint At( float x, float y, float z ) => new LevelPoint { X = x, Y = y, Z = z };
}

public sealed class ScrapSpawn
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; } = 16f;
	public int Amount { get; set; } = 15;

	public Vector3 Vec => new Vector3( X, Y, Z );
}

public sealed class GoliathSpawn
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; } = 40f;
	public float Lethality { get; set; } = 0.85f;

	public Vector3 Vec => new Vector3( X, Y, Z );
}

/// <summary>Optional extra ids beyond the compiled builtins. Written by the editor New Level menu.</summary>
public sealed class LevelIndex
{
	public List<string> Ids { get; set; } = new();
}
