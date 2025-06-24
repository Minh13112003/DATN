using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();
        Task<bool> AddCategory(CategoryDTOs categoryDTOs);
        Task<bool> UpdateCategory(CategoryToUpdateDTOs category);
        Task<bool> DeleteCategory(string id);
    }
}
