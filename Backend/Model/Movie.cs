namespace DoAnTotNghiep.Model
{
    public class Movie
    {
        public string IdMovie { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SlugTitle {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string TypeMovie { get; set; } = string.Empty;
        public string SlugTypeMovie {  get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int NumberOfMovie { get; set; }
        public int Duration { get; set; }
        public string Quality { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public int View { get; set; }
        public string? Image {  get; set; }
        public string? BackgroundImage {  get; set; }
        public string SlugNation { get; set; } = string.Empty;
        public bool Block {  get; set; } = false;
        public string NameDirector {  get; set; } = string.Empty;
        public string SlugNameDirector {  get; set; } = string.Empty;
        public bool IsVip { get; set; } = false;
        public decimal Point {  get; set; }
        public virtual List<SubCategory>? SubCategories { get;  }
        public virtual List<SubActor>? SubActors { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Report>? Reports { get; set; }
        public virtual ICollection<LinkMovie>? LinkMovies { get; set; }
        public virtual ICollection<History>? History { get; set; }
        public virtual ICollection<MovieRating>? MovieRating { get; set; }
        
    }
}
