using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using CodealizerDomain.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodealizerTests;

public class MethodModelServiceTests
{
    [Fact]
    public void BuildMethodModel_ShouldReturnEmptyList_WhenTypeDeclarationSyntaxRepresentingAnEmptyClassIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing an empty class
        string classCode = @"
            public class TestClass
            {
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return an empty list
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Empty(buildMethodModelResult.Data!);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithOneMethod_WhenTypeDeclarationSyntaxRepresentingAClassWithOnePublicMethodIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with one public method
        string classCode = @"
            public class TestClass
            {
                public void TestMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with one MethodModel representing the method
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("TestMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.NotEqual(Guid.Empty, buildMethodModelResult.Data![0].MethodId);
        Assert.Empty(buildMethodModelResult.Data![0].CalledMethods);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithOneMethod_WhenTypeDeclarationSyntaxRepresentingAClassWithOnePrivateMethodIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with one private method
        string classCode = @"
            public class TestClass
            {
                private void PrivateMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PrivateKeyword);

        // Then it should return a list with one MethodModel representing the private method
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("PrivateMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.NotEqual(Guid.Empty, buildMethodModelResult.Data![0].MethodId);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnEmptyList_WhenTypeDeclarationSyntaxRepresentingAClassWithPublicMethodButPrivateModifierIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with a public method
        string classCode = @"
            public class TestClass
            {
                public void PublicMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with PrivateKeyword modifier
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PrivateKeyword);

        // Then it should return an empty list because the public method is ignored
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Empty(buildMethodModelResult.Data!);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnEmptyList_WhenTypeDeclarationSyntaxRepresentingAClassWithPrivateMethodButPublicModifierIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with a private method
        string classCode = @"
            public class TestClass
            {
                private void PrivateMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with PublicKeyword modifier
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return an empty list because the private method is ignored
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Empty(buildMethodModelResult.Data!);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithMultipleMethods_WhenTypeDeclarationSyntaxRepresentingAClassWithMultiplePublicMethodsIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with multiple public methods
        string classCode = @"
            public class TestClass
            {
                public void Method1() { }
                public void Method2() { }
                public void Method3() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with three MethodModels
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Equal(3, buildMethodModelResult.Data!.Count);
        Assert.Contains(buildMethodModelResult.Data!, m => m.MethodName == "Method1");
        Assert.Contains(buildMethodModelResult.Data!, m => m.MethodName == "Method2");
        Assert.Contains(buildMethodModelResult.Data!, m => m.MethodName == "Method3");
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithMethodsContainingCalledMethods_WhenTypeDeclarationSyntaxRepresentingAClassWithMethodCallsIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with method calls
        string classCode = @"
            public class TestClass
            {
                public void CallerMethod() 
                {
                    CalledMethod();
                }
                
                private void CalledMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with one MethodModel with one called method tracked
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("CallerMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.Single(buildMethodModelResult.Data![0].CalledMethods);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithMethodsContainingMultipleCalledMethods_WhenTypeDeclarationSyntaxRepresentingAClassWithMultipleMethodCallsIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with multiple method calls
        string classCode = @"
            public class TestClass
            {
                public void CallerMethod() 
                {
                    Method1();
                    Method2();
                    Method3();
                }
                
                private void Method1() { }
                private void Method2() { }
                private void Method3() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with one MethodModel with three called methods tracked
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("CallerMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.Equal(3, buildMethodModelResult.Data![0].CalledMethods.Count);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithMethodsContainingParametersInCalledMethods_WhenTypeDeclarationSyntaxRepresentingAClassWithMethodCallsWithParametersIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with method calls with parameters
        string classCode = @"
            public class TestClass
            {
                public void CallerMethod() 
                {
                    CalledMethod(10, ""test"");
                }
                
                private void CalledMethod(int num, string text) { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with one MethodModel with parameters tracked in called method
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("CallerMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.Single(buildMethodModelResult.Data![0].CalledMethods);
        (Guid methodId, string parameters) = buildMethodModelResult.Data![0].CalledMethods[0];
        Assert.Contains("10", parameters);
        Assert.Contains("test", parameters);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithMethodsContainingMemberAccessMethodCall_WhenTypeDeclarationSyntaxRepresentingAClassWithObjectMethodCallIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with method call on an object
        string classCode = @"
            public class TestClass
            {
                public void CallerMethod() 
                {
                    var obj = new OtherClass();
                    obj.OtherMethod();
                }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with the TypeDeclarationSyntax
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PublicKeyword);

        // Then it should return a list with one MethodModel with member access method call tracked
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("CallerMethod", buildMethodModelResult.Data![0].MethodName);
        Assert.Single(buildMethodModelResult.Data![0].CalledMethods);
    }

    [Fact]
    public void BuildMethodModel_ShouldReturnListWithImplicitPrivateMethod_WhenTypeDeclarationSyntaxRepresentingAClassWithMethodWithoutAccessModifierIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with method without access modifier
        string classCode = @"
            public class TestClass
            {
                void ImplicitPrivateMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        TypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .First();

        // When BuildMethodModel is called with PrivateKeyword modifier
        Result<List<MethodModel>> buildMethodModelResult = MethodModelService.BuildMethodModels(typeDeclaration, SyntaxKind.PrivateKeyword);

        // Then it should return a list with one MethodModel for the implicit private method
        Assert.True(buildMethodModelResult.IsSuccess);
        Assert.NotNull(buildMethodModelResult.Data!);
        Assert.Single(buildMethodModelResult.Data!);
        Assert.Equal("ImplicitPrivateMethod", buildMethodModelResult.Data![0].MethodName);
    }
}