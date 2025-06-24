using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class CategoryServices : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryServices(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> AddCategory(CategoryDTOs categoryDTOs)
        {
            return await _categoryRepository.AddCategory(categoryDTOs);
        }

        public async Task<bool> DeleteCategory(string id)
        {
            return await _categoryRepository.DeleteCategory(id);
        }
        
        public async Task<List<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<bool> UpdateCategory(CategoryToUpdateDTOs category)
        {
            return await _categoryRepository.UpdateCategory(category);
        }

        
    }
}
