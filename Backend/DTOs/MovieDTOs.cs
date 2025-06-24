namespace DoAnTotNghiep.DTOs
{
    public class MovieDTOs
    {
        public string Title { get; set; } = string.Empty;
        public string SlugTitle {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string TypeMovie { get; set; } = string.Empty;
        public string SlugTypeMovie { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string NameCategories { get; set; } = string.Empty;
        public string SlugNameCategories { get; set; } = string.Empty;
        public int NumberOfMovie { get; set; }
        public int Duration { get; set; }
        public string Quality { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int View { get; set; }
        public string Image { get; set; } = string.Empty;
        public string UrlMovie {  get; set; } = string.Empty;
    }
}
