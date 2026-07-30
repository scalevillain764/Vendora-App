using Application.Result;
namespace Application.Interfaces
{
    public interface IS3Service
    {
        Task<Result<string>> UploadPhotoAsync(IFormFile file);
        Task<Result<List<string>>> UploadPhotosAsync(List<IFormFile> files);
        Task<Result<string>> RemovePhotoByUrlAsync(string fileURL);
    }
}