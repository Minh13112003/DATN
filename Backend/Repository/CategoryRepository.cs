using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.SlugifyHelper;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseContext _databaseContext;
        public CategoryRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AddCategory(CategoryDTOs categoryDTOs)
        {
            try
            {
                var category = new Category
                {
                    IdCategories = Guid.NewGuid().ToString(),
                    NameCategories = categoryDTOs.NameCategories,
                    SlugNameCategories = SlugHelper.Slugify(categoryDTOs.NameCategories),
                };
                await _databaseContext.Categories.AddAsync(category);
                await _databaseContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCategory(string id)
        {
            var category = await _databaseContext.Categories.FirstOrDefaultAsync(i => i.IdCategories == id);
            if (category == null)
            {
                return false;
            }
            _databaseContext.Categories.Remove(category);
            await _databaseContext.SaveChangesAsync();
            return true;

        
        }

        public async Task<List<Category>> GetAll()
        {
            var Category = await _databaseContext.Categories.ToListAsync();
            /*var movieTypes = await _databaseContext.Movies
                        .Select(m => new
                        {
                            nameMovieType = m.TypeMovie,
                            slugNameMovieType = m.SlugTypeMovie
                        })
                        .Distinct()
                        .ToListAsync();
            return new { Category, movieTypes };*/
            return Category;
        }

            public async Task<bool> UpdateCategory(CategoryToUpdateDTOs category)
        {
            var Category = await _databaseContext.Categories.FirstOrDefaultAsync(i => i.IdCategories == category.IdCategories);
            if (Category == null)
            {
                return false;
            }
            Category.SlugNameCategories = SlugHelper.Slugify(category.NameCategories);
            Category.NameCategories = category.NameCategories;
            await _databaseContext.SaveChangesAsync();
            return true;

        }
    }
}
