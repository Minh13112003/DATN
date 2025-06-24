using System.Text.Json.Serialization;

namespace DoAnTotNghiep.Model
{
    public class Category
    {
        public string IdCategories { get; set; } = string.Empty;
        public string SlugNameCategories {  get; set; } = string.Empty;
        public string NameCategories { get; set; } = string.Empty;
        [JsonIgnore]
        public List<SubCategory>? SubCategories { get; }

    }
}
