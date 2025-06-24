using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Repository
{
    public interface ILinkMovieRepository
    {
        public Task<bool> AddLinkMovie(LinkMovieToAddDTOs linkMovieDTOs );
        public Task <bool> UpdateLinkMovie (LinkMovieDTOs linkMovie);
        public Task<bool> DeleteLinkMovie(string id);
        public Task<List<LinkMovieToShowDTOs>> ShowAllLinkMovie();
        public Task<List<LinkMovieToShowDTOs>> GetMovieByIdMovie(string IdMovie);
    }
}
