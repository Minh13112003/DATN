namespace DoAnTotNghiep.DTOs
{
    public class MovieFilterDto
    {
        public List<string>? Genres { get; set; }
        public List<string>? Countries { get; set; }
        public List<string>? Type { get; set; }
        public List<string>? Status { get; set; }
    }
}
