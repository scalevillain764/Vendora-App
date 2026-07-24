using Application.Result;
using Domain.ErrorTypes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Presentation.Controllers
{
    [ApiController]
    public class BaseController: ControllerBase
    {
        protected Ulid CurrentUserId => ExtractUserIdFromClaims();
        private Ulid ExtractUserIdFromClaims()
            => Ulid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) 
            ? userId : throw new UnauthorizedAccessException();
        protected IActionResult ProcessResult<T> (Result<T> rez) where T : class
        {
            if(!rez.IsSuccess)
            {
                return rez.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(rez.ErrorMessage),
                    ErrorType.Validation => BadRequest(rez.ErrorMessage),
                    ErrorType.Forbidden => Forbid(),
                    ErrorType.Unauthorized => Unauthorized(rez.ErrorMessage),
                    ErrorType.Conflict => Conflict(rez.ErrorMessage)
                };
            }
            return Ok(rez.data);
        }
    }
}