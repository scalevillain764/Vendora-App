namespace Application.DTO.AuthDTO
{
    public record UserRegistrationResponseDTO
    (
        Ulid UserId,
        string Login
    );
}