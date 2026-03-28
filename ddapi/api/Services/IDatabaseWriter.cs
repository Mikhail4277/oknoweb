namespace api.Services;

public interface IDatabaseWriter
{
    Task         RegisterVersion     (LocalVersionInfo info);
    Task         EditVersion         (string id, LocalVersionInfo info);
    Task         DeleteVersion       (string id);
}