using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.SlugifyHelper;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;


namespace DoAnTotNghiep.Repository
{
    public class ActorRepository : IActorRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ActorRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<bool> DeleteActor(string IdActor)
        {
            if (IdActor == null) return false;
            var actorToDelete = await _databaseContext.Actors.FirstOrDefaultAsync(i => i.IdActor == IdActor);
            if (actorToDelete == null) return false;
            _databaseContext.Actors.Remove(actorToDelete);
            return true;
        }

        public async Task<ActorDTOs> GetActorBySlugName(string SlugActorName)
        {
            return await _databaseContext.Actors
                .Where(i=>i.SlugActorName == SlugActorName)
                .Select(a => new ActorDTOs
                {
                    IdActor = a.IdActor,
                    ActorName = a.ActorName,
                    BirthDay = a.BirthDay,
                    UrlImage = a.UrlImage,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<Actor>> GetAllActor()
        {
            // Truy vấn dữ liệu từ DbContext (EF Core hiểu được ToListAsync)
            return await _databaseContext.Actors.ToListAsync();

            // Chuyển sang DTO (thực hiện trong bộ nhớ, tránh lỗi mapping LINQ)
            
        }



        public async Task<bool> UpdateActor(ActorDTOs actor)
        {
            if (!string.IsNullOrEmpty(actor.IdActor))
            {
                var actorToUpdate = await _databaseContext.Actors.FirstOrDefaultAsync(i => i.IdActor == actor.IdActor);
                if (actorToUpdate != null) 
                {
                    actorToUpdate.ActorName = actor.ActorName;
                    actorToUpdate.BirthDay = actor.BirthDay;
                    actorToUpdate.SlugActorName = SlugHelper.Slugify(actor.ActorName);
                    actorToUpdate.UrlImage = actor.UrlImage;
                    await _databaseContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
    }
}
