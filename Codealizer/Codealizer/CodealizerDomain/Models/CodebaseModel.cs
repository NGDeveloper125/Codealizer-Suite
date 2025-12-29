using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerDomain.Models;

public record CodebaseModel
{
    public List<ClassModel> Classes { get; init; } = new();
}
