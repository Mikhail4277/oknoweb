namespace api.Services;

public interface IDatabaseWriter
{
    Task RegisterVersion      (VersionInfo info);
    Task EditVersion          (string id, VersionInfo info);
    Task EditVersionPath      (string versionID, string newPath);
    Task EditVersionChangelog (string versionID, string newChangelog);
    Task EditVersionID        (string versionID, string newID);
    Task DeleteVersion        (string id);
}