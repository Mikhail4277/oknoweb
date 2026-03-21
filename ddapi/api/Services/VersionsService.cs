namespace api.Services;

public sealed class VersionsService : IVersionsService
{
    private readonly IConfig Config;

    public IDatabaseWriter Writer { get; set; }
    public IDatabaseReader Reader { get; set; }

    public VersionsService(IConfig config, IDatabaseReader reader, IDatabaseWriter writer)
    {
        Config = config;
        Reader = reader;
        Writer = writer;
    }
 
    public async Task<VersionInfo[]> GetVersions()
    {
        return await Reader.ReadAllVersionsInfo();
    }

    public async Task<VersionInfo> GetVersion(string id)
    {
        return await Reader.ReadVersionInfo(id);
    }

    public async Task<FileStream> GetVersionFile(string versionId)
    {
        VersionInfo info = await GetVersion(versionId);

        return new FileStream(info.Path, FileMode.Open, FileAccess.Read);
    }

    public async Task<bool> SaveOnDiskAndRegister(IFormFile formFile, string id, string changelog)
    {
        try
        {
            string path = Path.Combine(Config.ArchiveMainPath, id + ".zip");
            
            using (var stream = File.Create(path))
            {
                await formFile.CopyToAsync(stream);
            }
            
            VersionInfo info = new(id, path, changelog, DateTime.Today.Date.ToString());

            await Writer.RegisterVersion(info);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed writing file on disk: {0}", ex);
            return false;
        }
    }
}