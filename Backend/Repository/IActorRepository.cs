using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Repository
{
    public interface IActorRepository
    {
        Task<List<Actor>> GetAllActor();
        Task<ActorDTOs> GetActorBySlugName(string SlugActorName);
        Task<bool> UpdateActor(ActorDTOs actor);
        Task<bool> DeleteActor(string IdActor);
    }
}
