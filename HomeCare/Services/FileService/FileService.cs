using HomeCare.Services.Result;
using Microsoft.AspNetCore.Hosting;

namespace HomeCare.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        public FileService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ServiceResult<string>> SaveFileAsync(IFormFile file, string FolderName)
        {
            if (file == null || file.Length == 0)
                return new ServiceResult<string>("File is empty", ErrorTypeEnum.BAD_REQUEST);

            if (file.Length > 5000000)
                return new ServiceResult<string>("File is too large Max Size is 5MB", ErrorTypeEnum.BAD_REQUEST);

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
                return new ServiceResult<string>("Invalid file type", ErrorTypeEnum.BAD_REQUEST);

            var fileName = Guid.NewGuid() + extension;

            var root = _hostEnvironment.WebRootPath ?? _hostEnvironment.ContentRootPath;

            var folderPath = Path.Combine(root, "uploads", FolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var path = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            var finalPath = Path.Combine("uploads", FolderName, fileName)
                .Replace("\\", "/");

            return new ServiceResult<string>(finalPath);
        }

        public async Task<ServiceResult<SaveFileArrayResult>> SaveFileAsync(List<IFormFile> files, string FolderName)
        {

            if (files == null || files.Count == 0)
                return new ServiceResult<SaveFileArrayResult>("No files uploaded", ErrorTypeEnum.BAD_REQUEST);

            var result = new SaveFileArrayResult();
            foreach (var file in files)
            {
                var res = await SaveFileAsync(file, FolderName);

                if(res.Success)
                    result.FilePaths.Add(res.Data);
                else
                    result.Errors.Add(res.ErrorMessage);

            }

            if (result.FilePaths.Count == 0)
                return new ServiceResult<SaveFileArrayResult>("All uploads failed", ErrorTypeEnum.BAD_REQUEST);

            return new ServiceResult<SaveFileArrayResult>(result);
        }

        public  ServiceResult<string> DeleteFileAsync(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return new ServiceResult<string>("File is empty", ErrorTypeEnum.BAD_REQUEST);

            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, filepath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return new ServiceResult<string>("File deleted successfully");
            }
            return new ServiceResult<string>("File not found", ErrorTypeEnum.NOT_FOUND);
        }

        public ServiceResult<string> DeleteFileAsync(List<string> filepaths)
        {
            if (filepaths == null || filepaths.Count == 0)
                return new ServiceResult<string>("No files uploaded", ErrorTypeEnum.BAD_REQUEST);

            foreach (var filepath in filepaths)
            {
                var res = DeleteFileAsync(filepath);

                if (!res.Success)
                    return res;
            }

            return new ServiceResult<string>("Files deleted successfully");
        }
            
    }

    public class SaveFileArrayResult
    {
        public List<string> FilePaths { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
