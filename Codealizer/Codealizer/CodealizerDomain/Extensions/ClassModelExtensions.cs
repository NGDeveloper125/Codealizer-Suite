using CodealizerDomain.Models;

namespace CodealizerDomain.Extensions;

public static class ClassModelExtensions
{
    public static string[] GetClassesByProject(this List<ClassModel> classModels, string projectName)
    {
        return classModels
            .FindAll(c => c.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase))
            .Select(c => c.ClassName)
            .Distinct()
            .ToArray();
    }
}
