using CodealizerDomain.Models;

namespace CodealizerDomain.Extensions;

public static class CodebaseModelExtensions
{
    public static string[] GetProjects(this CodebaseModel codebaseModel)
    { 
        return codebaseModel.Classes.Select(c => c.ProjectName).Distinct().ToArray();
    }
}
