using DoAnTotNghiep.Helper.DateTimeVietNam;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace DoAnTotNghiep.DTOs
{
    public class LinkMovieDTOs
    {
        public string IdLinkMovie { get; set; } = string.Empty;
        public string IdMovie { get; set; } = string.Empty;
        public int Episode { get; set; }
        public string UrlMovie { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }
        [NotMapped]
        public string? CreatedAtString => CreatedAt.HasValue ? DateTimeHelper.ToStringDateTime(CreatedAt.Value) : null;
    }
}
