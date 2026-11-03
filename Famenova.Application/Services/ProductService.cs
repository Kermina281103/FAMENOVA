using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Services.Files;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Product;

namespace Famenova.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var findcat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId);
            if (findcat is null)
                throw new NotFoundException($"There isn't category with Id : {dto.CategoryId}");

            // 🆕
            if (dto.ProductGroupId.HasValue)
                await ValidateGroupAsync(dto.ProductGroupId.Value, dto.VariantLabel);

            var product = _mapper.Map<Product>(dto);
            if (dto.ImageUrl != null)
                product.ImageUrl = await _fileService.SaveImageAsync(dto.ImageUrl, "Products");
            product.ProductGroupId = dto.ProductGroupId;
            product.VariantLabel = dto.VariantLabel;

            await _unitOfWork.GetGeneric<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return await GetByIdAsync(product.Id);
          
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(id);
            if (product is null)
                throw new NotFoundException($"There isn't product with Id : {id}");
            _unitOfWork.GetGeneric<Product>().Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductResponseDto>> GetAllAsync(string? search, int? categoryId, string? sort, int pageIndex, int pageSize)
        {
            var spec = new ProductSpecification(search, categoryId, sort, pageIndex, pageSize);
            var result = await _unitOfWork.GetGeneric<Product>().GetPagedAsync(spec);
            var items = _mapper.Map<IEnumerable<ProductResponseDto>>(result.Items);

            return new PagedResult<ProductResponseDto>(items, result.PageNumber, result.PageSize, result.TotalCount);
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var spec = new ProductSpecification(id);
            var result = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(spec);
            if (result is null)
                throw new NotFoundException($"There isn't product With Id : {id}");

            var response = _mapper.Map<ProductResponseDto>(result);

            // 🆕 الأحجام
            var rootId = result.ProductGroupId ?? result.Id;
            var variants = await _unitOfWork.GetGeneric<Product>()
                .ListAsync(ProductSpecification.Variants(rootId));

            if (variants.Count > 1)
                response.Variants = _mapper.Map<List<ProductVariantDto>>(variants);

            return response;
        }

        public async Task<ProductResponseDto> UpdateAsync(int productId, UpdateProductDto dto)
        {
            var product = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(productId);
            if (product is null)
                throw new NotFoundException($"Product With Id : {productId} is not found ");

            #region Update
            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;

            if (dto.CategoryId.HasValue)
            {
                var cat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId.Value);
                if (cat is null)
                    throw new NotFoundException($"Category With id {dto.CategoryId.Value} is not found ");
                product.CategoryId = dto.CategoryId.Value;
            }

            // 🆕 اسم الحجم
            if (!string.IsNullOrEmpty(dto.VariantLabel))
                product.VariantLabel = dto.VariantLabel;

            // 🆕 المجموعة (0 = شيليه من المجموعة)
            if (dto.ProductGroupId.HasValue)
            {
                if (dto.ProductGroupId.Value == 0)
                {
                    product.ProductGroupId = null;
                }
                else
                {
                    await ValidateGroupAsync(dto.ProductGroupId.Value, product.VariantLabel, productId);
                    product.ProductGroupId = dto.ProductGroupId.Value;
                }
            }

            if (dto.ImageUrl is not null)
            {
                if (product.ImageUrl != null)
                    _fileService.DeleteImage(product.ImageUrl);
                product.ImageUrl = await _fileService.SaveImageAsync(dto.ImageUrl, "Products");
            }
            #endregion

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> IncreaseStockAsync(int productId, int quantity)
        {
            var product = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(productId)
                ?? throw new NotFoundException($"There isn't product with Id : {productId}");

            if (product is Medicine)
                throw new BadRequestException("Medicine stock is managed through its batches");

            product.IncreaseStock(quantity);
            await _unitOfWork.SaveChangesAsync();
            return await GetByIdAsync(productId);
        }

        public async Task<ProductResponseDto> AdjustStockAsync(int productId, int difference)
        {
            var product = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(productId)
                ?? throw new NotFoundException($"There isn't product with Id : {productId}");

            if (product is Medicine)
                throw new BadRequestException("Medicine stock is managed through its batches");

            product.AdjustStock(difference);
            await _unitOfWork.SaveChangesAsync();
            return await GetByIdAsync(productId);
        }

        private async Task ValidateGroupAsync(int groupId, string? variantLabel, int? currentId = null)
        {
            if (string.IsNullOrWhiteSpace(variantLabel))
                throw new BadRequestException("Variant label is required when the product belongs to a group");

            if (currentId == groupId)
                throw new BadRequestException("A product can't be grouped under itself");

            var root = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(groupId)
                ?? throw new NotFoundException($"There isn't product with Id : {groupId}");

            if (root.IsDeleted)
                throw new BadRequestException("The group's main product is deleted");

            if (root is Medicine)
                throw new BadRequestException("A regular product can't be grouped under a medicine");

            if (root.ProductGroupId.HasValue)
                throw new BadRequestException($"Product {groupId} is a variant itself. Use its main product Id : {root.ProductGroupId}");
        }
    }
}