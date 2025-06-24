namespace DoAnTotNghiep.DTOs
{
    public class UserToken
    {
        public string? Email {  get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public string? Roles { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? Image {  get; set; }
        public string? NickName {  get; set; }
        public string? VerifyCode { get; set; }
        public string? Phonenumber { get; set; }
        public string? Password { get; set; }
        public bool? isVip { get; set; }
        public string? NewEmail { get; set; }
        public string? OTP { get; set; }

    }
}
