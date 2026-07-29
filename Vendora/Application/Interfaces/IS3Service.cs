using Application.Result;
namespace Application.Interfaces
{
    public interface IS3Service
    {
        Task<Result<string>> UploadPhotoAsync(IFormFile file);
        Task<Result<List<string>>> UploadProductPhotoAsync(List<IFormFile> files);
    }
}