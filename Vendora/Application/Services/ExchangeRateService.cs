using IExchangeRateService = Application.Interfaces.IExchangeRateService;
using Application.DTO.ExchangeRateDTO;
using Application.Result;
using Domain.ErrorTypes;
namespace Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientFactory _factory;
        public ExchangeRateService(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public async Task<Result<ExchangeUserResponseDTO>> GetExchangeRatesAsync()
        {
            var client = _factory.CreateClient();

            string? API_KEY = Environment.GetEnvironmentVariable("EXCHANGE_RATES_API_KEY");
            string? API_BASE_URL = Environment.GetEnvironmentVariable("EXCHANGE_RATES_BASE_URL");
            string? API_ENDPOINT_SUFFIX = Environment.GetEnvironmentVariable("EXCHANGE_RATES_ENDPOINT_SUFFIX");

            if (API_KEY == null || API_BASE_URL == null || API_ENDPOINT_SUFFIX == null)
                return Result<ExchangeUserResponseDTO>.Error("Проверьте целостность данных", ErrorType.Validation);

            string CONNECTION_URL = $"{API_BASE_URL}{API_KEY}{API_ENDPOINT_SUFFIX}";
            ExchangeApiResponseDTO responseDTO = new ExchangeApiResponseDTO();

            try
            {
                var response = await client.GetAsync(CONNECTION_URL);

                if(response.IsSuccessStatusCode)
                {
                    try
                    {
                        responseDTO = await response.Content.ReadFromJsonAsync<ExchangeApiResponseDTO>();
                    }
                    catch (OperationCanceledException oce)
                    {
                        Console.WriteLine($"Parse error: {oce.Message}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error during creating request: {ex.Message}");
            }

            if (responseDTO.ConversionRates.Count == 0)
                return Result<ExchangeUserResponseDTO>.Error("Ошибка загрузки валюты", ErrorType.Conflict);

            return Result<ExchangeUserResponseDTO>.Success(new ExchangeUserResponseDTO(responseDTO.ConversionRates));
        }
    }
}