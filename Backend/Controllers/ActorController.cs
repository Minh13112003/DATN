using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.SlugifyHelper;
using DoAnTotNghiep.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/actor")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorServices _actorServices;
        public ActorController(IActorServices actorServices)
        {
            _actorServices = actorServices;
        }

        [HttpGet("GetActorByName/{actorName}")]
        public async Task<IActionResult> GetActorByName(string actorName)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (actorName.IsNullOrEmpty()) return BadRequest(new { message = "Không được để trống" });
                string ActorNameSlug = SlugHelper.Slugify(actorName);
                var Actor = await _actorServices.GetActorBySlugName(ActorNameSlug);
                if (Actor == null) return BadRequest(new { message = "Không tìm thấy tên diễn viên" });
                return Ok(Actor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /*[Authorize(Roles = "Admin")]*/
        [HttpGet("GetAllActor")]
        public async Task<IActionResult> GetAllActor()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var Actor = await _actorServices.GetAllActor();
            if (Actor == null) return BadRequest(new { message = "Không có diễn viên" });
            return Ok(Actor);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateActor")]
        public async Task<IActionResult> UpdateActor(ActorDTOs actorDTOs)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (actorDTOs == null) return BadRequest(new { message = "không được để trống thông tin" });
                var actorToUpdate = await _actorServices.UpdateActor(actorDTOs);
                if (actorToUpdate == false) return BadRequest(new { message = "Cập nhật diễn viên thất bại" });
                return Ok(new { message = "Cập nhật diễn viên thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteActor/{idActor}")]
        public async Task<IActionResult> DeleteActor(string idActor)
        {
            try
            {
                if (idActor.IsNullOrEmpty()) return BadRequest(new { message = "Id không được để trống" });
                var actorToDelete = await _actorServices.DeleteActor(idActor);
                if (actorToDelete == false) return BadRequest(new { message = "Không tìm thấy phim hoặc xóa phim thất bại" });
                return Ok(new { message = "Xóa phim thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
