using Domain.Users;
namespace Application.DTO.AuthDTO
{
    public record AuthResponseDTO(
        Ulid UserId,
        string AccessToken
    );
}