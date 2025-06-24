using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;
using System.Reflection.Metadata.Ecma335;

namespace DoAnTotNghiep.Services
{

    public class MovieServices : IMovieServices
    {
        private readonly IMovieRepository _movieRepository;

        public MovieServices(IMovieRepository movieRepository)
        {
           _movieRepository = movieRepository;
        }

        public async Task<bool> AddHistoryMovie(string IdMovie, string UserName)
        {
           return await _movieRepository.AddHistoryMovie(IdMovie, UserName);
        }

        public async Task<bool> AddMovie(MovieToAddDTOs movieToAddDTOs)
        {
            return await _movieRepository.AddMovie(movieToAddDTOs);
        }

        public async Task<bool> CreateSubCategory(string IdMovie, string IdCategory)
        {
            return await _movieRepository.CreateSubCategory(IdMovie, IdCategory);
        }

        public async Task<bool> DeleteMovie(string IdMovie)
        {
            return await _movieRepository.DeleteMovie(IdMovie);
        }

        public async Task<PaginatedMoviesResultDTO> GetAllMovie(string role, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetAllMovie(role, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetFavoriteMoviesBySlugTitlesAsync(List<string> slugTitles, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetFavoriteMoviesBySlugTitlesAsync(slugTitles, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetFilteredMovies(MovieFilterDto filter, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetFilteredMovies(filter, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetHistoryMovie(string UserName, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetHistoryMovie(UserName, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByActor(string actor, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetMovieByActor(actor, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByCategory(string category, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetMovieByCategory(category, pageNumber, pageSize);
        }

        public async Task<List<MovieToShowDTOs>> GetMovieById(string id)
        {
            return await _movieRepository.GetMovieById(id);
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByNation(string nation, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetMovieByNation(nation, pageNumber, pageSize);
        }      

        public async Task<PaginatedMoviesResultDTO> GetMovieByStatus(string status, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetMovieByStatus(status, pageNumber, pageSize);
        }

        public async Task<List<MovieToShowDTOs>> GetMovieByTitleSlug(string titleSlug)
        {
            return await _movieRepository.GetMovieByTitleSlug(titleSlug);
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByType(string type, int pageNumber, int pageSize)
        {
            return await _movieRepository.GetMovieByType(type, pageNumber, pageSize);
        }

        public async Task<PaginatedMoviesResultDTO> GetNewestMovie(int pageNumber, int pageSize)
        {
            return await _movieRepository.GetNewestMovie(pageNumber,pageSize);
        }

        public async Task<List<MovieToShowDTOs>> GetRelatedMoviesByCategories(string titleSlug, int limit = 6)
        {
            return await _movieRepository.GetRelatedMoviesByCategories(titleSlug, limit);
        }

        public async Task<bool> IncreaseMovieView(string titleSlug)
        {
            return await _movieRepository.IncreaseMovieView(titleSlug);
        }

        public async Task<PaginatedMoviesResultDTO> SearchMovie(string? keyword, string role, int pageNumber, int pageSize)
        {
            return await _movieRepository.SearchMovie(keyword, role, pageNumber, pageSize);
        }

        public async Task<bool> UpdateMovie(MovieToUpdateDTOs movieToAddDTOs)
        {
            return await _movieRepository.UpdateMovie(movieToAddDTOs);
        }
    }
}
