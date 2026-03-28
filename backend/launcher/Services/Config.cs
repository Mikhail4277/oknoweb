using System.Text.Json;

namespace api.Services;

public sealed class Config : IConfig
{
    public string CONFIG_PATH => Path.Combine(Directory.GetCurrentDirectory(), "config");
    
    public string IDColumn          => "id";
    public string NameColumn        => "name";
    public string TagColumn         => "tag";
    public string PathColumn        => "path";
    public string ChangelogColumn   => "changelog";
    public string ReleaseDateColumn => "release_date";

    public string DatabasePath       { get; private set; }
    public string VersionArchivePath { get; private set; }

    private readonly string RootPath;

    public Config()
    {
        RootPath        = File.ReadAllText(CONFIG_PATH);
        RootPath        = RootPath.Replace("\n", "");

        DatabasePath       = Path.Combine(RootPath, "database.db");
        VersionArchivePath = Path.Combine(RootPath, "versions_archive");
        
        ValidatePath(RootPath);
        ValidatePath(DatabasePath);
    }

    private void ValidatePath(string path)
    {
        if (!Path.Exists(path))
        {
            Console.WriteLine($"Invalid path: {JsonSerializer.Serialize(path)}");
        }
    }
}