using Application.DTO.ProductDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Application.Result;
using Domain.Products;
using Application.DTO.ProductDTO.StatisticsDTO;
namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<List<ProductResponseDTO>>> GetProductsFromStoreAsync(Ulid UserId, Ulid StoreId);
        Task<Result<ProductResponseDTO>> CreateProductAsync(Ulid UserId, ProductCreationDTO DTO);
        Task<Result<ProductResponseDTO>> RemoveProductAsync(Ulid UserId, Ulid ProductId);
        Task<Result<ProductResponseDTO>> ChangeProductNameAsync(Ulid UserId, Ulid ProductId, ProductChangeNameDTO DTO);
        Task<Result<ProductResponseDTO>> ChangeProductCategoryAsync(Ulid UserId, Ulid ProductId, ProductChangeCategoryDTO DTO);
        Task<Result<ProductResponseDTO>> ChangeProductQuantityAsync(Ulid UserId, Ulid ProductId, ProductChangeQuantityDTO DTO);
        Task<Result<ProductResponseDTO>> ChangeProductDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeDescriptionDTO DTO);
        Task<Result<ProductResponseDTO>> ChangeProductPriceAsync(Ulid UserId, Ulid ProductId, ProductChangePriceDTO DTO);
        Task<Result<ProductResponseDTO>> GetProductAsync(Ulid UserId, Ulid ProductId);
        Task<Result<ProductResponseDTO>> ChangeProductPreviewPictureAsync(Ulid UserId, Ulid ProductId, IFormFile? file);
        Task<Result<ProductResponseDTO>> AddPicturesToProductAsync(Ulid UserId, Ulid ProductId, ProductAddPicturesDTO DTO);
        Task<Result<ProductResponseDTO>> RemovePictureFromProduct(Ulid UserId, Ulid ProductId, ProductRemovePictureDTO DTO);
        Task<Result<ProductResponseDTO>> ChangeProductShortDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeShortDescriptionDTO DTO);
    } 
}