using Amazon.S3.Transfer;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ImageController: BaseController
    {
        private readonly IS3Service _service;
        public ImageController(IS3Service service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> UploadPictureAsync(IFormFile file)
            => ProcessResult(await _service.UploadPhotoAsync(file));

        [HttpPost]
        [Route("images")]
        public async Task<IActionResult> UploadPicturesAsync(List<IFormFile> files)
            => ProcessResult(await _service.UploadPhotosAsync(files));
    }
}