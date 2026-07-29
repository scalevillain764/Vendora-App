using System.Text.Json.Serialization;

namespace Application.DTO.ExchangeRateDTO
{
    public record ExchangeApiResponseDTO
    ([property: JsonPropertyName("conversion_rates")]
        Dictionary<string, decimal> ConversionRates)
    {
        public ExchangeApiResponseDTO() : this(new Dictionary<string, decimal>()) { }
    }
}