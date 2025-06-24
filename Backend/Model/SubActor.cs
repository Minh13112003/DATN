namespace DoAnTotNghiep.Model
{
    public class SubActor
    {
        public string IdMovie { get; set; } = string.Empty;
        public string IdActor { get; set; } = string.Empty;
        public Movie? Movie { get; set; }
        public Actor? Actor { get; set; }
    }
}
