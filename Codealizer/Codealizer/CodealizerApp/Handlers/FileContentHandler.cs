using CodealizerDomain.GeneralModels;
using Microsoft.AspNetCore.Components.Forms;

namespace CodealizerApp.Handlers;

public class FileContentHandler
{
    public static async Task<Result<string?>> ReadFileContentAsync(IBrowserFile file)
    {
        if (file == null)
        {
            return new Result<string?> { IsSuccess = false, ErrorMessage = "No file provided." };
        }

        const long maxFileSize = 5 * 1024 * 1024;

        if (file.Size > maxFileSize)
        {
            return new Result<string?> { IsSuccess = false, ErrorMessage = $"File {file.Name} size exceeds the maximum limit of {maxFileSize / (1024 * 1024)} MB." };
        }

        try
        {
            using var stream = file.OpenReadStream(maxFileSize);
            using var reader = new StreamReader(stream);
            return new Result<string?> { IsSuccess = true, Data = await reader.ReadToEndAsync() };
        }
        catch (Exception ex)
        {
            return new Result<string?> { IsSuccess = false, ErrorMessage = $"An error occurred while reading the file {file.Name} - {ex.Message}" };
        }
    }
}
