
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class MixAPIServices : IMixAPIService
    { 
        private readonly IMixAPIRepository _mixAPIRepository;
        public MixAPIServices(IMixAPIRepository mixAPIRepository)
        {
            _mixAPIRepository = mixAPIRepository;
        }

        public async Task<dynamic> GetcountMovieAndCategory()
        {
            return await _mixAPIRepository.GetcountMovieAndCategory();
        }

        public async Task<List<MoviePointViewDTO>> GetMovieAndPoint(string sortBy)
        {
            return await _mixAPIRepository.GetMovieAndPoint(sortBy);
        }

        public async Task<dynamic> GetMovieTypeAndCategory()
        {
            return await _mixAPIRepository.GetMovieTypeAndCategory();
        }
    }
}
