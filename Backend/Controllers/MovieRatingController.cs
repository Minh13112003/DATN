using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.RatingResult;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/movierating")]
    public class MovieRatingController : ControllerBase
    {
        private readonly IMovieRatingService _service;
        public MovieRatingController(IMovieRatingService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost("AddRating")]
        public async Task<IActionResult> AddRating([FromBody] MovieRatingDTOs movieRatingDTOs)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var UserName = User.Identity?.Name;
                var result = await _service.AddRating(movieRatingDTOs.IdMovie, UserName!,movieRatingDTOs.Point);
                switch (result)
                {
                    case AddRatingResult.Success:
                        return Ok("Đánh giá phim thành công");
                    case AddRatingResult.MissingInfo:
                        return BadRequest(new { message = "Thiếu thông tin" });
                    case AddRatingResult.AlreadyRated:
                        return BadRequest(new { message = "Bạn đã đánh giá phim này" });
                    case AddRatingResult.NotWatched:
                        return BadRequest(new {message = "Bạn chưa xem phim này" });
                    default:
                        return StatusCode(500, "Lỗi không xác định");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpGet("GetRating/{IdMovie}")]
        public async Task<IActionResult> GetMovieRating([FromRoute] string IdMovie)
        {
            try
            {
                var UserName = User.Identity?.Name;
                if (IdMovie.IsNullOrEmpty()) return BadRequest(new { message = "Thiếu thông tin" });
                var ratingpoint = await _service.GetRating(IdMovie, UserName!);
                switch (ratingpoint)
                {
                    case -2:
                        return BadRequest(new { message = "Thiếu trường thông tin" });
                    case -1:
                        return Ok(new { message = "Chưa đánh giá", rating = -1 });
                    default:
                        return Ok(new { message = "Đã đánh giá", rating = ratingpoint });
                }
            
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
