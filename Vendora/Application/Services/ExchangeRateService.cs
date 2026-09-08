using IExchangeRateService = Application.Interfaces.IExchangeRateService;
using Application.DTO.ExchangeRateDTO;
using Application.Result;
using Domain.ErrorTypes;
namespace Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ExchangeRateService> _logger;
        public ExchangeRateService(IHttpClientFactory factory, ILogger<ExchangeRateService> logger)
        {
            _factory = factory;
            _logger = logger;
        }
        public async Task<Result<ExchangeUserResponseDTO>> GetExchangeRatesAsync(CancellationToken token)
        {
            List<string> target_keys = ["RUB", "USD", "BYN"];
            var client = _factory.CreateClient();

            string? API_KEY = Environment.GetEnvironmentVariable("EXCHANGE_RATES_API_KEY");
            string? API_BASE_URL = Environment.GetEnvironmentVariable("EXCHANGE_RATES_BASE_URL");
            string? API_ENDPOINT_SUFFIX = Environment.GetEnvironmentVariable("EXCHANGE_RATES_ENDPOINT_SUFFIX");

            if (string.IsNullOrEmpty(API_KEY) || string.IsNullOrEmpty(API_BASE_URL)|| string.IsNullOrEmpty(API_ENDPOINT_SUFFIX))
                return Result<ExchangeUserResponseDTO>.Error("Проверьте целостность данных", ErrorType.Validation);

            string CONNECTION_URL = $"{API_BASE_URL}{API_KEY}{API_ENDPOINT_SUFFIX}";
            ExchangeApiResponseDTO responseDTO = new ExchangeApiResponseDTO();

            try
            {
                var response = await client.GetAsync(CONNECTION_URL, token);

                if(response.IsSuccessStatusCode)
                {
                    try
                    {
                        responseDTO = await response.Content.ReadFromJsonAsync<ExchangeApiResponseDTO>(token);
                    }
                    catch (OperationCanceledException oce)
                    {
                        _logger.LogError(oce.Message);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Network error during creating request: " + ex.Message);
            }

            if (responseDTO!.ConversionRates.Count == 0)
                return Result<ExchangeUserResponseDTO>.Error("Ошибка загрузки валюты", ErrorType.Conflict);

            var rez = responseDTO.ConversionRates
                .Where(x => target_keys.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            return Result<ExchangeUserResponseDTO>.Success(new ExchangeUserResponseDTO(rez));
        }
    }
}