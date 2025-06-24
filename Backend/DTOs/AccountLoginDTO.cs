using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;


namespace DoAnTotNghiep.DTOs
{
    public class AccountLoginDTO
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string? EmailAddress { get; set; }
        public DateTime Birthday { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
