using System.Text.Json.Serialization;

namespace Application.DTO.PaymentDTO
{
    public record PaymentYooKassaRequestDTO(
        [property: JsonPropertyName("object")]
        ObjectDTO obj
        );

    public record ObjectDTO(
        [property: JsonPropertyName("id")]
        string Id
       );
}