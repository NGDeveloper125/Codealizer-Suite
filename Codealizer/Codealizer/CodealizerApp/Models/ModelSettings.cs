namespace CodealizerApp.Models;

public class ModelSettings
{
    public bool ShowPublicMethods { get; set; } = true;
    public bool ShowPrivateMethods { get; set; } = true;
    public List<string> ProjectsNotToShow { get; set; } = new();
    public List<string> ClassesNotToShow { get; set; } = new();
}
