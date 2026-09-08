using Application.DTO.ProductDTO;
using Application.DTO.ProductDTO.StatisticsDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Application.PagedResponse;
using Application.Result;
using Domain.Products;
namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<PagedResponse<ProductResponseDTO>>> GetProductsFromStoreAsync(Ulid UserId, ProductsGetFromStoreDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> CreateProductAsync(Ulid UserId, ProductCreationDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> RemoveProductAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductNameAsync(Ulid UserId, Ulid ProductId, ProductChangeNameDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductCategoryAsync(Ulid UserId, Ulid ProductId, ProductChangeCategoryDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductQuantityAsync(Ulid UserId, Ulid ProductId, ProductChangeQuantityDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeDescriptionDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductPriceAsync(Ulid UserId, Ulid ProductId, ProductChangePriceDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> GetProductAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductPreviewPictureAsync(Ulid UserId, Ulid ProductId, IFormFile? file, CancellationToken token);
        Task<Result<ProductResponseDTO>> AddPicturesToProductAsync(Ulid UserId, Ulid ProductId, ProductAddPicturesDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> RemovePictureFromProduct(Ulid UserId, Ulid ProductId, ProductRemovePictureDTO DTO, CancellationToken token);
        Task<Result<ProductResponseDTO>> ChangeProductShortDescriptionAsync(Ulid UserId, Ulid ProductId, ProductChangeShortDescriptionDTO DTO, CancellationToken token);
    } 
}