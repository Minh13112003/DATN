namespace DoAnTotNghiep.Model
{
    public class MovieRating
    {
        public string IdMovie {  get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int RatePoint { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
