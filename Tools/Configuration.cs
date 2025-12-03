namespace Ixjok.Tools;

public sealed class ConfigHelper
{
    public const string JsonDatabaseFilename = "Ixjok.json";
    public const string DatabaseFilename = "Ixjok.db3";
    public static string DatabaseDirectory => Path.Combine(FileSystem.AppDataDirectory, "Database");
    public const SQLite.SQLiteOpenFlags SqlFlags = SQLite.SQLiteOpenFlags.ReadWrite
    | SQLite.SQLiteOpenFlags.Create
    | SQLite.SQLiteOpenFlags.SharedCache
    | SQLite.SQLiteOpenFlags.NoMutex
    | SQLite.SQLiteOpenFlags.ProtectionCompleteUntilFirstUserAuthentication;
    public static string JsonDatabasePath => Path.Combine(DatabaseDirectory, JsonDatabaseFilename);
    public static string SqlDatabasePath => Path.Combine(DatabaseDirectory, DatabaseFilename);
}