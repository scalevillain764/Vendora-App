using System.Text.Json.Serialization;

namespace Application.DTO.ExchangeRateDTO
{
    public record ExchangeUserResponseDTO(Dictionary<string, decimal> ConversionRates);
}