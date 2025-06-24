using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace DoAnTotNghiep.Model
{
    public class AppUser : IdentityUser
    {        
        public int? Age {  get; set; }
        public string? FavoriteSlugTitle { get; set; }
        public bool IsVip {  get; set; }
        public string? OTP {  get; set; }
        public DateTime? OtpCreatedAt {  get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime? TimeTopUp { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string? Nickname { get; set; }
        public string? Image {  get; set; }
        public string? VerificationCode { get; set; }
    }
}
