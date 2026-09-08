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
        private async Task<Result<FavoriteResponseDTO>> CheckUserAndProductAsync(Ulid UserId, Ulid ProductId, CancellationToken token)
        {
            bool userExists = await _context.Users
               .AnyAsync(x => x.Id == UserId, token);

            if (!userExists)
                return Result<FavoriteResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            bool productExists = await _context.Products
                .AnyAsync(x => x.Id == ProductId, token);

            if (!productExists)
                return Result<FavoriteResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            return Result<FavoriteResponseDTO>.Success(new FavoriteResponseDTO(ProductId, false));
        }
        public async Task<Result<FavoriteResponseDTO>> AddToFavouriteAsync(Ulid UserId, Ulid ProductId, CancellationToken token)
        {
            var checkUserAndProduct = await CheckUserAndProductAsync(UserId, ProductId, token);

            if (!checkUserAndProduct.IsSuccess)
                return Result<FavoriteResponseDTO>.Error(checkUserAndProduct.ErrorMessage!, (ErrorType)checkUserAndProduct.ErrorType!);

            var favourite = new Favourite(UserId, ProductId);

            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync(token);

            return Result<FavoriteResponseDTO>.Success(new FavoriteResponseDTO(ProductId, true));
        }

        public async Task<Result<FavoriteResponseDTO>> RemoveFromFavouriteAsync(Ulid UserId, Ulid ProductId, CancellationToken token)
        {
            var checkUserAndProduct = await CheckUserAndProductAsync(UserId, ProductId, token);

            if (!checkUserAndProduct.IsSuccess)
                return Result<FavoriteResponseDTO>.Error(checkUserAndProduct.ErrorMessage!, (ErrorType)checkUserAndProduct.ErrorType!);

            var favourite = await _context.Favourites
                .FindAsync(UserId, ProductId, token);

            if(favourite == null)
                return Result<FavoriteResponseDTO>.Error("Товар не добавлен в избранное", ErrorType.Conflict);

            _context.Favourites.Remove(favourite);

            await _context.SaveChangesAsync(token);

            return Result<FavoriteResponseDTO>.Success(new FavoriteResponseDTO(ProductId, false));
        }

        public async Task<Result<List<ProductCardDTO>>> GetFavouritesByIdAsync(Ulid UserId, CancellationToken token)
        {
            bool userExists = await _context.Users
                .AnyAsync(x => x.Id == UserId, token);

            if (!userExists)
                return Result<List<ProductCardDTO>>.Error("Пользователь не найден", ErrorType.Forbidden);

            bool hasFilms = await _context.Favourites
                .AnyAsync(x => x.UserId == UserId, token);

            if(!hasFilms)
                return Result<List<ProductCardDTO>>.Error("Нет избранных товаров", ErrorType.Validation);

            var rez = await _context.Favourites
                .Where(x => x.UserId == UserId)
                    .Include(x => x.Product)
                        .Select(x => new ProductCardDTO(x.Product, true))
                .ToListAsync(token);

            return Result<List<ProductCardDTO>>.Success(rez);
        }
    }
}