using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Trofi.io.Client.Extensions;

public static class BrowseFileExtensions
{
    /// <summary>
    /// This method takes the chosen file by the user on the web page and converts it into an IFormFile
    /// which can be sent to the file uploads controller.
    /// </summary>
    /// <param name="browserFile"></param>
    /// <returns></returns>
    public static async Task<IFormFile> CovertToIFormFileAsync(this IBrowserFile browserFile, int MaxAllowedFileSize)
    {
        Stream stream = browserFile.OpenReadStream(MaxAllowedFileSize);
        MemoryStream memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);
        return new FormFile(memoryStream, 0, memoryStream.Length, "file", browserFile.Name);
    }
}
