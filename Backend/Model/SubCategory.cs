namespace DoAnTotNghiep.Model
{
    public class SubCategory
    {     
        public string IdMovie { get; set; } = string.Empty;
        public string IdCategory {  get; set; } = string.Empty;
        public Movie? Movie { get; set; }
        public Category? Category { get; set; } 
        
    }
}
