using Application.Result;
using Application.DTO.FavouriteDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Domain.ErrorTypes;
using Infrastructure.AppDbContexts;
using IFavouriteService = Application.Interfaces.IFavouriteService;
using Microsoft.EntityFrameworkCore;
using Domain.Favourites;
namespace Application.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly AppDbContext _context;
        public FavouriteService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<FavoriteResponseDTO>> AddToFavouriteAsync(Ulid UserId, Ulid ProductId)
        {
            bool userExists = await _context.Users
                .AnyAsync(x => x.Id == UserId);

            if (!userExists)
                return Result<FavoriteResponseDTO>.Error("Пользователь не найден", ErrorType.Forbidden);

            bool productExists = await _context.Products
                .AnyAsync(x => x.Id == ProductId);

            if (!productExists)
                return Result<FavoriteResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            var favourite = new Favourite(UserId, ProductId);

            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync();

            return Result<FavoriteResponseDTO>.Success(new FavoriteResponseDTO(ProductId, true));
        }

        public async Task<Result<FavoriteResponseDTO>> RemoveFromFavouriteAsync(Ulid UserId, Ulid ProductId)
        {
            bool userExists = await _context.Users
                .AnyAsync(x => x.Id == UserId);

            if (!userExists)
                return Result<FavoriteResponseDTO>.Error("Пользователь не найден", ErrorType.Forbidden);

            bool productExists = await _context.Products
                .AnyAsync(x => x.Id == ProductId);

            if (!productExists)
                return Result<FavoriteResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            var favourite = await _context.Favourites
                .FindAsync(UserId, ProductId);

            if(favourite == null)
                return Result<FavoriteResponseDTO>.Error("Товар не добавлен в избранное", ErrorType.Conflict);

            _context.Favourites.Remove(favourite);

            await _context.SaveChangesAsync();

            return Result<FavoriteResponseDTO>.Success(new FavoriteResponseDTO(ProductId, false));
        }

        public async Task<Result<List<ProductCardDTO>>> GetFavouritesByIdAsync(Ulid UserId)
        {
            bool userExists = await _context.Users
                .AnyAsync(x => x.Id == UserId);

            if (!userExists)
                return Result<List<ProductCardDTO>>.Error("Пользователь не найден", ErrorType.Forbidden);

            bool hasFilms = await _context.Favourites
                .AnyAsync(x => x.UserId == UserId);

            if(!hasFilms)
                return Result<List<ProductCardDTO>>.Error("Нет избранных товаров", ErrorType.Validation);

            var rez = await _context.Favourites
                .Where(x => x.UserId == x.UserId)
                    .Include(x => x.Product)
                        .Select(x => new ProductCardDTO(x.Product, true))
                .ToListAsync();

            return Result<List<ProductCardDTO>>.Success(rez);
        }
    }
}