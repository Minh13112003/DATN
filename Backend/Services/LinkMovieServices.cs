using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class LinkMovieServices : ILinkMovieServices
    {
        private readonly ILinkMovieRepository _linkMovieRepository;

        public LinkMovieServices(ILinkMovieRepository linkMovieRepository)
        {
            _linkMovieRepository = linkMovieRepository;
        }

        public async Task<bool> AddLinkMovie(LinkMovieToAddDTOs linkMovieDTOs)
        {
            return await _linkMovieRepository.AddLinkMovie(linkMovieDTOs);
        }

        public async Task<bool> DeleteLinkMovie(string id)
        {
            return await _linkMovieRepository.DeleteLinkMovie(id);
        }

        public async Task<List<LinkMovieToShowDTOs>> GetMovieByIdMovie(string IdMovie)
        {
            return await _linkMovieRepository.GetMovieByIdMovie(IdMovie);
        }

        public async Task<List<LinkMovieToShowDTOs>> ShowAllLinkMovie()
        {
            return await _linkMovieRepository.ShowAllLinkMovie();
        }

        public async Task<bool> UpdateLinkMovie(LinkMovieDTOs linkMovie)
        {
            return await _linkMovieRepository.UpdateLinkMovie(linkMovie);
        }
    }
}
