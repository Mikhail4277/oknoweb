namespace api.Services;

public interface IDatabaseReader
{
    Task<bool>          ValidateUserAuth(string username, string password);
    Task<VersionInfo>   ReadVersionInfo(string id);
    Task<VersionInfo[]> ReadAllVersionsInfo();
}