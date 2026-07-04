namespace OnionSetUp.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder=FilePaths.ProductFolder);
        Task<bool> DeleteImageAsync(string imageUrl);
        bool IsImageFile(IFormFile file);
    }
}