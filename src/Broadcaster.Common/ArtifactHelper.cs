using Microsoft.Extensions.Configuration;

namespace Broadcaster.Common;

public class ArtifactHelper
{
    public readonly string ApplicationMode;
    public readonly string ArtifactPath;
    public readonly string DatabasePath;

    public ArtifactHelper(IConfiguration config)
    {
        var path = config.GetSection("Artifact:Path")?.Value ?? "/var/lib/broadcaster/";

        ApplicationMode = config.GetSection("ApplicationMode").Value ?? "Development";

        if (ApplicationMode == "Development")
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "var-lib/broadcaster/");

        Directory.CreateDirectory(Path.GetDirectoryName(path)!); // safety net

        string testPath = Path.Combine(path, "broadcaster-test-write.txt");
        string testText = $"broadcaster-test-write {DateTime.Now:T}";
        File.WriteAllText(testPath, testText);
        string readback = File.ReadAllText(testPath);
        File.Delete(testPath);
        if (testText != readback)
            throw new AccessViolationException($"File write test failed on artifact path: {path}");

        ArtifactPath = path;

        string dbFileName = config.GetSection("Database.FileName")?.Value ?? "broadcaster.db";

        DatabasePath = Path.Combine(path, dbFileName);
    }
}
