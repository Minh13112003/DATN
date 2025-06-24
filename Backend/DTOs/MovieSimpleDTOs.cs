using DoAnTotNghiep.Helper.DateTimeVietNam;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DoAnTotNghiep.DTOs
{
    public class MovieSimpleDTOs
    {
        public string Id {  get; set; }
        public string Title { get; set; }
        public string SlugTitle { get; set; }
        public string Nation { get; set; }
        public string NameCategories { get; set; }
        public string SlugNameCategories { get; set; }
        public string BackgroundImage { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }
        public int? Episode { get; set; }
        [NotMapped]
        public string? CreatedAtString => CreatedAt.HasValue ? DateTimeHelper.ToStringDateTime(CreatedAt.Value) : null;
    }
}
