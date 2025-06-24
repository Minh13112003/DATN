using DoAnTotNghiep.Helper.DateTimeVietNam;

namespace DoAnTotNghiep.Model
{
    public class Notification
    {
        public string IdNotice { get; set; }
        public string? Idcomment { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetDateTimeVnNowWithDateTime();
        public string? TitleMovie { get; set; }

    }
}
