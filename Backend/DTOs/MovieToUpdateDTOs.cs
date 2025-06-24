namespace DoAnTotNghiep.DTOs
{
    public class MovieToUpdateDTOs
    {
        public string IdMovie { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string TypeMovie { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string NameCategories { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Quality { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool Block { get; set; } = false;
        public string NameDirector { get; set; } = string.Empty;
        public bool IsVip { get; set; } = false;
        public string NameActors { get; set; } = string.Empty;
    }
}
