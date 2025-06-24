using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace DoAnTotNghiep.Repository
{
    public class LinkMovieRepository : ILinkMovieRepository
    {
        private readonly DatabaseContext _databaseContext;
        public LinkMovieRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> AddLinkMovie(LinkMovieToAddDTOs linkMovieDTOs)
        {
            if (linkMovieDTOs == null) return false;
            var LinkMovie = await _databaseContext.LinkMovies.FirstOrDefaultAsync(i => i.UrlMovie == linkMovieDTOs.UrlMovie || i.IdMovie == linkMovieDTOs.IdMovie && i.Episode == linkMovieDTOs.Episode);
            if(LinkMovie != null) return false;
            LinkMovie linkMovie = new LinkMovie
            {
                IdLinkMovie = Guid.NewGuid().ToString(),
                IdMovie = linkMovieDTOs.IdMovie,
                Episode = linkMovieDTOs.Episode,
                UrlMovie = linkMovieDTOs.UrlMovie,
                CreatedAt = DateTimeHelper.GetDateTimeVnNowWithDateTime()
            };
            await _databaseContext.AddAsync(linkMovie);
            var Movie = await _databaseContext.Movies.FirstOrDefaultAsync(i => i.IdMovie == linkMovieDTOs.IdMovie);
            ++Movie.NumberOfMovie;
            if(Movie.Status == "0") Movie.Status = "2";     
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLinkMovie(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var linkmovie = await _databaseContext.LinkMovies.FirstOrDefaultAsync(i => i.IdLinkMovie == id);
                if (linkmovie != null)
                {
                     _databaseContext.LinkMovies.Remove(linkmovie);
                    await _databaseContext.SaveChangesAsync();
                    return true;
                }
                
            }
            return false;
        }

        public async Task<List<LinkMovieToShowDTOs>> GetMovieByIdMovie(string IdMovie)
        {
            if (IdMovie.IsNullOrEmpty()) return null;
            var sql = @"
                SELECT 
                    lm.""IdLinkMovie"",
                    lm.""IdMovie"",
                    lm.""Episode"",
                    lm.""UrlMovie"",
                    lm.""CreatedAt"",
                    m.""Title"",
                    m.""Image"",
                    m.""BackgroundImage""
                FROM ""LinkMovie"" lm
                LEFT JOIN ""Movie"" m ON lm.""IdMovie"" = m.""Id""
                WHERE lm.""IdMovie"" = @idmovie";
            return await _databaseContext.Database
        .SqlQueryRaw<LinkMovieToShowDTOs>(sql, new[] { new NpgsqlParameter("@idmovie", IdMovie) })
            .ToListAsync();

        }

        public async Task<List<LinkMovieToShowDTOs>> ShowAllLinkMovie()
        {
            var sql = @"
                SELECT 
                    lm.""IdLinkMovie"",
                    lm.""IdMovie"",
                    lm.""Episode"",
                    lm.""UrlMovie"",
                    lm.""CreatedAt"",
                    m.""Title"",
                    m.""Image"",
                    m.""BackgroundImage""
                FROM ""LinkMovie"" lm
                LEFT JOIN ""Movie"" m ON lm.""IdMovie"" = m.""Id""";

            return await _databaseContext.Database.SqlQueryRaw<LinkMovieToShowDTOs>(sql).ToListAsync();
        }
        

        public async Task<bool> UpdateLinkMovie(LinkMovieDTOs linkMovie)
        {
            if(linkMovie.IdLinkMovie != null)
            {
                var movie = await _databaseContext.LinkMovies.FirstOrDefaultAsync(i =>i.IdLinkMovie == linkMovie.IdLinkMovie);
                if (movie != null)
                {
                    movie.UrlMovie = linkMovie.UrlMovie;  
                    movie.Episode = linkMovie.Episode;
                    movie.UrlMovie = linkMovie.UrlMovie;
                    movie.CreatedAt = DateTimeHelper.GetDateTimeVnNowWithDateTime();
                    await _databaseContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
    }
}
