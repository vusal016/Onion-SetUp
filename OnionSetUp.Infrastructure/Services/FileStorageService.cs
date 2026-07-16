namespace OnionSetUp.Infrastructure.Services
{
    public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB
        public bool IsImageFile(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return false;
            var extention = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extention);
        }
        public async Task<string> UploadImageAsync(IFormFile file, string folder = FilePaths.ProductFolder)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException(ErrorMessages.FileNotFound);
            if (!IsImageFile(file))
                throw new ArgumentException(ErrorMessages.InvalidFileFormat);
            if (file.Length > _maxFileSize)
                throw new ArgumentException(ErrorMessages.FileUploadFailed);
            var path = Path.Combine(environment.WebRootPath, FilePaths.UploadRoot, folder);
                Directory.CreateDirectory(path);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var filePath = Path.Combine(path, fileName);
            using var stream = File.Create(filePath);
            await file.CopyToAsync(stream);
            return $"/{FilePaths.UploadRoot}/{folder}/{fileName}";
        }
        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl == FilePaths.DefaultImage)
                return Task.FromResult(true);
            var relativePath = imageUrl.TrimStart('/');
            var physicalPath = Path.Combine(environment.WebRootPath, relativePath);

            if (!File.Exists(physicalPath))
                return Task.FromResult(false);

            File.Delete(physicalPath);
            return Task.FromResult(true);
        }

    }
}
