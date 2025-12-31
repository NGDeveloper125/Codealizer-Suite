using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodealizerDomain.Services;

public class ClassModelService
{
    public static Result<ClassModel> BuildClassModel(BaseTypeDeclarationSyntax typeDecl, string nameSpace)
    {
        try
        {
            List<MethodModel> publicMethods = new List<MethodModel>();
            List<MethodModel> privateMethods = new List<MethodModel>();

            // Only TypeDeclarationSyntax types (classes, interfaces, structs) have members with methods
            // Enums don't have methods
            if (typeDecl is TypeDeclarationSyntax typeDeclWithMembers)
            {
                Result<List<MethodModel>> publicMethodsResult = MethodModelService.BuildMethodModels(typeDeclWithMembers, SyntaxKind.PublicKeyword);
                if (publicMethodsResult.IsSuccess)
                {
                    publicMethods = publicMethodsResult.Data!;
                }
                else
                {
                    return Result<ClassModel>.Failure(publicMethodsResult.ErrorMessage!);
                }

                Result<List<MethodModel>> privateMethodsResult = MethodModelService.BuildMethodModels(typeDeclWithMembers, SyntaxKind.PrivateKeyword);
                if (privateMethodsResult.IsSuccess)
                {
                    privateMethods = privateMethodsResult.Data!;
                }
                else
                {
                    return Result<ClassModel>.Failure(privateMethodsResult.ErrorMessage!);
                }
            }

            return Result<ClassModel>.Success(new ClassModel
            {
                ClassId = Guid.NewGuid(),
                ClassName = typeDecl.Identifier.Text,
                Namespace = nameSpace,
                IsAbstract = typeDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)),
                IsInterface = typeDecl is InterfaceDeclarationSyntax,
                IsEnum = typeDecl is EnumDeclarationSyntax,
                IsInheriting = typeDecl.BaseList?.Types.Count > 0,
                PublicMethods = publicMethods,
                PrivateMethods = privateMethods
            });
        }
        catch (Exception ex)
        {
            return Result<ClassModel>.Failure($"Error building ClassModel: {ex.Message}");
        }
    }
}