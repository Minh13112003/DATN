using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var categories = await _categoryService.GetAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryToUpdateDTOs category)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var Category = await _categoryService.UpdateCategory(category);
                if (Category) { return Ok(new { message = $"Sửa thông tin phim thành công " }); }
                else return BadRequest(new { message = $"Sửa thông tin phim thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("addCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory(CategoryDTOs categoryDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var category = await _categoryService.AddCategory(categoryDTOs);
                if (category) return Ok(new { message = "Thêm thể loại thành công" });
                else return BadRequest(new { message = "Thêm thể loại thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("deleteCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory([FromRoute]string id)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (!String.IsNullOrEmpty(id)) 
                {
                    var CategoryToDelete = await _categoryService.DeleteCategory(id);
                    if (CategoryToDelete) return Ok(new { message = "Xoá thể loại phim thành công" });

                }
                return NotFound(new { message = "Lỗi khi xóa phim" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
