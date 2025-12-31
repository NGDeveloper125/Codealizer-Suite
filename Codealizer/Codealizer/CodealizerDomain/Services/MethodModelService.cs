using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerDomain.Services;

public class MethodModelService
{
    public static Result<List<MethodModel>> BuildMethodModels(TypeDeclarationSyntax typeDecl, SyntaxKind accessModifier)
    {
        try
        {
            return Result<List<MethodModel>>.Success(typeDecl.Members
                       .OfType<MethodDeclarationSyntax>()
                       .Where(m => HasModifier(m.Modifiers, accessModifier))
                       .Select(m => new MethodModel
                       {
                           MethodId = Guid.NewGuid(),
                           MethodName = m.Identifier.Text,
                           CalledMethods = ExtractMethodCalls(m)
                       })
                       .ToList());
        }
        catch (Exception ex)
        {
            return Result<List<MethodModel>>.Failure($"Error building MethodModel: {ex.Message}");
        }
    }

    private static bool HasModifier(SyntaxTokenList modifiers, SyntaxKind kind)
    {
        // If no explicit modifier, it's private by default in classes
        if (!modifiers.Any() && kind == SyntaxKind.PrivateKeyword)
            return true;

        return modifiers.Any(m => m.IsKind(kind));
    }

    private static List<(Guid, string)> ExtractMethodCalls(MethodDeclarationSyntax method)
    {
        var methodCalls = method.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(invocation =>
            {
                var methodName = GetMethodName(invocation);
                var parameters = string.Join(", ", invocation.ArgumentList.Arguments);

                // Store with Guid.Empty for now - will resolve in second pass
                return (Guid.Empty, $"{methodName}({parameters})");
            })
            .ToList();

        return methodCalls;
    }

    private static string GetMethodName(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax mae => mae.Name.Identifier.Text,
            IdentifierNameSyntax ins => ins.Identifier.Text,
            GenericNameSyntax gns => gns.Identifier.Text,
            _ => "Unknown"
        };
    }

    // Extract object creations for ChildClasses
    private static List<string> ExtractObjectCreations(TypeDeclarationSyntax typeDecl)
    {
        return typeDecl.DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .Select(creation => creation.Type.ToString())
            .Distinct()
            .ToList();
    }

    // Extract static method calls for StaticClassesCalls
    private static List<string> ExtractStaticCalls(TypeDeclarationSyntax typeDecl)
    {
        return typeDecl.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv => inv.Expression is MemberAccessExpressionSyntax mae
                && mae.Expression is IdentifierNameSyntax) // Indicates static call
            .Select(inv => ((MemberAccessExpressionSyntax)inv.Expression).Expression.ToString())
            .Distinct()
            .ToList();
    }
}
