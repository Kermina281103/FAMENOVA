using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Famenova.Shared.Dtos.Product;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var findcat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId);
            if (findcat is null)
                throw new NotFoundException($"There isn't category with Id : {dto.CategoryId}");
            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.GetGeneric<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(product);
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
            var Items = _mapper.Map<IEnumerable<ProductResponseDto>>(result.Items);

            return new PagedResult<ProductResponseDto>(Items, result.PageNumber, result.PageSize, result.TotalCount);
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var spec = new ProductSpecification(id);
            var result = await _unitOfWork.GetGeneric<Product>().GetByIdAsync(spec);
            if (result is null)
                throw new NotFoundException($"There isn't product With Id : {id}");
            return _mapper.Map<ProductResponseDto>(result);
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
            if (!string.IsNullOrEmpty(dto.ImageUrl))
                product.ImageUrl = dto.ImageUrl;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;
            
            if (dto.CategoryId.HasValue)
            {
                var cat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(dto.CategoryId.Value);
                if (cat is null)
                    throw new NotFoundException($"Category With id {dto.CategoryId.Value} is not found ");
                product.CategoryId = dto.CategoryId.Value;

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
    }
}
