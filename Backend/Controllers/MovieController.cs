using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DoAnTotNghiep.Helper.SlugifyHelper;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieServices _services;
        private readonly ITokenServices _tokenServices;
        private readonly UserManager<AppUser> _userManager;
       

        public MovieController(IMovieServices services, ITokenServices tokenServices, UserManager<AppUser> userManager)
        {
            _services = services;
            _tokenServices = tokenServices;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addmovie")]
        public async Task<IActionResult> AddMovie([FromBody]MovieToAddDTOs movieDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var addMovie = await _services.AddMovie(movieDTOs);

                if (addMovie != null) return Ok(new {message = "Thêm thành công"});               
                else return Unauthorized(new {message = "Thêm thất bại"});
                
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{movieid}")]
        public async Task<IActionResult> DeleteMovie(string movieid)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var DeleteMovie = await _services.DeleteMovie(movieid);              

                if (DeleteMovie != null) return Ok(new { message = "Xóa phim thành công" });
                else return Unauthorized(new { message = "Xóa phim thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateMovie([FromBody] MovieToUpdateDTOs movieDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var Movie = await _services.UpdateMovie(movieDTOs);

                if (Movie != false) return Ok(new { message = "Sửa phim thành công" });
                else return Unauthorized(new { message = "Sửa thất bại" });
            }catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }


        
        [HttpGet("getMovieBySlugCategory/{slugCategoryName}")]
        public async Task<IActionResult> GetMovieByCategory([FromRoute]string slugCategoryName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(slugCategoryName))
                {
                    return BadRequest(new { message = "Category name cannot be empty!" });
                }

               string SlugCategoryName = SlugHelper.Slugify(slugCategoryName);
                var movies = await _services.GetMovieByCategory(SlugCategoryName, pageNumber, pageSize);

                if (movies == null)
                {
                    return NotFound(new { message = $"No movies found for category: {slugCategoryName}" });
                }

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        
        [HttpGet("GetMovieBySlugTitle/{slugtitle}")]
        public async Task<IActionResult> GetMovieByTilte(string slugtitle)
        {
            try
            {
                
                if (!ModelState.IsValid) return BadRequest(ModelState);
                string SlugTitle = SlugHelper.Slugify(slugtitle);
                var movie = await _services.GetMovieByTitleSlug(SlugTitle);
                if (movie == null) return NotFound(new { message = "Không tìm thấy phim hoặc sai định dạng Slug" });

                return Ok(movie);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message);
            }

        }
        [HttpGet("GetMovieById/{id}")]
        public async Task<IActionResult> GetMovieById(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id)) 
                {
                    var movie = await _services.GetMovieById(id);
                    if (movie == null) return NotFound(new {message = "Không tìm thấy phim" });
                    return Ok(movie);
                }
                return BadRequest(new { message = "Đã có lỗi xảy ra" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("GetMovieBySlugType/{slugtype}")]
        public async Task<IActionResult> GetMovieBySlugType(string slugtype, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slugtype))
                {
                    return BadRequest(new { message = "Type name cannot be empty!" });
                }
                string SlugType = SlugHelper.Slugify(slugtype);
                var movies = await _services.GetMovieByType(SlugType, pageNumber, pageSize);
                if (movies == null ) return NotFound(new { message = "No movies found!" });

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
        
        [HttpGet("ShowAllMovieCategory")]
        public async Task<IActionResult> ShowAllMovieIncludeCategory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var movie = await _services.GetAllMovie(role!, pageNumber, pageSize);
                if (movie == null) return NotFound(new { message = "Không tìm thấy phim" });
                return Ok(movie);

            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet("SearchMovie")]
        public async Task<IActionResult> SearchMovie(string? keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (!keyword.IsNullOrEmpty())
                {
                    var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    var MovieToSearch = await _services.SearchMovie(keyword, role!, pageNumber, pageSize);
                    if (MovieToSearch != null)
                    {
                        return Ok(MovieToSearch);

                    }
                    else return BadRequest(new { message = "Không tìm được phim" });
                }
                else return BadRequest(new { message = "Keyword không được để trống" });

            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetMovieBySlugNation/{slugNation}")]
        public async Task<IActionResult> GetMovieByNation(string slugNation, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                if (slugNation.IsNullOrEmpty()) return BadRequest(new { message = "Bị trống thông tin" });
                string SlugNation = SlugHelper.Slugify(slugNation);
                var movies = await _services.GetMovieByNation(SlugNation, pageNumber, pageSize);
                if (movies != null) return Ok(movies);
                return BadRequest(new { message = "Không tìm thấy phim" });
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [HttpGet("GetMovieByStatus/{status}")]
        public async Task<IActionResult> GetMovieByStatus(string status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (status.IsNullOrEmpty()) return BadRequest(new { message = "Bị trống thông tin" });
                var movies = await _services.GetMovieByStatus(status, pageNumber, pageSize);
                if (movies != null) return Ok(movies);
                return BadRequest(new { message = "Không tìm thấy phim" });
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [HttpGet("GetMovieByActor/{actor}")]
        public async Task<IActionResult> GetMovieByActor(string actor, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (!ModelState.IsValid || actor.IsNullOrEmpty()) return BadRequest(new { message = "Đã có lỗi xảy ra" });
                string actorSlug = SlugHelper.Slugify(actor);
                var movie = await _services.GetMovieByActor(actorSlug, pageNumber, pageSize);
                if (movie != null) return Ok(movie);
                return BadRequest(new { message = "Không tìm thấy phim" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpPost("ToggleFavoriteMovie/{slugtitle}")]
        public async Task<IActionResult> ToggleFavoriteMovie(string slugtitle)
        {
            try
            {
                var userName = User.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { message = "Không xác định được người dùng." });

                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy người dùng." });

                if (string.IsNullOrEmpty(slugtitle))
                    return BadRequest(new { message = "Slug title không hợp lệ." });

                var newSlugTitle = SlugHelper.Slugify(slugtitle); // Chuẩn hóa slugtitle

                var currentFavorite = user.FavoriteSlugTitle ?? string.Empty;
                var list = currentFavorite
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                bool isFavorite;
                string message;

                if (list.Contains(newSlugTitle))
                {
                    list.Remove(newSlugTitle);
                    isFavorite = false;
                    message = "Đã xóa phim khỏi danh sách yêu thích.";
                }
                else
                {
                    list.Add(newSlugTitle);
                    isFavorite = true;
                    message = "Đã thêm phim vào danh sách yêu thích.";
                }

                user.FavoriteSlugTitle = string.Join(",", list);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { message, isFavorite });
                }
                else
                {
                    return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật dữ liệu.", errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("FavoriteMovies")]
        public async Task<IActionResult> GetFavoriteMovies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "Không xác định được người dùng." });
            if (pageNumber < 1) return BadRequest(new { message = "Số trang không hợp lệ" });
            if (pageSize <= 0) return BadRequest(new { message = "PageSize không hợp lệ" });
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });
            if (string.IsNullOrEmpty(user.FavoriteSlugTitle)) return Ok(new { message = "Danh sách yêu thích trống." });
            var slugTitles = user.FavoriteSlugTitle
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.Equals(s, "s.Value", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var movies = await _services.GetFavoriteMoviesBySlugTitlesAsync(slugTitles, pageNumber, pageSize);

            return Ok(movies);
        }
        [HttpPost("IncreaseView/{titleSlug}")]
        public async Task<IActionResult> IncreaseView(string titleSlug)
        {
            if(titleSlug.IsNullOrEmpty()) return BadRequest(new {messsae = ""});
            var result = await _services.IncreaseMovieView(titleSlug);
            if (!result)
                return NotFound();

            return Ok(new { message = "Tăng View thành công." });
        }
        [HttpGet("GetNewestMovie")]
        public async Task<IActionResult> GetNewestMovie([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var movie = await _services.GetNewestMovie(pageNumber,pageSize);
                return Ok(movie);
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpPost("AddHistory")]
        public async Task<IActionResult> AddMovieHistory([FromBody] HistoryDTOs historyDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var userName = User.Identity?.Name;
                var result = await _services.AddHistoryMovie(historyDTOs.IdMovie, userName!);
                if (result == true) return Ok(new { message = "Đã lưu phim vào lịch sử xem phim hoặc phim đã lưu vào lịch sử xem phim" });
                return Unauthorized(new { message = "Trống thông tin " });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("GetRelatedMovie/{slugtitle}")]
        public async Task<IActionResult> GetRelatedMovie(string slugtitle, [FromQuery] int limit = 6)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest(ModelState);
                var slug = SlugHelper.Slugify(slugtitle);
                var movie = await _services.GetRelatedMoviesByCategories(slug, limit);
                if (movie == null) return Ok(new { message = "Chưa có phim liên quan" });
                return Ok(movie);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpGet("GetHistoryMovie")]
        public async Task<IActionResult> GetHistoryMovie([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var UserName = User.Identity?.Name;
                var Movie = await _services.GetHistoryMovie(UserName!, pageNumber, pageSize);
                return Ok(Movie);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("AdvanceSearch")]
        public async Task<IActionResult> SearchMovies(
        [FromQuery] string? genres,
        [FromQuery] string? countries,
        [FromQuery] string? type,
        [FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 36)
        {
            var filter = new MovieFilterDto
            {
                Genres = string.IsNullOrWhiteSpace(genres) ? null : genres.Split(',').ToList(),
                Countries = string.IsNullOrWhiteSpace(countries) ? null : countries.Split(',').ToList(),
                Type = string.IsNullOrWhiteSpace(type) ? null : type.Split(',').ToList(),
                Status = string.IsNullOrWhiteSpace(status) ? null : status.Split(',').ToList()
            };

            var movies = await _services.GetFilteredMovies(filter, pageNumber, pageSize);
            if(movies != null) return Ok(movies);
            return BadRequest(new {message = "Không tìm thấy phim mà bạn cần"});
        }
    }
}
