using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string UserName {  get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public string OTP {  get; set; } = string.Empty;
    }
}
