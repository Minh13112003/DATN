using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Repository
{
    public interface IMovieRepository
    {
        
        Task<PaginatedMoviesResultDTO> GetFavoriteMoviesBySlugTitlesAsync(List<string> slugTitles, int pageNumber, int pageSize);

        Task<bool> IncreaseMovieView(string titleSlug);
        Task<PaginatedMoviesResultDTO> GetNewestMovie(int pageNumber, int pageSize);
        Task<bool> AddHistoryMovie(string IdMovie, string UserName);
        Task<PaginatedMoviesResultDTO> GetHistoryMovie(string UserName, int pageNumber, int pageSize);
        Task<PaginatedMoviesResultDTO> GetFilteredMovies(MovieFilterDto filter, int pageNumber, int pageSize);
        Task<PaginatedMoviesResultDTO> GetAllMovie(string role, int pageNumber, int pageSize);
        Task<bool> CreateSubCategory(string IdMovie, string IdCategory);
        Task<List<MovieToShowDTOs>> GetMovieByTitleSlug(string titleSlug);
        Task<PaginatedMoviesResultDTO> GetMovieByType(string slugtype, int pageNumber, int pageSize);
        Task<bool> DeleteMovie(string IdMovie);
        Task<bool> AddMovie(MovieToAddDTOs movieToAddDTOs);
        Task<PaginatedMoviesResultDTO> GetMovieByCategory(string category, int pageNumber, int pageSize);
        Task<bool> UpdateMovie(MovieToUpdateDTOs movieToAddDTOs);
        Task<PaginatedMoviesResultDTO> SearchMovie(string? keyword, string role, int pageNumber, int pageSize);
        Task<PaginatedMoviesResultDTO> GetMovieByNation(string slugNation, int pageNumber, int pageSize);
        Task<PaginatedMoviesResultDTO> GetMovieByStatus(string status, int pageNumber, int pageSize);
        Task<PaginatedMoviesResultDTO> GetMovieByActor(string actor, int pageNumber, int pageSize);
        Task<List<MovieToShowDTOs>> GetMovieById(string id);
        Task<List<MovieToShowDTOs>> GetRelatedMoviesByCategories(string titleSlug, int limit = 6);
    }
}
