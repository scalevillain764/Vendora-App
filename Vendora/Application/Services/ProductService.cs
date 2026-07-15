using Application.DTO.ProductDTO.StoreDTO; 
using Application.Result;
using Domain.ErrorTypes;
using Domain.Products;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IProductService = Application.Interfaces.IProductService;
namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }
        private async Task<Result<ProductResponseDTO>> ChangeProductProperty(Ulid UserId, Ulid ProductId, Action<Product> action)
        {
            var storeId = await _context.Stores
                .Where(x => x.SellerId == UserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if(storeId == default)
                return Result<ProductResponseDTO>.Error("Сначала создайте магазин", ErrorType.Forbidden);

            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == ProductId 
                && x.StoreId == storeId);

            if (product == null)
                return Result<ProductResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            action(product);

            await _context.SaveChangesAsync();
            return Result<ProductResponseDTO>.Success(new ProductResponseDTO(product));
        }
        public async Task<Result<ProductResponseDTO>> CreateProductAsync(Ulid UserId, ProductCreationDTO DTO)
        {
            var storeId = await _context.Stores
                .Where(s => s.SellerId == UserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (storeId == default)
                return Result<ProductResponseDTO>.Error("Сначала создайте магазин", ErrorType.Forbidden);

            var newProduct = new Product(storeId, DTO.Category, DTO.Name, DTO.Description, DTO.Price, DTO.Quantity, DTO.PreviewUrl);

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync(); 

            return Result<ProductResponseDTO>.Success(new ProductResponseDTO(newProduct));
        }

        public async Task<Result<ProductResponseDTO>> RemoveProductAsync(Ulid UserId, Ulid ProductId)
        {
            var storeId = await _context.Stores
                .Where(x => x.SellerId == UserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (storeId == default)
                return Result<ProductResponseDTO>.Error("Магазин не найден", ErrorType.NotFound);

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == ProductId 
                    && p.StoreId == storeId);

            if (product == null)
                return Result<ProductResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            var productDTO = new ProductResponseDTO(product);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            return Result<ProductResponseDTO>.Success(productDTO);
        }

        public Task<Result<ProductResponseDTO>> ChangeProductNameAsync(Ulid UserId, Ulid ProductId, ProductChangeNameDTO DTO)
            =>  ChangeProductProperty(UserId, ProductId, x => x.Name = DTO.Name);

        public Task<Result<ProductResponseDTO>> ChangeProductCategoryAsync(Ulid UserId, Ulid ProductId, ProductChangeCategoryDTO DTO)
            => ChangeProductProperty(UserId, ProductId, x => x.Category = (Product.ProductCategory)DTO.Category);

        public Task<Result<ProductResponseDTO>> ChangeProductQuantityAsync(Ulid UserId, Ulid ProductId, ProductChangeQuantityDTO DTO)
            =>  ChangeProductProperty(UserId, ProductId, x => x.Quantity = DTO.Quantity);

        public Task<Result<ProductResponseDTO>> ChangeProductDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeDescriptionDTO DTO) 
            => ChangeProductProperty(UserId, ProductId, x => x.Description = DTO.Description);

        public Task<Result<ProductResponseDTO>> ChangeProductPriceAsync(Ulid UserId, Ulid ProductId, ProductChangePriceDTO DTO)
            => ChangeProductProperty(UserId, ProductId, x => x.Price = DTO.Price);
    }
}