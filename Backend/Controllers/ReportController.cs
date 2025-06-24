using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportServices _reportServices;
        public ReportController(IReportServices reportServices)
        {
            _reportServices = reportServices;
        }
        //Người dùng theo dõi Report mà bản thân đã gửi
        [Authorize]
        [HttpGet("GetSelfReport")]
        public async Task<IActionResult> GetSelfReport()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username)) return BadRequest(new { message = "Bạn chưa đăng nhập" });
                var result = await _reportServices.GetSelfReport(username);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // Người dùng gửi Report tới Admin
        [Authorize]
        [HttpPost("UpReport")]
        public async Task<IActionResult> UpReport(UpReportDTOs upReportDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var update = await _reportServices.UpReport(username, upReportDTOs);
                    if (update == false) return BadRequest(new { message = "Thêm thất bại" });
                    return Ok(new { message = "Thêm báo cáo thành công" });
                }
                return BadRequest(new { message = "Thêm thất bại" });
            }
            catch (InvalidOperationException ex2)
            {
                return BadRequest(ex2.Message); 
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            
        }

        //Hiển thị danh sách report cho phía admin
        [Authorize(Roles = "Admin")]
        [HttpGet("GetReportSystem")]
        public async Task<IActionResult> GetReportAdmin()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var result = await _reportServices.GetReportAdmin(username);
                    if(result != null) return Ok(result);
                    
                }
                return BadRequest(new { message = "Lỗi" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetReportComment")]
        public async Task<IActionResult> GetReportComment()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var result = await _reportServices.GetReportComment(username);
                    if (result != null) return Ok(result);

                }
                return BadRequest(new { message = "Lỗi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetReportMovie")]
        public async Task<IActionResult> GetReportMovie()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var result = await _reportServices.GetReportMovie(username);
                    if (result != null) return Ok(result);

                }
                return BadRequest(new { message = "Lỗi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var result = await _reportServices.GetNotificationAdmin(username);
                    if (result != null) return Ok(result);

                }
                return BadRequest(new { message = "Lỗi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //Xử lý report khi admin nhận
        [Authorize(Roles = "Admin")]
        [HttpPut("ReceiveReport/{IdReport}")]
        public async Task<IActionResult> ReceiveReportAdmin([FromRoute]string IdReport)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username)) 
                {
                    var result = await _reportServices.ReceiveReport(IdReport, username);
                    if (result == true) return Ok(new { message = "Đã nhận report" });

                }
                return BadRequest(new {message = "Đã có lỗi" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //Gửi phản hồi về cho người dùng
        [Authorize(Roles = "Admin")]
        [HttpPut("ResponseReport")]
        public async Task<IActionResult> ResponseReport([FromBody] ResponseReport responseReport)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    var result = await _reportServices.ResponseReport(username, responseReport);
                    if (result == true) return Ok(new { message = "Xử lý phản hồi thành công" });

                }
                return BadRequest(new { message = "Có lỗi xảy ra" });
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteReport/{IdReport}")]
        public async Task<IActionResult> DeleteReport([FromRoute] string IdReport)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = await _reportServices.DeleteReport(IdReport);
                if (result == true) return Ok(new { message = "Xóa báo cáo thành công" });
                return BadRequest(new { message = "Xóa báo cáo thất bại" });
                     
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetCommentReport/{IdComment}")]
        public async Task<IActionResult> GetCommentReport([FromRoute] string IdComment)
        {
            if (string.IsNullOrEmpty(IdComment)) return BadRequest(new { message = "Thiếu Id bình luận" });
            var username = User.Identity?.Name;
            var comment = await _reportServices.GetCommentReport(IdComment, username!);
            if (comment == null) return BadRequest(new { message = "Không tìm thấy báo cáo của bình luận" });
            return Ok(comment);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ExecuteReport/{IdComment}")]
        public async Task<IActionResult> ExecuteReport([FromRoute] string IdComment)
        {
            if (string.IsNullOrEmpty(IdComment)) return BadRequest(new { message = "Thiếu Id bình luận" });
            var username = User.Identity?.Name;
            var result = await _reportServices.ExecuteCommentReport(IdComment, username!);
            if (result == true) return Ok();
            return BadRequest(new {message = "Đã có lỗi xảy ra"});
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("ResponseCommentReport")]
        public async Task<IActionResult> ResponseCommentReport([FromBody] List<ResponseReport> responseReports)
        {
            var username = User.Identity?.Name;
            var result =await _reportServices.ResponseCommentReports(responseReports, username!);
            if (!result)
                return BadRequest("Xử lý báo cáo thất bại.");

            return Ok("Phản hồi báo cáo thành công.");
        }
    }
}
