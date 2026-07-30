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

        public async Task<Result<List<string>>> UploadPhotosAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return Result<List<string>>.Error("Проверьте корректность файлов", ErrorType.Validation);

            var fileURLS = new List<string>();

            foreach(var file in files)
            {
                if (file == null) continue;
                var url = await UploadPhotoAsync(file);
                
                if(!url.IsSuccess)
                {
                    Console.WriteLine($"Ошибка загрузки фото: {file.FileName}");
                    continue;
                }

                fileURLS.Add(url.data);
            }

            return Result<List<string>>.Success(fileURLS);
        }

        public async Task<Result<string>> RemovePhotoByUrlAsync(string fileURL)
        {
            if (string.IsNullOrEmpty(fileURL))  
                return Result<string>.Error("Cсылка не найдена", ErrorType.NotFound);

            string? bucketName = Environment.GetEnvironmentVariable("GARAGE_BUCKET_NAME");

            if (bucketName == null)
                return Result<string>.Error("Бакет не найден", ErrorType.NotFound);

            try
            {
                string marker = $"/{bucketName}/";

                int index = fileURL.IndexOf(marker);
                if (index == -1)
                    return Result<string>.Error("Проверьте корректность бакета", ErrorType.Validation);

                string fileName = fileURL.Substring(index + marker.Length);

                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName
                };

                await _S3Client.DeleteObjectAsync(request);
                return Result<string>.Success(fileURL);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result<string>.Error(ex.Message, ErrorType.Conflict);
            }
        }
    }
}