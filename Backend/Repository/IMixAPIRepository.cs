using DoAnTotNghiep.DTOs;

namespace DoAnTotNghiep.Repository
{
    public interface IMixAPIRepository
    {
        Task<dynamic> GetMovieTypeAndCategory();
        Task<dynamic> GetcountMovieAndCategory();
        Task<List<MoviePointViewDTO>> GetMovieAndPoint(string sortby);
    }
}
