using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DoAnTotNghiep.DTOs
{
    public class ReportToShowDTOs
    {
        public string IdReport { get; set; } = string.Empty; 
        public string? IdMovie { get; set; } = string.Empty;
        public string? IdComment { get; set; } = string.Empty;
        public string? NameOfUserReported {  get; set; } = string.Empty;
        public string? ContentCommentReported {  get; set; } = string.Empty;
        public string UserNameReporter { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? UserNameAdminFix { get; set; } = string.Empty;
        public string? Response { get; set; } = string.Empty;
        public string TimeReport { get; set; } = string.Empty;
        public string TimeResponse { get; set; } = string.Empty;
        [JsonIgnore]
        public int Status { get; set; }
        [NotMapped]
        public string StatusText => !MovieStatusDict.TryGetValue(Status, out var statusText) ? "Không xác định" : statusText;

        private static readonly Dictionary<int, string> MovieStatusDict = new Dictionary<int, string>
        {
            { 0, "Chưa xử lý" },
            { 1, "Đang xử lý" },
            { 2, "Đã phản hồi" },        
        };
    }
}
