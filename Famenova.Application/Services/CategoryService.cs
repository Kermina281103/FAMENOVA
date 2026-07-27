using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Dtos.Category;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
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

            CategoryRepo.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var CategoryRepo = _unitOfWork.GetGeneric<Category>();

            var catRes = await CategoryRepo.GetAllAsync();

            return _mapper.Map<IEnumerable<CategoryResponseDto>>(catRes);
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


            _mapper.Map(dto, findCat);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(findCat);
        }
    }
}
