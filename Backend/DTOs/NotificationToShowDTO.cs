using DoAnTotNghiep.Helper.DateTimeVietNam;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DoAnTotNghiep.DTOs
{
    public class NotificationToShowDTO
    {
        public string IdNotice { get; set; }
        public string? Idcomment { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
       
        public string? TitleMovie { get; set; }
        [NotMapped]
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }
        public string? CreatedAtString => CreatedAt.HasValue ? DateTimeHelper.ToStringDateTime(CreatedAt.Value) : null;
    }
}
