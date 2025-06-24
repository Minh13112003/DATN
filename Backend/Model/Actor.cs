namespace DoAnTotNghiep.Model
{
    public class Actor
    {
        public string IdActor { get; set; } = string.Empty;
        public string ActorName { get; set; } = string.Empty;
        public string? BirthDay { get; set; } = string.Empty;
        public string SlugActorName { get; set; } = string.Empty;
        public string? UrlImage { get; set; } = string.Empty;
        public virtual List<SubActor>? SubActors { get; set; }
        
    }
}
