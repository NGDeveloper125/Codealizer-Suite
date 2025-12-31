using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerDomain.Models;

public record ClassModel
{
    public Guid ClassId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string ClassName { get; init; } = string.Empty;
    public string Namespace { get; init; } = string.Empty;
    public bool IsAbstract { get; init; } 
    public bool IsInterface { get; init; }
    public bool IsEnum { get; init; }
    public bool IsInheriting { get; init; }
    public Guid? BaseClassId { get; init; } = null;
    public List<Guid> ParentClasses { get; init; } = new(); // list of classes that create an instances of this class 
    public List<Guid> ChildClasses { get; init; } = new(); // list of classes that this class creates an instance of
    public List<Guid> StaticClassesCalls { get; init; } = new(); // list of static classes that this class calls
    public List<MethodModel> PublicMethods { get; init; } = new(); 
    public List<MethodModel> PrivateMethods { get; init; } = new();
}
