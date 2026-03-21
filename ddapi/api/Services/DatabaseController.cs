using Microsoft.Data.Sqlite;

namespace api.Services;

public sealed class DatabaseController : IDatabaseReader, IDatabaseWriter
{
    private readonly IConfig Config;

    public DatabaseController(IConfig config)
    {
        Config = config;
    }

    public Task<bool> ValidateUserAuth(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<VersionInfo> ReadVersionInfo(string id)
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");

        await connection.OpenAsync();

        using SqliteCommand command = new SqliteCommand($"SELECT * FROM main WHERE id = '{id}'", connection);
        
        using SqliteDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new VersionInfo(
            reader["id"]           as string,
            reader["path"]         as string,
            reader["changelog"]    as string,
            reader["release_date"] as string);
    }

    public async Task<VersionInfo[]> ReadAllVersionsInfo()
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");
        
        await connection.OpenAsync();

        using SqliteCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT * FROM main";
        
        using SqliteDataReader reader = command.ExecuteReader();

        List<VersionInfo> infos = new();
        
        while (await reader.ReadAsync())
        {
            infos.Add(new VersionInfo(
                reader["id"]           as string, 
                reader["path"]         as string,
                reader["changelog"]    as string, 
                reader["release_date"] as string));
        }

        return infos.ToArray();
    }

    public async Task RegisterVersion(VersionInfo info)
    {
        SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");
        await connection.OpenAsync();

        var insertCommand = $"insert into main ({Config.IDColumn}, {Config.PathColumn}, {Config.ChangelogColumn}, {Config.ReleaseDateColumn}) values(@id, @path, @changelog, @release_date)";
        SqliteCommand command = new(insertCommand, connection);
        command.Parameters.AddWithValue("@id", info.ID);
        command.Parameters.AddWithValue("@path", info.Path);
        command.Parameters.AddWithValue("@changelog", info.Changelog);
        command.Parameters.AddWithValue("@release_date", info.ReleaseDate);

        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }

    public async Task EditVersion(string versionID, VersionInfo info)
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");
        
        await connection.OpenAsync();

        SqliteCommand command = new(
            $"update main" +
            $"set {Config.IDColumn} = @id, {Config.PathColumn} = @path, {Config.ChangelogColumn} = @changelog, {Config.ReleaseDateColumn}) = @release_date" +
            $"where {Config.IDColumn} = '{versionID}'"
        );
        command.Parameters.AddWithValue("@id", info.ID);
        command.Parameters.AddWithValue("@path", info.Path);
        command.Parameters.AddWithValue("@changelog", info.Changelog);
        command.Parameters.AddWithValue("@release_date", info.ReleaseDate);

        await command.ExecuteNonQueryAsync();
    }

    public async Task EditVersionPath(string versionID, string newPath)
    {
        VersionInfo info = await ReadVersionInfo(versionID);
        info.Path = newPath;
        
        await EditVersion(versionID, info);
    }

    public async Task EditVersionChangelog(string versionID, string newChangelog)
    {
        VersionInfo info = await ReadVersionInfo(versionID);
        info.Changelog = newChangelog;
        
        await EditVersion(versionID, info);
    }

    public async Task EditVersionID(string versionID, string newID)
    {
        VersionInfo info = await ReadVersionInfo(versionID);
        info.ID = newID;
        
        await EditVersion(versionID, info);
    }

    public async Task DeleteVersion(string id)
    {
        using SqliteConnection connection = new SqliteConnection($"Data Source={Config.DatabasePath}");
        
        await connection.OpenAsync();

        SqliteCommand command = new($"delete from main where {Config.IDColumn} = '{id}'");

        await command.ExecuteNonQueryAsync();
    }
}