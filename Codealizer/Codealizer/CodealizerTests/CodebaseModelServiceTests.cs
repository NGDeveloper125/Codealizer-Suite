using CodealizerDomain.GeneralModels;
using CodealizerDomain.Models;
using CodealizerDomain.Services;

namespace CodealizerTests;

public class CodebaseModelServiceTests
{
    [Fact]
    public void BuildCodebaseModel_ShouldReturnUpdatedCodebaseModel_WhenNewFileContentIsPassedIn()
    {
        // Given an empty codebase model and a file content that was not yet processed
        CodebaseModel codebaseModel = new CodebaseModel();
        string newFileContent = @"
            using System;
            Namespace SampleNamespace; 
            
            public class SampleClass 
            {
                public void SampleMethod() 
                {
                    // method implementation
                }
            }";
        
        // When BuildCodebaseModel is called with the new file content, the codebase model and the list of already processed files
        Result<CodebaseModel> buildCodebaseModelResult = CodebaseModelService.BuildCodebaseModel(newFileContent, codebaseModel);

        // Then it should return an updated codebase model that contain the classes and methods found in the new file content and the list of already processed files should include the new file
        Assert.True(buildCodebaseModelResult.IsSuccess);
        CodebaseModel updatedCodebaseModel = buildCodebaseModelResult.Data!;
        Assert.Single(updatedCodebaseModel.Classes);
        ClassModel sampleClass = updatedCodebaseModel.Classes[0];
        Assert.Equal("SampleClass", sampleClass.ClassName);
        Assert.Single(sampleClass.PublicMethods);
        MethodModel sampleMethod = sampleClass.PublicMethods[0];
        Assert.Equal("SampleMethod", sampleMethod.MethodName);
    }
}
