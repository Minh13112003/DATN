using DoAnTotNghiep.Helper.DateTimeVietNam;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DoAnTotNghiep.DTOs
{
    public class MovieToShowDTOs
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string TypeMovie { get; set; } = string.Empty;
        [JsonIgnore]
        public string Status { get; set; } = string.Empty;
        public string NameCategories { get; set; } = string.Empty;
        public int NumberOfMovie { get; set; }
        public int Duration { get; set; }
        public string Quality { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int View { get; set; }
        public bool Block { get; set; } = false;
        public string? NameDirector { get; set; } = string.Empty;
        public string? NameActors {  get; set; } = string.Empty;
        public bool IsVip { get; set; } = false;
        public string? UrlMovie { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public string? BackgroundImage { get; set; } = string.Empty;
        public decimal Point {  get; set; }
        public int? Episode { get; set; }
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }
        [NotMapped]

        public string? CreatedAtString => CreatedAt.HasValue ? DateTimeHelper.ToStringDateTime(CreatedAt.Value) : null;
        [NotMapped]
        public string StatusText => MovieStatusDict.TryGetValue(int.Parse(Status), out var statusText) ? statusText : "Không xác định";

        private static readonly Dictionary<int, string> MovieStatusDict = new Dictionary<int, string>
    {
        { 0, "Chưa có lịch" },
        { 1, "Sắp chiếu" },
        { 2, "Đang cập nhật" },
        { 3, "Đang chiếu" },
        { 4, "Đã kết thúc" },
        { 5, "Đã hoàn thành" }
    };
    }
}
