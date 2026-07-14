namespace Application.DTO.UserDTO
{
    public record UserChangePasswordDTO(
        string NewPassword,
        string OldPassword
    );
}