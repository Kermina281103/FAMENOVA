using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Models;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Specifications;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class CategoryService(IUnitOfWork _unitOfWork, IMapper _mapper) : ICategoryService
    {
      
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var CategoryRepo = _unitOfWork.GetGeneric<Category>();
            if(await CategoryRepo.AnyAsync(s => s.Name.ToLower() == dto.Name.ToLower()))
            {
                throw new ConflictException("Category With this Name Is Already Exist");
            }
            var category = _mapper.Map<Category>(dto);
            var count = await CategoryRepo.CountAsync();

            if(dto.DisplayOrder.HasValue &&
                (dto.DisplayOrder.Value<1|| dto.DisplayOrder.Value > count + 1))
            {
                throw new ValidationException([ "Invalid DisplayOrder" ]);
            }
            if (dto.DisplayOrder.HasValue)
            {
                var getCat = await CategoryRepo.GetAllAsync(c => c.DisplayOrder >= dto.DisplayOrder!.Value);

                foreach(var item in getCat)
                {
                    item.DisplayOrder++;
                }
            }

            if (!dto.DisplayOrder.HasValue)
            {
                category.DisplayOrder = count + 1;
            }
            else
            {
                category.DisplayOrder = dto.DisplayOrder.Value;
            }

                await CategoryRepo.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
            
        }
       
        public async Task DeleteAsync(int id)
        {
            var CategoryRepo = _unitOfWork.GetGeneric<Category>();
            var category = await CategoryRepo.GetByIdAsync(id);
            if (category is null)
                throw new NotFoundException("Category Is Not Found ");
            var getCat = await CategoryRepo.GetAllAsync(c => c.DisplayOrder > category.DisplayOrder);
            foreach(var item in getCat)
            {
                item.DisplayOrder--;
            }

            CategoryRepo.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<CategoryResponseDto>> GetAllAsync(string? search, string? sort, int pageIndex, int pageSize)
        {
            var CategoryRepo = _unitOfWork.GetGeneric<Category>();
            var spec = new CategorySpecification(search, sort, pageIndex, pageSize);

            var pagedCategories = await CategoryRepo.GetPagedAsync(spec);
            var items = _mapper.Map<IEnumerable<CategoryResponseDto>>(pagedCategories.Items);

            return new PagedResult<CategoryResponseDto>
                (items , 
                pagedCategories.PageNumber,
                pagedCategories.PageSize , 
                pagedCategories.TotalCount
                );

            
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int categoryId)
        {
            var findCat = await _unitOfWork.GetGeneric<Category>().GetByIdAsync(categoryId);

            if (findCat is null)
                throw  new NotFoundException($"Category With id {categoryId} is not found ");

            return _mapper.Map<CategoryResponseDto>(findCat);

        }

        public async Task<CategoryResponseDto> UpdateAsync(int categoryId, UpdateCategoryDto dto)
        {
            var CategoryRepo = _unitOfWork.GetGeneric<Category>();
            var findCat = await CategoryRepo.GetByIdAsync(categoryId);
            if (findCat is null)
                throw new NotFoundException($"Category With Id {categoryId} is not found ");
           
            var exists = await CategoryRepo.AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower() && categoryId != c.Id);
            if (exists)
                throw new ConflictException("There is Conflict on Naming");

            var count = await CategoryRepo.CountAsync();

            if (dto.DisplayOrder!=0)
            {
                var newOrder = dto.DisplayOrder;
                var oldOrder = findCat.DisplayOrder;

                if (newOrder < 1 || newOrder > count)
                {
                    throw new ValidationException(["Invalid DisplayOrder"]);
                   
                }
                if (newOrder != oldOrder)
                {
                    if (newOrder > oldOrder)
                    {
                        var categoriesToShift = await CategoryRepo.GetAllAsync(c => c.DisplayOrder > oldOrder && c.DisplayOrder <= newOrder);

                        foreach(var item in categoriesToShift)
                        {
                            item.DisplayOrder--;
                        }
                    }
                    else
                    {
                        var categoriesToShift = await CategoryRepo.GetAllAsync(
                            c => c.DisplayOrder >= newOrder &&
                            c.DisplayOrder < oldOrder);
                        foreach(var item in categoriesToShift)
                        {
                            item.DisplayOrder++;
                        }
                    }
                    findCat.DisplayOrder = newOrder;
                }
            }
            if (!string.IsNullOrEmpty(dto.Name ))
                findCat.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
                findCat.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.ImageUrl))
                findCat.ImageUrl = dto.ImageUrl;
           


            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(findCat);
        }
    }
}
