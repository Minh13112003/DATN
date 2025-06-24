using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAll();
        Task<bool> AddCategory(CategoryDTOs categoryDTOs);
        Task<bool> UpdateCategory(CategoryToUpdateDTOs category);
        Task<bool> DeleteCategory(string id);
    }
}
