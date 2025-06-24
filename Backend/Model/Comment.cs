namespace DoAnTotNghiep.Model
{
    public class Comment
    {
        public string IdComment { get; set; } = string.Empty;
        public string IdUserName { get; set; } = string.Empty;
        public string IdMovie {  get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
        public string TimeComment {  get; set; } = string.Empty;
        public virtual Movie? Movie { get; set; }

    }
}
