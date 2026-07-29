using Application.DTO.ExchangeRateDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<Result<ExchangeUserResponseDTO>> GetExchangeRatesAsync();
    }
}