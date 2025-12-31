using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using CodealizerDomain.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodealizerTests;

public class ClassModelServiceTests
{
    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithAnEmptyPublicClass_WhenTypeDeclarationSyntaxRepresntingAnEmptyPublicClassIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class
        string classCode = @"
            public class TestClass
            {
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel representing the class
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("TestClass", buildClassModelResult.Data!.ClassName);
        Assert.Equal("TestNamespace", buildClassModelResult.Data!.Namespace);
        Assert.False(buildClassModelResult.Data!.IsAbstract);
        Assert.False(buildClassModelResult.Data!.IsInterface);
        Assert.False(buildClassModelResult.Data!.IsEnum);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithAPublicClassWithOneMethod_WhenTypeDeclarationSyntaxRepresntingAPublicClassWithOneMethodIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class
        string classCode = @"
            public class TestClass
            {
                public void TestMethod() { }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel representing the class
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("TestClass", buildClassModelResult.Data!.ClassName);
        Assert.Single(buildClassModelResult.Data!.PublicMethods);
        Assert.Equal("TestMethod", buildClassModelResult.Data!.PublicMethods[0].MethodName);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithAbstractClass_WhenTypeDeclarationSyntaxRepresentingAnAbstractClassIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing an abstract class
        string classCode = @"
            public abstract class AbstractTestClass
            {
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel with IsAbstract set to true
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("AbstractTestClass", buildClassModelResult.Data!.ClassName);
        Assert.True(buildClassModelResult.Data!.IsAbstract);
        Assert.False(buildClassModelResult.Data!.IsInterface);
        Assert.False(buildClassModelResult.Data!.IsEnum);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithInterface_WhenTypeDeclarationSyntaxRepresentingAnInterfaceIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing an interface
        string classCode = @"
            public interface ITestInterface
            {
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel with IsInterface set to true
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("ITestInterface", buildClassModelResult.Data!.ClassName);
        Assert.True(buildClassModelResult.Data!.IsInterface);
        Assert.False(buildClassModelResult.Data!.IsAbstract);
        Assert.False(buildClassModelResult.Data!.IsEnum);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithEnum_WhenTypeDeclarationSyntaxRepresentingAnEnumIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing an enum
        string classCode = @"
            public enum TestEnum
            {
                Value1,
                Value2
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel with IsEnum set to true
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("TestEnum", buildClassModelResult.Data!.ClassName);
        Assert.True(buildClassModelResult.Data!.IsEnum);
        Assert.False(buildClassModelResult.Data!.IsAbstract);
        Assert.False(buildClassModelResult.Data!.IsInterface);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithInheritanceFlag_WhenTypeDeclarationSyntaxRepresentingAClassInheritingFromBaseClassIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with inheritance
        string classCode = @"
            public class DerivedClass : BaseClass
            {
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel with IsInheriting set to true
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("DerivedClass", buildClassModelResult.Data!.ClassName);
        Assert.True(buildClassModelResult.Data!.IsInheriting);
    }

    [Fact]
    public void BuildClassModel_ShouldReturnAClassModelWithMultipleMethods_WhenTypeDeclarationSyntaxRepresentingAClassWithMultiplePublicAndPrivateMethodsIsPassedIn()
    {
        // Given a valid TypeDeclarationSyntax representing a class with multiple methods
        string classCode = @"
            public class TestClass
            {
                public void PublicMethod1() { }
                public int PublicMethod2() { return 0; }
                private void PrivateMethod1() { }
                private string PrivateMethod2() { return string.Empty; }
            }";
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);
        BaseTypeDeclarationSyntax typeDeclaration = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();
        string nameSpace = "TestNamespace";

        // When BuildClassModel is called with the TypeDeclarationSyntax
        Result<ClassModel> buildClassModelResult = ClassModelService.BuildClassModel(typeDeclaration, nameSpace);

        // Then it should return a ClassModel with correct method counts
        Assert.True(buildClassModelResult.IsSuccess);
        Assert.NotNull(buildClassModelResult.Data!);
        Assert.Equal("TestClass", buildClassModelResult.Data!.ClassName);
        Assert.Equal(2, buildClassModelResult.Data!.PublicMethods.Count);
        Assert.Equal(2, buildClassModelResult.Data!.PrivateMethods.Count);
        Assert.Contains(buildClassModelResult.Data!.PublicMethods, m => m.MethodName == "PublicMethod1");
        Assert.Contains(buildClassModelResult.Data!.PublicMethods, m => m.MethodName == "PublicMethod2");
        Assert.Contains(buildClassModelResult.Data!.PrivateMethods, m => m.MethodName == "PrivateMethod1");
        Assert.Contains(buildClassModelResult.Data!.PrivateMethods, m => m.MethodName == "PrivateMethod2");
    }
}
