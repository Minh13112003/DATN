using DoAnTotNghiep.Helper.DateTimeVietNam;

namespace DoAnTotNghiep.DTOs
{
    public class NotificationToAddDTO
    {
        public string? Idcomment { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string? TitleMovie { get; set; }
    }
}
