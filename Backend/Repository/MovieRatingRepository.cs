using DoAnTotNghiep.Data;
using DoAnTotNghiep.Helper.RatingResult;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace DoAnTotNghiep.Repository
{
    public class MovieRatingRepository : IMovieRatingRepository
    {

        private readonly DatabaseContext _databaseContext;
        public MovieRatingRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<AddRatingResult> AddRating(string IdMovie, string UserName, int point)
        {
            if(IdMovie == null || UserName == null) return AddRatingResult.MissingInfo;
            var IsWatched = await _databaseContext.History.FirstOrDefaultAsync(h => h.IdMovie == IdMovie && h.UserName == UserName);
            if (IsWatched == null) return AddRatingResult.NotWatched;
            var IsRated = await _databaseContext.MovieRatings.FirstOrDefaultAsync(h => h.IdMovie == IdMovie && h.UserName == UserName);
            if(IsRated == null)
            {
                _databaseContext.MovieRatings.Add(new MovieRating
                {
                    IdMovie = IdMovie,
                    UserName = UserName,
                    RatePoint = point
                });

                await _databaseContext.SaveChangesAsync();

                // Cập nhật điểm trung bình (như bạn làm ở trên)
                await _databaseContext.Database.ExecuteSqlRawAsync(@"
                    UPDATE ""Movie"" m
                    SET ""Point"" = avg_table.""AveragePoint""
                    FROM (
                        SELECT ""IdMovie"", AVG(""RatePoint"") AS ""AveragePoint""
                        FROM ""MovieRating""
                        WHERE ""IdMovie"" = {0}
                        GROUP BY ""IdMovie""
                    ) AS avg_table
                    WHERE m.""Id"" = avg_table.""IdMovie"";
                ", IdMovie);
                return AddRatingResult.Success;
            }
            return AddRatingResult.AlreadyRated;
        }

        public async Task<int> GetRating(string IdMovie, string UserName)
        {
            if (string.IsNullOrEmpty(IdMovie) || string.IsNullOrEmpty(UserName))
            {
                return -2;
            }
            else
            {
                var rating = await _databaseContext.MovieRatings.FirstOrDefaultAsync(i => i.IdMovie == IdMovie && i.UserName == UserName);
                if (rating == null) return -1;
                else return rating.RatePoint;
            }
        }
    }
}
