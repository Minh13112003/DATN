namespace DoAnTotNghiep.DTOs
{
    public class CommentToShowDTOs
    {
        public string IdComment { get; set; } = string.Empty;
        public string IdUserName { get; set; } = string.Empty;
        public string IdMovie { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string TimeComment { get; set; } = string.Empty;
        public string Title {  get; set; } = string.Empty;
        public bool IsReported {  get; set; }
    }
}
