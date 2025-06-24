using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/mixapi")]
    [ApiController]
    public class MixController : ControllerBase
    {
        private readonly IMixAPIService _mixAPIService;
        public MixController(IMixAPIService mixAPIService)
        {
            _mixAPIService = mixAPIService;
        }

        [HttpGet("getCategoryAndMovieType")]
        public async Task<IActionResult> GetCategoryAndMovieType()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _mixAPIService.GetMovieTypeAndCategory();
                if (result == null) return BadRequest(new { message = "Không có dữ liệu" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("getNumberOfMovieAndCategory")]
        public async Task<IActionResult> GetNumberOfMovieAndCategory()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _mixAPIService.GetcountMovieAndCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("MoviePointView")]
        public async Task<IActionResult> GetMoviePointView([FromQuery] string sortBy = "point")
        {
            var movie = await _mixAPIService.GetMovieAndPoint(sortBy);
            if (movie.Count > 0)
            {
                return Ok(movie);
            }
            return BadRequest(new { message = "Chưa có phim" });
        }
    }
}
