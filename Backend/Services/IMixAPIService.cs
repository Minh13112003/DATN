using DoAnTotNghiep.DTOs;

namespace DoAnTotNghiep.Services
{
    public interface IMixAPIService
    {
        Task<dynamic> GetMovieTypeAndCategory();
        Task<dynamic> GetcountMovieAndCategory();
        Task<List<MoviePointViewDTO>> GetMovieAndPoint(string sortBy);
    }
}
