using HomeCare.Services.Result;

namespace HomeCare.Services.FileService
{
    public interface IFileService
    {
        Task<ServiceResult<string>> SaveFileAsync(IFormFile file, string FolderName);
        Task<ServiceResult<SaveFileArrayResult>> SaveFileAsync(List<IFormFile> files, string FolderName);
        ServiceResult<string> DeleteFileAsync(string filepath);
        ServiceResult<string> DeleteFileAsync(List<string> filepaths);

    }
}
