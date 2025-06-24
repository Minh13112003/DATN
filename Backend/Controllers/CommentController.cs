using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Migrations;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : Controller
    {

        private readonly ICommentServices _commentServices;
        public CommentController(ICommentServices commentServices)
        {
            _commentServices = commentServices;
        }
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] CommentDTOs commentDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username)) return BadRequest(new { message = "Không tìm thấy người dùng" });
                var comment = await _commentServices.AddComment(username, commentDTOs);
                return Ok(new { messsage = "Thêm phim thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetCommentBySlugTitle/{slugtitle}")]
        public async Task<IActionResult> GetCommentBySlugTitle(string slugtitle)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (string.IsNullOrEmpty(slugtitle)) return BadRequest(new { messsage = "Trống thông tin" });
                var movie = await _commentServices.GetCommentByMovie(slugtitle);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("UpdateComment")]
        public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTOs commentDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username)) return BadRequest(new { message = "Không tìm thấy người dùng" });
                var comment = await _commentServices.UpdateComment(username,commentDTOs);
                if (comment != true) return BadRequest(new { message = "Sửa bình luận thất bại" });
                return Ok(new { message = "Sửa bình luận thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("DeleteComment/{Idcomment}")]
        public async Task<IActionResult> DeleteComment([FromRoute]string Idcomment)
        {
            try
            {
                var username = User.Identity?.Name;
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(username)) return BadRequest(new { message = "Không tìm thấy người dùng" });
                var CommentDelete = await _commentServices.DeleteComment(role ,username, Idcomment);
                if (CommentDelete != true) return BadRequest(new { messsage = "Xóa bình luận thất bại"});
                return Ok(new { message = "Xóa bình luận thành công" });
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllComment")]
        public async Task<IActionResult> GetAllComment()
        {
            try
            {
                var UserName = User.Identity?.Name;
                var comment = await _commentServices.GetAllComment(UserName!);
                if (comment == null) return BadRequest(new { message = "Đã có lỗi xảy ra" });
                return Ok(comment);
            }catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

    }
}
