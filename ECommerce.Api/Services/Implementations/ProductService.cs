using AutoMapper;
using ECommerce.Api.Dtos.Product;
using ECommerce.Api.Entities;
using ECommerce.Api.Helpers;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public ProductService(IProductRepository repo, IMapper mapper, IPhotoService photoService)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            if (dto.ImageFile != null)
            {
                var result = await _photoService.AddPhotoAsync(dto.ImageFile);

                if (result.Error != null)
                    throw new Exception(result.Error.Message);

                var image = new ProductImage
                {
                    Url = result.SecureUrl.AbsoluteUri,
                    PublicId = result.PublicId,
                    IsMain = true
                };

                product.Images.Add(image);
            }

            await _repo.AddAsync(product);
            var savedProduct = await _repo.GetByIdAsync(product.Id);
            return _mapper.Map<ProductDto>(savedProduct);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<PagedList<ProductDto>> GetAllProductsAsync(ProductParams productParams)
        {
            var pagedProducts = await _repo.GetAllAsync(productParams);

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(pagedProducts);

            return new PagedList<ProductDto>(
                productDtos.ToList(),
                pagedProducts.TotalCount,
                pagedProducts.CurrentPage,
                pagedProducts.PageSize
            );
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(Guid id, CreateProductDto dto)
        {
            var existingProduct = await _repo.GetByIdAsync(id);
            if (existingProduct == null) return;

            _mapper.Map(dto, existingProduct);

            if (dto.ImageFile != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.ImageFile);

                if (uploadResult.Error == null)
                {
                    foreach (var img in existingProduct.Images)
                        img.IsMain = false;

                    var newImage = new ProductImage
                    {
                        Url = uploadResult.SecureUrl.AbsoluteUri,
                        PublicId = uploadResult.PublicId,
                        IsMain = true,
                        ProductId = existingProduct.Id
                    };

                    existingProduct.Images.Add(newImage);
                }
            }
            await _repo.UpdateAsync(existingProduct);
        }
    }
}
