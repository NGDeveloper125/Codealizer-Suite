using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerDomain.Services;

public class CodebaseModelService
{
    public static Result<CodebaseModel> BuildCodebaseModel(string fileContent, CodebaseModel codebaseModel)
    {
        try
        {
            SyntaxTree fileSyntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            CompilationUnitSyntax root = fileSyntaxTree.GetCompilationUnitRoot();

            // Extract namespace
            BaseNamespaceDeclarationSyntax namespaceDeclaration = root.DescendantNodes()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault();

            string namespaceName = namespaceDeclaration?.Name.ToString() ?? string.Empty;

            // Process all type declarations (classes, interfaces, enums)
            IEnumerable<BaseTypeDeclarationSyntax> typeDeclarations = root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>();

            foreach (BaseTypeDeclarationSyntax typeDecl in typeDeclarations)
            {
                Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDecl, namespaceName);

                if (!buildClassModelResult.IsSuccess)
                {
                    return Result<CodebaseModel>.Failure(buildClassModelResult.ErrorMessage);
                }

                if (buildClassModelResult.Data != null)
                {
                    codebaseModel.Classes.Add(buildClassModelResult.Data);
                }
            }

            return Result<CodebaseModel>.Success(codebaseModel);
        }
        catch (Exception ex)
        {
            return Result<CodebaseModel>.Failure($"Error parsing file: {ex.Message}");
        }
    }
}
