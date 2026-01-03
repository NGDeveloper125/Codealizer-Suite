namespace CodealizerDomain.Extensions;

public static class StringExtensions
{
    public static string ExtractProjectNameFromNameSpace(this string nameSpace)
    {
        if (string.IsNullOrWhiteSpace(nameSpace))
        {
            return string.Empty;
        }
        string[] parts = nameSpace.Split('.');
        return parts.Length > 0 ? parts[0] : nameSpace;
    }
}
