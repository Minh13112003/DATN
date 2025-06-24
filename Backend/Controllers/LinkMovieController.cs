using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/linkmovie")]
    [ApiController]
    public class LinkMovieController : ControllerBase
    {
        private readonly ILinkMovieServices _services;
        public LinkMovieController(ILinkMovieServices services)
        {
            _services = services;
        }
        [HttpGet("GetAllLinkMovie")]
        public async Task<IActionResult> GetAllLinkMovie()
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var linkmovie = await _services.ShowAllLinkMovie();
                return Ok(linkmovie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("AddLinkMovie")]
        public async Task<IActionResult> AddLinkMovie(LinkMovieToAddDTOs linkmovie)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (linkmovie == null) return BadRequest(new { message = "Bạn không được nhập trống thông tin" });
                var Linkmovie = await _services.AddLinkMovie(linkmovie);
                if (Linkmovie != false)
                {
                    return Ok(new {message = "Thêm Link phim thành công"});
                }
                return BadRequest(new { message = "Thêm link phim thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("DeleteLinkMovie/{id}")]
        public async Task<IActionResult> DeleteLinkMovie(string id)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var LinkMovie = await _services.DeleteLinkMovie(id);
                if (LinkMovie != false)
                {
                    return Ok(new {message = "Xóa thành công"});
                }
                return BadRequest(new { message = "Xóa thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("UpdateLinkMovie")]
        public async Task<IActionResult> UpdateLinkMovie(LinkMovieDTOs linkMovie)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (linkMovie == null) return BadRequest(new { message = "Bạn không được để trống thông tin" });
                var LinkMovieUpdate = await _services.UpdateLinkMovie(linkMovie);
                if (LinkMovieUpdate != false) return Ok(new { message = "Sửa link phim thành công" });
                return BadRequest(new { message = "Sửa link phim thất bại" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetLinkMovieByIdMovie/{idmovie}")]
        public async Task<IActionResult> GetLinkMovieByIdMovie([FromRoute]string idmovie)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var Linkmovie = await _services.GetMovieByIdMovie(idmovie);
                if (Linkmovie != null) return Ok(Linkmovie);
                return BadRequest(new { message = "Phim chưa có tập" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
