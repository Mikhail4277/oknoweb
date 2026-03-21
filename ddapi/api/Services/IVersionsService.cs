namespace api.Services;

public interface IVersionsService
{
    Task<VersionInfo[]> GetVersions           ();    

    Task<VersionInfo>   GetVersion            (string id);
    Task<FileStream>    GetVersionFile        (string versionId);
    Task<bool>          SaveOnDiskAndRegister (IFormFile formFile, string id, string changelog);
}