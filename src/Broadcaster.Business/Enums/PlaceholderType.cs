public enum PlaceholderType
{
    Pre = 1,
    Sacrament = 2,
    Post = 3
}

public static class PlaceholderTypeExtensions
{
    public static string FilePath(this PlaceholderType placeholderType)
    {
        return $"{placeholderType.ToString().ToLower()}.jpg";
    }
}