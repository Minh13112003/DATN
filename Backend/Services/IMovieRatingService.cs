using DoAnTotNghiep.Helper.RatingResult;

namespace DoAnTotNghiep.Services
{
    public interface IMovieRatingService
    {
        Task<AddRatingResult> AddRating(string IdMovie, string UserName, int point);
        Task<int> GetRating(string IdMovie, string UserName);
    }
}
