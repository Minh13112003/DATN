using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DatabaseContext _database;
        private readonly ILogger<ProfileController> _logger;
        private readonly Cloudinary _cloudinary;

        public ProfileController(
            UserManager<AppUser> userManager,
            DatabaseContext database,
            ILogger<ProfileController> logger,
            Cloudinary cloudinary)
        {
            _userManager = userManager;
            _database = database;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] FormFileDTO formFileDTO)
        {
            var file = formFileDTO.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Kiểm tra loại file ảnh
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Only image files (jpg, png, gif) are allowed.");

            try
            {
                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "Avatars", // Optional: Organize files in a folder
                    PublicId = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return BadRequest("Failed to upload image to Cloudinary.");

                var fileUrl = uploadResult.SecureUrl.ToString();

                // Update user image
                var username = User.Identity?.Name;
                var user = await _userManager.FindByNameAsync(username!);
                if (user == null)
                    return NotFound("User not found.");

                // Delete old image from Cloudinary if it exists
                if (!string.IsNullOrEmpty(user.Image) && Uri.TryCreate(user.Image, UriKind.Absolute, out var uriResult))
                {
                    var oldPublicId = ExtractPublicIdFromUrl(user.Image);
                    if (!string.IsNullOrEmpty(oldPublicId))
                    {
                        var deletionParams = new DeletionParams(oldPublicId);
                        await _cloudinary.DestroyAsync(deletionParams);
                        _logger.LogInformation($"Deleted old image from Cloudinary: {oldPublicId}");
                    }
                }

                user.Image = fileUrl;
                await _userManager.UpdateAsync(user);

                return Ok(new { fileName = uploadResult.PublicId, fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image to Cloudinary: {ex.Message}");
                return StatusCode(500, "An error occurred while uploading the image.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-Image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageOrBackgroundDTO UploadImage)
        {
            var file = UploadImage.File;
            var NameMovie = UploadImage.NameMovie;
            if (file == null || file.Length == 0)
                return BadRequest("Không có tệp được tải lên.");

            // Kiểm tra loại file ảnh
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Chỉ chấp nhận các tệp ảnh (jpg, png, gif).");

            try
            {
                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "ImageMovie", // Optional: Organize files in a folder
                    PublicId = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return BadRequest("Failed to upload image to Cloudinary.");

                var fileUrl = uploadResult.SecureUrl.ToString();

                // Update movie image
                var movie = await _database.Movies.FirstOrDefaultAsync(i => i.Title == NameMovie);
                if (movie == null)
                    return NotFound("Không tìm thấy phim.");

                // Delete old image from Cloudinary if it exists
                if (!string.IsNullOrEmpty(movie.Image) && Uri.TryCreate(movie.Image, UriKind.Absolute, out var uriResult))
                {
                    var oldPublicId = ExtractPublicIdFromUrl(movie.Image);
                    if (!string.IsNullOrEmpty(oldPublicId))
                    {
                        var deletionParams = new DeletionParams(oldPublicId);
                        await _cloudinary.DestroyAsync(deletionParams);
                        _logger.LogInformation($"Deleted old image from Cloudinary: {oldPublicId}");
                    }
                }

                movie.Image = fileUrl;
                await _database.SaveChangesAsync();

                return Ok(new { fileName = uploadResult.PublicId, fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image to Cloudinary: {ex.Message}");
                return StatusCode(500, "An error occurred while uploading the image.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("upload-Background")]
        public async Task<IActionResult> UploadBackgroundImage([FromForm] UploadImageOrBackgroundDTO UploadImage)
        {
            var file = UploadImage.File;
            var NameMovie = UploadImage.NameMovie;
            if (file == null || file.Length == 0)
                return BadRequest("Không có tệp được tải lên.");

            // Kiểm tra loại file ảnh
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Chỉ chấp nhận các tệp ảnh (jpg, png, gif).");

            try
            {
                // Upload to Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "BackgroundImageMovie", // Optional: Organize files in a folder
                    PublicId = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return BadRequest("Failed to upload image to Cloudinary.");

                var fileUrl = uploadResult.SecureUrl.ToString();

                // Update movie background image
                var movie = await _database.Movies.FirstOrDefaultAsync(i => i.Title == NameMovie);
                if (movie == null)
                    return NotFound("Không tìm thấy phim.");

                // Delete old background image from Cloudinary if it exists
                if (!string.IsNullOrEmpty(movie.BackgroundImage) && Uri.TryCreate(movie.BackgroundImage, UriKind.Absolute, out var uriResult))
                {
                    var oldPublicId = ExtractPublicIdFromUrl(movie.BackgroundImage);
                    if (!string.IsNullOrEmpty(oldPublicId))
                    {
                        var deletionParams = new DeletionParams(oldPublicId);
                        await _cloudinary.DestroyAsync(deletionParams);
                        _logger.LogInformation($"Deleted old background image from Cloudinary: {oldPublicId}");
                    }
                }

                movie.BackgroundImage = fileUrl;
                await _database.SaveChangesAsync();

                return Ok(new { fileName = uploadResult.PublicId, fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading background image to Cloudinary: {ex.Message}");
                return StatusCode(500, "An error occurred while uploading the background image.");
            }
        }

        // Helper method to extract PublicId from Cloudinary URL
        private string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                var index = Array.IndexOf(segments, "upload");
                if (index != -1 && index + 2 < segments.Length)
                {
                    // PublicId is everything after "upload/v<version>/" and before the file extension
                    return string.Join("/", segments[(index + 2)..]).Split('.')[0];
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}