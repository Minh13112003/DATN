using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class ActorServices : IActorServices
    {
        private readonly IActorRepository _actorRepository;

        public ActorServices(IActorRepository actorRepository ) {
            _actorRepository = actorRepository;
        }
        public async Task<bool> DeleteActor(string IdActor)
        {
            return await _actorRepository.DeleteActor(IdActor);
        }

        public async Task<ActorDTOs> GetActorBySlugName(string SlugActorName)
        {
            return await _actorRepository.GetActorBySlugName(SlugActorName);   
        }

        public async Task<List<Actor>> GetAllActor()
        {
            return await _actorRepository.GetAllActor();
        }

        public async Task<bool> UpdateActor(ActorDTOs actor)
        {
            return await _actorRepository.UpdateActor(actor);
        }
    }
}
