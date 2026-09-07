using Application.DTO.AuthDTO;
using Application.DTO.CartDTO;
using Application.DTO.ProductDTO.CartDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Products;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Text.Json;
using ICartService = Application.Interfaces.ICartService;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;

        public CartService(AppDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis.GetDatabase();
        }
        private async Task SaveRedis(CartCacheResponseDTO cachedCart, Ulid UserId)
        {
            var serializedCart = JsonSerializer.Serialize(cachedCart);
            await _redis.StringSetAsync($"cart:user:{UserId}", serializedCart);
        }
        private async Task<Result<CartCacheResponseDTO>> GetCartInCacheAsync(Ulid UserId)
        {
            var cachedCart = await _redis.StringGetAsync($"cart:user:{UserId}");

            if (cachedCart.IsNullOrEmpty)
                return Result<CartCacheResponseDTO>.Error("Произошла ошибка получения корзины", ErrorType.Conflict);

            var deserializedCachedCart = JsonSerializer.Deserialize<CartCacheResponseDTO>((string)cachedCart!);

            if (deserializedCachedCart == null)
                return Result<CartCacheResponseDTO>.Error("Произошла ошибка получения корзины", ErrorType.Conflict);

            return Result<CartCacheResponseDTO>.Success(deserializedCachedCart);
        }
        public async Task<Result<CartResponseDTO>> GetMyCartAsync(Ulid UserId)
        {
            var cachedCartRequest = await GetCartInCacheAsync(UserId);

            if (!cachedCartRequest.IsSuccess)
                return Result<CartResponseDTO>.Error(cachedCartRequest.ErrorMessage!, (ErrorType)cachedCartRequest.ErrorType!);

            var cachedCart = cachedCartRequest.data!;

            var Ids = cachedCart.CartItems
                .Keys.ToList();

            var products = await _context.Products
                .Where(p => Ids.Contains(p.Id))
                .Select(p => 
                    new ProductCartCardResponseDTO(p.Id, p.Name, p.Price, p.ShortDescription, p.PreviewUrl, cachedCart.CartItems[p.Id]))
                .ToListAsync();

            var cartResponse = new CartResponseDTO(cachedCart.UserId, products, products.Count, products.Sum(p => p.Quantity * p.PricePerUnit));

            return Result<CartResponseDTO>.Success(cartResponse);
        }

        public async Task<Result<string>> RemoveProductFromCartAsync(Ulid UserId, Ulid ProductId)
        {
            var cachedCartRequest = await GetCartInCacheAsync(UserId);

            if (!cachedCartRequest.IsSuccess)
                return Result<string>.Error(cachedCartRequest.ErrorMessage!, (ErrorType)cachedCartRequest.ErrorType!);

            var cachedCart = cachedCartRequest.data!;

            cachedCart.CartItems.Remove(ProductId);
           
            return Result<string>.Success("OK");
        }

        public async Task<Result<ProductCartCardResponseDTO>> DecreaseQuantityAsync(Ulid UserId, Ulid ProductId)
        {
            var cachedCartRequest = await GetCartInCacheAsync(UserId);

            if (!cachedCartRequest.IsSuccess)
                return Result<ProductCartCardResponseDTO>.Error(cachedCartRequest.ErrorMessage!, (ErrorType)cachedCartRequest.ErrorType!);

            var cachedCart = cachedCartRequest.data!;

            if (!cachedCart.CartItems.ContainsKey(ProductId))
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            if (cachedCart.CartItems[ProductId] > 1)
            {
                cachedCart.CartItems[ProductId]--;
                await SaveRedis(cachedCart, UserId);
            }
              
            var product = await _context.Products
                .FindAsync(ProductId);

            if(product == null)
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            return Result<ProductCartCardResponseDTO>.Success(new ProductCartCardResponseDTO(product, cachedCart.CartItems[ProductId]));
        }

        public async Task<Result<ProductCartCardResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid ProductId)
        {
            var cachedCartRequest = await GetCartInCacheAsync(UserId);

            if (!cachedCartRequest.IsSuccess)
                return Result<ProductCartCardResponseDTO>.Error(cachedCartRequest.ErrorMessage!, (ErrorType)cachedCartRequest.ErrorType!);

            var cachedCart = cachedCartRequest.data!;

            if (!cachedCart.CartItems.ContainsKey(ProductId))
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            var product = await _context.Products
                .FindAsync(ProductId);

            if (product == null)
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            if (cachedCart.CartItems[ProductId] > product.Quantity)
            {
                cachedCart.CartItems[ProductId]++;
                await SaveRedis(cachedCart, UserId);
            }
         
            return Result<ProductCartCardResponseDTO>.Success(new ProductCartCardResponseDTO(product, cachedCart.CartItems[ProductId]));
        }

        public async Task<Result<ProductCartCardResponseDTO>> AddProductToCartAsync(Ulid UserId, Ulid ProductId)
        {
            var cachedCartRequest = await GetCartInCacheAsync(UserId);

            if (!cachedCartRequest.IsSuccess)
                return Result<ProductCartCardResponseDTO>.Error(cachedCartRequest.ErrorMessage!, (ErrorType)cachedCartRequest.ErrorType!);

            var cachedCart = cachedCartRequest.data!;

            if (!cachedCart.CartItems.ContainsKey(ProductId))
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            cachedCart.CartItems.Add(ProductId, 1);

            var product = await _context.Products
                .FindAsync(ProductId);

            if (product == null)
                return Result<ProductCartCardResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            await SaveRedis(cachedCart, UserId);

            return Result<ProductCartCardResponseDTO>.Success(new ProductCartCardResponseDTO(product, cachedCart.CartItems[ProductId]));
        }
    }
}
