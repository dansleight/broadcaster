public enum MusicType
{
    Default,
    Sacrament
}

public static class MusicTypeExtensions
{
    public static string FileDirectory(this MusicType musicType)
    {
        return $"{musicType.ToString().ToLower()}";
    }
}