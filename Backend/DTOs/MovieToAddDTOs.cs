using System.Text.Json.Serialization;

namespace DoAnTotNghiep.DTOs
{
    public class MovieToAddDTOs
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string TypeMovie { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string NameCategories { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Quality { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool Block { get; set; } = false;
        public string NameDirector { get; set; } = string.Empty;
        public string NameActors {  get; set; } = string.Empty;
        public bool IsVip { get; set; } = false;
        

    }
}
