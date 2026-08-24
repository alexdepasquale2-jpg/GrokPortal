/// <summary>
/// Every hole the game knows. Resolve order: mounted JSON → Data override → compiled builtin → cavern_0.
/// Discovery does not use FindFile (API not compile-checked here). It unions builtins,
/// <c>levels/index.json</c>, session extras from the editor menu, and a short cavern_N probe.
/// </summary>
public static class LevelCatalog
{
	public const string IndexPath = "levels/index.json";
	public const string IndexPathAlt = "Assets/levels/index.json";
	public const string DataIndexPath = "skyneet_level_index.json";

	/// <summary>The slice plus the 60-second iterate hole. Always available even if JSON does not mount.</summary>
	public static readonly string[] BuiltinIds = { "cavern_0", "cavern_1", "cavern_2", "dev_60s" };

	public static LevelDef Resolve( string id )
	{
		if ( string.IsNullOrWhiteSpace( id ) )
			id = "cavern_0";

		var fromFile = TryReadFile( id );
		if ( IsComplete( fromFile ) )
		{
			fromFile.Id = id;
			return fromFile;
		}

		if ( HasBuiltin( id ) )
			return Builtin( id );

		if ( CampaignSession.ExtraSiteIds.Contains( id ) )
		{
			var stamped = FromTemplate( id );
			Log.Info( $"[SkyNeet] Level '{id}' has no file yet — stamping the cavern_0 template." );
			return stamped;
		}

		Log.Info( $"[SkyNeet] Unknown level '{id}', using cavern_0." );
		return Builtin( "cavern_0" );
	}

	public static bool HasBuiltin( string id )
	{
		return BuiltinIds.Contains( id );
	}

	public static LevelDef Builtin( string id )
	{
		return id switch
		{
			"cavern_1" => Cavern1(),
			"cavern_2" => Cavern2(),
			"dev_60s" => Dev60s(),
			_ => Cavern0()
		};
	}

	public static LevelDef FromTemplate( string id )
	{
		var def = Cavern0();
		def.Id = id;
		def.Title = id;
		return def;
	}

	public static bool IsComplete( LevelDef def )
	{
		if ( def is null )
			return false;
		if ( def.Node is null || def.Extract is null || def.Fort is null || def.Sancient is null )
			return false;
		if ( def.Scrap is null || def.Scrap.Count == 0 )
			return false;
		if ( def.Goliaths is null || def.Goliaths.Count == 0 )
			return false;
		if ( def.StormSeconds <= 0f )
			return false;
		return true;
	}

	public static List<string> AllIds()
	{
		var ids = new List<string>();
		foreach ( var id in BuiltinIds )
			AddUnique( ids, id );

		foreach ( var id in ReadIndex() )
			AddUnique( ids, id );

		foreach ( var id in CampaignSession.ExtraSiteIds )
			AddUnique( ids, id );

		for ( var i = 0; i <= 15; i++ )
		{
			var id = "cavern_" + i;
			if ( ids.Contains( id ) )
				continue;
			if ( FilePresent( id ) )
				AddUnique( ids, id );
		}

		return ids;
	}

	public static string NextAfter( string current )
	{
		var ids = AllIds();
		if ( ids.Count == 0 )
			return "cavern_0";
		var idx = ids.IndexOf( current );
		if ( idx < 0 )
			return ids[0];
		return ids[(idx + 1) % ids.Count];
	}

	public static string NextNewId()
	{
		var used = AllIds();
		for ( var i = 0; i <= 99; i++ )
		{
			var id = "cavern_" + i;
			if ( !used.Contains( id ) )
				return id;
		}
		return "cavern_x";
	}

	public static LevelDef Cavern0()
	{
		return new LevelDef
		{
			Id = "cavern_0",
			Title = "Cavern",
			StormSeconds = 15f * 60f,
			FortCost = 20,
			Node = LevelPoint.At( 180, 0, 32 ),
			Extract = LevelPoint.At( -220, 0, 32 ),
			Fort = LevelPoint.At( 0, -80, 16 ),
			Sancient = LevelPoint.At( 400, 400, 40 ),
			Scrap = new List<ScrapSpawn>
			{
				new ScrapSpawn { X = 80, Y = 120, Z = 16, Amount = 15 },
				new ScrapSpawn { X = -80, Y = 140, Z = 16, Amount = 15 },
				new ScrapSpawn { X = 40, Y = -160, Z = 16, Amount = 20 }
			},
			Goliaths = new List<GoliathSpawn>
			{
				new GoliathSpawn { X = 300, Y = 200, Z = 40, Lethality = 0.85f }
			}
		};
	}

	public static LevelDef Cavern1()
	{
		return new LevelDef
		{
			Id = "cavern_1",
			Title = "Mirror Hole",
			StormSeconds = 15f * 60f,
			FortCost = 20,
			Node = LevelPoint.At( -180, 0, 32 ),
			Extract = LevelPoint.At( 220, 0, 32 ),
			Fort = LevelPoint.At( 0, 80, 16 ),
			Sancient = LevelPoint.At( -400, 400, 40 ),
			Scrap = new List<ScrapSpawn>
			{
				new ScrapSpawn { X = -80, Y = 120, Z = 16, Amount = 15 },
				new ScrapSpawn { X = 80, Y = 140, Z = 16, Amount = 20 },
				new ScrapSpawn { X = -40, Y = -160, Z = 16, Amount = 25 }
			},
			Goliaths = new List<GoliathSpawn>
			{
				new GoliathSpawn { X = -300, Y = 200, Z = 40, Lethality = 0.7f }
			}
		};
	}

	public static LevelDef Cavern2()
	{
		return new LevelDef
		{
			Id = "cavern_2",
			Title = "Scrap Trail",
			StormSeconds = 15f * 60f,
			FortCost = 20,
			Node = LevelPoint.At( 0, 240, 32 ),
			Extract = LevelPoint.At( 0, -240, 32 ),
			Fort = LevelPoint.At( 120, -80, 16 ),
			Sancient = LevelPoint.At( 0, 400, 40 ),
			Scrap = new List<ScrapSpawn>
			{
				new ScrapSpawn { X = 100, Y = 0, Z = 16, Amount = 10 },
				new ScrapSpawn { X = 160, Y = 40, Z = 16, Amount = 10 },
				new ScrapSpawn { X = 220, Y = 80, Z = 16, Amount = 10 },
				new ScrapSpawn { X = -120, Y = -40, Z = 16, Amount = 30 }
			},
			Goliaths = new List<GoliathSpawn>
			{
				new GoliathSpawn { X = -280, Y = 160, Z = 40, Lethality = 0.9f }
			}
		};
	}

	/// <summary>Iterate combat and occupancy in one minute, not fifteen.</summary>
	public static LevelDef Dev60s()
	{
		return new LevelDef
		{
			Id = "dev_60s",
			Title = "Dev 60s",
			StormSeconds = 60f,
			FortCost = 20,
			Node = LevelPoint.At( 80, 0, 32 ),
			Extract = LevelPoint.At( -80, 0, 32 ),
			Fort = LevelPoint.At( 0, -60, 16 ),
			Sancient = LevelPoint.At( 160, 160, 40 ),
			Scrap = new List<ScrapSpawn>
			{
				new ScrapSpawn { X = 40, Y = 40, Z = 16, Amount = 20 },
				new ScrapSpawn { X = -40, Y = 40, Z = 16, Amount = 20 }
			},
			Goliaths = new List<GoliathSpawn>
			{
				new GoliathSpawn { X = 120, Y = 80, Z = 40, Lethality = 0.85f }
			}
		};
	}

	static void AddUnique( List<string> ids, string id )
	{
		if ( string.IsNullOrWhiteSpace( id ) || ids.Contains( id ) )
			return;
		ids.Add( id );
	}

	static IEnumerable<string> ReadIndex()
	{
		foreach ( var path in new[] { IndexPath, IndexPathAlt } )
		{
			var parsed = ParseIndex( TryReadMounted( path ) );
			if ( parsed is not null )
				return parsed;
		}

		var dataIndex = ParseIndex( TryReadData( DataIndexPath ) );
		if ( dataIndex is not null )
			return dataIndex;

		return Array.Empty<string>();
	}

	static List<string> ParseIndex( string text )
	{
		if ( text is null )
			return null;
		try
		{
			var index = Json.Deserialize<LevelIndex>( text );
			if ( index?.Ids is not null )
				return index.Ids;
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Level index skipped: {e.Message}" );
		}

		return null;
	}

	/// <summary>
	/// Editor New Level. Writes the hole into FileSystem.Data so the next drop can load it
	/// without System.IO (sandboxed) and without waiting for Assets to remount. Copy the
	/// log line into Assets/levels if you want it in git.
	/// </summary>
	public static void WriteOverride( LevelDef def )
	{
		if ( def is null || string.IsNullOrWhiteSpace( def.Id ) )
			return;

		try
		{
			FileSystem.Data.WriteAllText( DataPath( def.Id ), Json.Serialize( def ) );
			var ids = AllIds();
			if ( !ids.Contains( def.Id ) )
				ids.Add( def.Id );
			FileSystem.Data.WriteAllText( DataIndexPath, Json.Serialize( new LevelIndex { Ids = ids } ) );
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Level override write failed: {e.Message}" );
		}
	}

	static bool FilePresent( string id )
	{
		foreach ( var path in FilePaths( id ) )
		{
			if ( ExistsMounted( path ) || ExistsData( path ) )
				return true;
		}

		return ExistsData( DataPath( id ) );
	}

	static LevelDef TryReadFile( string id )
	{
		foreach ( var path in FilePaths( id ) )
		{
			var def = ParseLevel( TryReadMounted( path ) ?? TryReadData( path ), id, path );
			if ( def is not null )
				return def;
		}

		return ParseLevel( TryReadData( DataPath( id ) ), id, DataPath( id ) );
	}

	static LevelDef ParseLevel( string text, string id, string path )
	{
		if ( text is null )
			return null;
		try
		{
			var def = Json.Deserialize<LevelDef>( text );
			if ( def is not null )
				return def;
		}
		catch ( Exception e )
		{
			Log.Warning( $"[SkyNeet] Level '{id}' at {path} skipped: {e.Message}" );
		}

		return null;
	}

	static string TryReadMounted( string path )
	{
		try
		{
			if ( FileSystem.Mounted is null )
				return null;
			if ( !FileSystem.Mounted.FileExists( path ) )
				return null;
			return FileSystem.Mounted.ReadAllText( path );
		}
		catch
		{
			return null;
		}
	}

	static string TryReadData( string path )
	{
		try
		{
			if ( !FileSystem.Data.FileExists( path ) )
				return null;
			return FileSystem.Data.ReadAllText( path );
		}
		catch
		{
			return null;
		}
	}

	static bool ExistsMounted( string path )
	{
		try
		{
			return FileSystem.Mounted is not null && FileSystem.Mounted.FileExists( path );
		}
		catch
		{
			return false;
		}
	}

	static bool ExistsData( string path )
	{
		try
		{
			return FileSystem.Data.FileExists( path );
		}
		catch
		{
			return false;
		}
	}

	static string DataPath( string id ) => "skyneet_level_" + id + ".json";

	static string[] FilePaths( string id )
	{
		return new[] { "levels/" + id + ".json", "Assets/levels/" + id + ".json" };
	}
}
