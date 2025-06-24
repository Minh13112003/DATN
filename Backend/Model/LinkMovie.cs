using System.Text.Json.Serialization;

namespace DoAnTotNghiep.Model
{
    public class LinkMovie
    {
        public string IdLinkMovie { get; set; } = string.Empty;
        public string IdMovie {  get; set; } = string.Empty;
        public int Episode { get; set; }
        public string UrlMovie {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public Movie? Movie { get; set; }
    }
}
