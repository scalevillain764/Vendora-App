using Amazon.S3;
using Amazon.S3.Model;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.ErrorTypes;
using IS3Service = Application.Interfaces.IS3Service;
namespace Application.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _S3Client;
        public S3Service (IAmazonS3 S3Client)
        {
            _S3Client = S3Client;
        } 
        public async Task<Result<string>> UploadPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Result<string>.Error("Файл не выбран или пуст", ErrorType.Validation);

            using var memoryStream = file.OpenReadStream();

            string? bucketName = Environment.GetEnvironmentVariable("GARAGE_BUCKET_NAME");

            if (bucketName == null)
                return Result<string>.Error("Проверьте корректность данных", ErrorType.Validation);

            var fileName = $"uploads/{Guid.NewGuid()}_{file.FileName}";

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = memoryStream,
                ContentType = file.ContentType
            };

            await _S3Client.PutObjectAsync(request);

            string? baseGarageURL = Environment.GetEnvironmentVariable("GARAGE_BASE_URL");

            if (baseGarageURL == null)
                return Result<string>.Error("Проверьте корректность данных", ErrorType.Validation);

            return Result<string>.Success($"{baseGarageURL}/{bucketName}/{fileName}");
        }
    }
}