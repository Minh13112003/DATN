namespace DoAnTotNghiep.DTOs
{
    public class PaginatedMoviesResultDTO
    {
        public List<MovieToShowDTOs> Movies { get; set; }
        public int TotalRecords { get; set; }
        public int? TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
