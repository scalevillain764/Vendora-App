using Application.DTO.ProductDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Application.Interfaces;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Products;
using Domain.ProductStatisticsForStores;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IProductService = Application.Interfaces.IProductService;
namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IS3Service _S3Service;
        public ProductService(AppDbContext context, IS3Service S3Service)
        {
            _context = context;
            _S3Service = S3Service;
        }
        private async Task<Result<ProductResponseDTO>> ChangeProductProperty(Ulid UserId, Ulid ProductId, Action<Product> action)
        {
            var product = await _context.Products
                .Include(x => x.ProductReviews)
                .FirstOrDefaultAsync(x => x.Id == ProductId && x.Store.SellerId == UserId);

            if (product == null)
                return Result<ProductResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            action(product);

            await _context.SaveChangesAsync();
            return Result<ProductResponseDTO>.Success(new ProductResponseDTO(product, true));
        }
        public async Task<Result<ProductResponseDTO>> CreateProductAsync(Ulid UserId, ProductCreationDTO DTO)
        {
            var storeId = await _context.Stores
                .Where(s => s.SellerId == UserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (storeId == default)
                return Result<ProductResponseDTO>.Error("Сначала создайте магазин", ErrorType.Forbidden); 

            var newProduct = new Product
                (storeId, DTO.Category, DTO.Name, DTO.Description, DTO.ShortDescription, DTO.Price, 
                DTO.Quantity, DTO.PreviewUrl, DTO.Pictures);

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync(); 

            return Result<ProductResponseDTO>.Success(new ProductResponseDTO(newProduct, true));
        }
        public Task<Result<ProductResponseDTO>> RemoveProductAsync(Ulid UserId, Ulid ProductId)
            => ChangeProductProperty(UserId, ProductId, x => x.IsDeleted = true);
        
        // pics
        public Task<Result<ProductResponseDTO>> AddPicturesToProductAsync(Ulid UserId, Ulid ProductId, ProductAddPicturesDTO DTO)
            => ChangeProductProperty(UserId, ProductId, x => x.Pictures.AddRange(DTO.pictures)); // load new pics for product, after creating e.g.

        public async Task<Result<ProductResponseDTO>> RemovePictureFromProduct(Ulid UserId, Ulid ProductId, ProductRemovePictureDTO DTO) // rempve picture from product, after creaing e.g. 
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == ProductId && x.Store.SellerId == UserId);

            if (product == null)
                return Result<ProductResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            if (product.Pictures == null)
                return Result<ProductResponseDTO>.Error("Фото отсутсвуют", ErrorType.Conflict);

            var productFileUrl = product.Pictures.FirstOrDefault(x => x == DTO.fileURL);

            if(productFileUrl == default)
                return Result<ProductResponseDTO>.Error("Фотография не найдена", ErrorType.NotFound);

            var removement_rezult = await _S3Service.RemovePhotoByUrlAsync(productFileUrl);
            if (!removement_rezult.IsSuccess)
                return Result<ProductResponseDTO>.Error(removement_rezult.ErrorMessage,
                    removement_rezult.ErrorType ?? ErrorType.Conflict);

            product.Pictures.Remove(DTO.fileURL);
            
            await _context.SaveChangesAsync();
            return Result<ProductResponseDTO>.Success(new ProductResponseDTO(product, true));
        }

        public async Task<Result<List<ProductResponseDTO>>> GetProductsFromStoreAsync(Ulid UserId, Ulid StoreId)
        {
            var rez = await _context.Products
                    .Where(x => x.StoreId == StoreId)
                    .Select(x => new ProductResponseDTO(x, x.Store.SellerId == UserId))
                    .ToListAsync();

            return Result<List<ProductResponseDTO>>.Success(rez);
        }
                

        public async Task<Result<ProductResponseDTO>> ChangeProductPreviewPictureAsync(Ulid UserId, Ulid ProductId, IFormFile file) // change preview after creating e.g
        {
            var loadPicture = await _S3Service.UploadPhotoAsync(file);

            if (!loadPicture.IsSuccess)
                return Result<ProductResponseDTO>.Error(loadPicture.ErrorMessage, loadPicture.ErrorType ?? ErrorType.Conflict);

            string NewUrl = loadPicture.data;
            string? PrevUrl = null;

            var updateProductPreviewUrl = await ChangeProductProperty(UserId, ProductId, x =>
            {
                PrevUrl = x.PreviewUrl;
                x.PreviewUrl = NewUrl;              
            });

            if (!updateProductPreviewUrl.IsSuccess)
            {
                await _S3Service.RemovePhotoByUrlAsync(NewUrl);
                return Result<ProductResponseDTO>.Error(updateProductPreviewUrl.ErrorMessage, updateProductPreviewUrl.ErrorType ?? ErrorType.Conflict);
            }

            if (!string.IsNullOrEmpty(PrevUrl))
                await _S3Service.RemovePhotoByUrlAsync(PrevUrl);

            return Result<ProductResponseDTO>.Success(updateProductPreviewUrl.data);
        }  

        public async Task<Result<ProductResponseDTO>> RemoveProductPreviewPictureAsync(Ulid UserId, Ulid ProductId, ProductChangeAndRemovePreviewPictureDTO DTO) // remove preview after creating e.g
        {
            var updateProductPreviewUrl = await ChangeProductProperty(UserId, ProductId, x => x.PreviewUrl = null);

            if (!updateProductPreviewUrl.IsSuccess)
                return Result<ProductResponseDTO>.Error(updateProductPreviewUrl.ErrorMessage, updateProductPreviewUrl.ErrorType ?? ErrorType.Conflict);

            var removePictureFromS3 = await _S3Service.RemovePhotoByUrlAsync(DTO.previewURL);

            if (!removePictureFromS3.IsSuccess)
            {
                await ChangeProductProperty(UserId, ProductId, x => x.PreviewUrl = DTO.previewURL);
                return Result<ProductResponseDTO>.Error(removePictureFromS3.ErrorMessage, removePictureFromS3.ErrorType ?? ErrorType.Conflict);
            }

            return Result<ProductResponseDTO>.Success(updateProductPreviewUrl.data);
        }
        // pics

        public async Task<Result<ProductResponseDTO>> GetProductAsync(Ulid UserId, Ulid ProductId)
        {
            var product = await _context.Products
                .Include(x => x.Store)
                .FirstOrDefaultAsync(x => x.Id == ProductId);

            if (product == null)
                return Result<ProductResponseDTO>.Error("Продукт не найден", ErrorType.NotFound);

            return Result<ProductResponseDTO>.Success(
                new ProductResponseDTO(product, product.Store.SellerId == UserId));
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

        public Task<Result<ProductResponseDTO>> ChangeProductShortDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeShortDescriptionDTO DTO)
           => ChangeProductProperty(UserId, ProductId, x => x.ShortDescription = DTO.shortDescription);
    }
}