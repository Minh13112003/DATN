using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Services
{
    public interface IActorServices
    {
        Task<List<Actor>> GetAllActor();
        Task<ActorDTOs> GetActorBySlugName(string SlugActorName);
        Task<bool> UpdateActor(ActorDTOs actor);
        Task<bool> DeleteActor(string IdActor);
    }
}
