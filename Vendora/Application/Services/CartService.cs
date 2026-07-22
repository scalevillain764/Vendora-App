using Application.DTO.ProductDTO.CartDTO;
using Application.Result;
using Domain.CartItems;
using Domain.ErrorTypes;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using ICartService = Application.Interfaces.ICartService;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }
        private async Task<Result<CartItem>> GetCartItemAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItem = await _context.CartItems
                 .Include(c => c.Product)
                    .Include(c => c.Cart)
                 .FirstOrDefaultAsync(x => x.Id == CartItemId);

            if (cartItem == null)
                return Result<CartItem>.Error("Товар в корзине не найден", ErrorType.NotFound);

            if (cartItem.Cart.UserId != UserId)
                return Result<CartItem>.Error("Товар не в вашей корзине", ErrorType.Conflict);

            return Result<CartItem>.Success(cartItem);
        }
        public async Task<Result<List<ProductCartCardResponseDTO>>> GetMyCartAsync(Ulid UserId)
        {
            var cart = await _context.Carts
                .Include(x => x.Items)
                    .ThenInclude(c => c.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == UserId);

            if (cart == null)
                return Result<List<ProductCartCardResponseDTO>>.Error("Корзина не создана", ErrorType.NotFound);

            var rez = cart.Items
                .Select(x => new ProductCartCardResponseDTO(x))
                .ToList();

            return Result<List<ProductCartCardResponseDTO>>.Success(rez);
        }

        public async Task<Result<RemoveCartItemResponseDTO>> RemoveCartItemAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<RemoveCartItemResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            _context.CartItems.Remove(cartItemResult.data);

            await _context.SaveChangesAsync();

            return Result<RemoveCartItemResponseDTO>.Success(new RemoveCartItemResponseDTO(CartItemId));
        }

        public async Task<Result<ProductCartCardResponseDTO>> DecreaseQuantityAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<ProductCartCardResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            if (cartItemResult.data.Quantity > 1)
            {
                cartItemResult.data.Quantity--;
                await _context.SaveChangesAsync();
            }
            return Result<ProductCartCardResponseDTO>.Success(new ProductCartCardResponseDTO(cartItemResult.data));
        }

        public async Task<Result<ProductCartCardResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<ProductCartCardResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            if (cartItemResult.data.Product.Quantity > cartItemResult.data.Quantity)
            {
                cartItemResult.data.Quantity++;
                await _context.SaveChangesAsync();
            }

            return Result<ProductCartCardResponseDTO>.Success(new ProductCartCardResponseDTO(cartItemResult.data));
        }
    }
}
