using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerDomain.Models;

public record MethodModel
{
    public Guid MethodId { get; init; }
    public string MethodName { get; init; } = string.Empty;
    public List<(Guid, string)> CalledMethods { get; init; } = new(); // list of (MethodId, Parameters as one string) that this method calls
}
