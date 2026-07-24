using Application.DTO.CartDTO;
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
        public async Task<Result<CartResponseDTO>> GetMyCartAsync(Ulid UserId)
        {
            var cart = await _context.Carts
                .Include(x => x.Items)
                    .ThenInclude(c => c.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == UserId);

            if (cart == null)
                return Result<CartResponseDTO>.Error("Корзина не создана", ErrorType.NotFound);

            return Result<CartResponseDTO>.Success(new CartResponseDTO(cart));
        }

        public async Task<Result<CartResponseDTO>> RemoveCartItemAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<CartResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            var cartItem = cartItemResult.data;
            var cart = cartItem.Cart;

            if (cartItem == null || cart == null)
                return Result<CartResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return Result<CartResponseDTO>.Success(new CartResponseDTO(cart));
        }

        public async Task<Result<CartResponseDTO>> DecreaseQuantityAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<CartResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            var cartItem = cartItemResult.data;
            var cart = cartItem.Cart;

            if (cartItem == null || cart == null)
                return Result<CartResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                await _context.SaveChangesAsync();
            }

            return Result<CartResponseDTO>.Success(new CartResponseDTO(cart));
        }

        public async Task<Result<CartResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid CartItemId)
        {
            var cartItemResult = await GetCartItemAsync(UserId, CartItemId);

            if (!cartItemResult.IsSuccess)
                return Result<CartResponseDTO>.Error(cartItemResult.ErrorMessage, cartItemResult.ErrorType.Value);

            var cartItem = cartItemResult.data;
            var cart = cartItem.Cart;

            if (cartItem == null || cart == null)
                return Result<CartResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            if (cartItem.Product.Quantity > cartItem.Quantity)
            {
                cartItem.Quantity++;
                await _context.SaveChangesAsync();
            }

            return Result<CartResponseDTO>.Success(new CartResponseDTO(cart));
        }

        public async Task<Result<CartResponseDTO>> AddProductToCartAsync(Ulid UserId, Ulid ProductId)
        {
            var cart = await _context.Carts
                .Include(x => x.Items)
                    .ThenInclude(c => c.Product)
                .FirstOrDefaultAsync(x => x.UserId == UserId);

            if(cart == null)
                return Result<CartResponseDTO>.Error("Что-то пошло не так", ErrorType.Validation);

            var product = await _context.Products
                .FindAsync(ProductId);

            if(product == null)
                return Result<CartResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            if(cart.Items.Any(x => x.ProductId == product.Id))
                return Result<CartResponseDTO>.Error("Товар уже добавлен в корзину", ErrorType.Conflict);

            if (product.Quantity == 0)
                return Result<CartResponseDTO>.Error("Товара нет в наличии", ErrorType.Conflict);

            cart.Items.Add(new CartItem(cart.Id, ProductId, product.Price)
            {
                Product = product 
            });
          
            await _context.SaveChangesAsync();

            return Result<CartResponseDTO>.Success(new CartResponseDTO(cart));
        }
    }
}
