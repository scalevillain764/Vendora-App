using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.SearchDTO;
using Application.Result;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using ISearchService = Application.Interfaces.ISearchService;
using Application.PagedResponse;
namespace Application.Services
{
    public class SearchService: ISearchService
    {
        private readonly AppDbContext _context;
        public SearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PagedResponse<ProductCardDTO>>> SearchAsync(Ulid UserId, SearchRequestDTO DTO, CancellationToken token)
        {
            var products = _context.Products
                .Include(x => x.ProductReviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(DTO.Query))
            {
                string[] words = DTO.Query
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach(var word in words)
                {
                    products = products
                        .Where(x => x.Name
                        .ToLower()
                        .Contains(word));
                }
            }

            if (DTO.CategoryIds != null)
            {
                foreach(var category in DTO.CategoryIds)
                {
                    products = products
                        .Where(x => DTO.CategoryIds.Contains((int)x.Category)); // fix later: add list categories
                }
            }

            if (DTO.MinPrice != null)
                products = products
                    .Where(x => x.Price >= DTO.MinPrice);

            if(DTO.MaxPrice != null)
                products = products
                    .Where(x => x.Price <= DTO.MaxPrice);

            if (DTO.OnlyInStock != null)
                products = products
                    .Where(x => x.Quantity > 0);

            int totalCount = await products.CountAsync();
            var userLikes = _context.Favourites.Where(x => x.UserId == UserId);
             
            var result = await products
                .GroupJoin (
                    userLikes,
                    product => product.Id,
                    fav => fav.ProductId,
                    (product, favs) => new
                    {
                        Product = product,
                        IsFav = favs.Any()
                    }
                 )
                .Skip((DTO.Page - 1) * DTO.PageSize)
                .Take(DTO.PageSize)
                .Select(x => new ProductCardDTO(x.Product, x.IsFav))
                .ToListAsync(token);

            return Result<PagedResponse<ProductCardDTO>>.Success(new PagedResponse<ProductCardDTO>(result, DTO.Page, DTO.PageSize, totalCount));
        }
    }
}