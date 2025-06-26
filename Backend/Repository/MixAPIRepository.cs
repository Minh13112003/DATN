
using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace DoAnTotNghiep.Repository
{
    public class MixAPIRepository : IMixAPIRepository
    {
        private readonly DatabaseContext _databaseContext;
        public MixAPIRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<dynamic> GetcountMovieAndCategory()
        {
            var NumberOfMovie = await _databaseContext.Movies.CountAsync();
            var NumberOfCategory = await _databaseContext.Categories.CountAsync();
            var NumberOfComments = await _databaseContext.Comments.CountAsync();
            var NumberOfFeedbacks = await _databaseContext.Reports.CountAsync();
            var PendingFeedbacks = await _databaseContext.Reports.Where(i => i.Status == 0).CountAsync();
            var sqlday = @"
                    SELECT TO_CHAR(""ViewAt"", 'YYYY-MM-DD') AS timePeriod, COUNT(*) AS viewCount
                    FROM ""History""
                    GROUP BY TO_CHAR(""ViewAt"", 'YYYY-MM-DD')
                    ORDER BY timePeriod
                ";
            var sqlmonth = @"
                    SELECT TO_CHAR(""ViewAt"", 'MM-YYYY') AS timePeriod, COUNT(*) AS viewCount
                    FROM ""History""
                    GROUP BY TO_CHAR(""ViewAt"", 'MM-YYYY')
                    ORDER BY timePeriod
                ";
            var sqlyear = @"
                    SELECT TO_CHAR(""ViewAt"", 'YYYY') AS timePeriod, COUNT(*) AS viewCount
                    FROM ""History""
                    GROUP BY TO_CHAR(""ViewAt"", 'YYYY')
                    ORDER BY timePeriod";
            var sqlNumberMovie  = @"
                    SELECT 
                        c.""NameCategories"" AS ""CategoryName"",
                        COUNT(DISTINCT sc.""IdMovie"") AS ""MovieCount""
                    FROM ""Category"" c
                    LEFT JOIN ""SubCategories"" sc ON c.""IdCategories"" = sc.""IdCategory""
                    LEFT JOIN ""Movie"" m ON sc.""IdMovie"" = m.""Id""
                    WHERE m.""Block"" = FALSE
                    GROUP BY c.""NameCategories""
                    ORDER BY ""MovieCount"" DESC;
                ";
            var sqlStatus = @"
                SELECT 
                    s.statusText AS ""StatusText"",
                    COUNT(m.""Status"") AS ""MovieCount""
                FROM (
                    VALUES
                        ('0', 'Chưa có lịch'),
                        ('1', 'Sắp chiếu'),
                        ('2', 'Đang cập nhật'),
                        ('3', 'Đang chiếu'),
                        ('4', 'Đã kết thúc'),
                        ('5', 'Đã hoàn thành')
                ) AS s(statusCode, statusText)
                LEFT JOIN ""Movie"" m ON m.""Status"" = s.statusCode
                GROUP BY s.statusText, s.statusCode
                ORDER BY s.statusCode;
            ";
            var StatDay = await _databaseContext.Database.SqlQueryRaw<ViewStatsDto>(sqlday).ToListAsync();
            var StatMonth = await _databaseContext.Database.SqlQueryRaw<ViewStatsDto>(sqlmonth).ToListAsync();
            var StatYear = await _databaseContext.Database.SqlQueryRaw<ViewStatsDto>(sqlyear).ToListAsync();
            var ListCategoryAndNumber = await _databaseContext.Database.SqlQueryRaw<CategoryMovieCountDTOs>(sqlNumberMovie).ToListAsync();
            var ListMovieStatus = await _databaseContext.Database.SqlQueryRaw<MovieStatusCountDto>(sqlStatus).ToListAsync();
            return new { NumberOfMovie, NumberOfCategory, NumberOfComments, NumberOfFeedbacks, PendingFeedbacks 
                ,StatDay, StatMonth, StatYear, ListCategoryAndNumber, ListMovieStatus
                
            };
        }
        public async Task<List<MoviePointViewDTO>> GetMovieAndPoint(string sortBy)
        {
            var sortField = sortBy == "point" ? "\"Point\"" : "\"View\"";

            var sql = $@"
                SELECT 
                    ""Id"",
                    ""Title"",
                    ""Point"",
                    ""View""
                FROM ""Movie""
                ORDER BY {sortField} DESC;
            ";
            var movieStats = await _databaseContext.Database
                .SqlQueryRaw<MoviePointViewDTO>(sql)
                .ToListAsync();
            return movieStats;
        }
        /*public async Task<dynamic> GetMovieTypeAndCategory()
        {
            var Category = await _databaseContext.Categories.ToListAsync();
            var movies = await _databaseContext.Movies
            .Where(m => m.Block == false)
            .Select(m => new
            {
                title = m.Title,
                slugtitle = m.SlugTitle,
                description = m.Description,
                nameMovieType = m.TypeMovie,
                slugNameMovieType = m.SlugTypeMovie,
                nation = m.Nation,
                slugNation = m.SlugNation,
                status = m.Status,
                image = m.Image,
                nameCategories = string.Join(", ",
                m.SubCategories.Select(sc => sc.Category.NameCategories)), // Lấy danh sách thể loại của phim
                slugnameCategories = string.Join(", ",
                m.SubCategories.Select(sc => sc.Category.SlugNameCategories)),
                backgroundImage = m.BackgroundImage
            })

            .ToListAsync();
            var movieTypes = movies
            .Select(m => new { m.nameMovieType, m.slugNameMovieType })
            .Distinct()
            .ToList();

            var Nations = movies.Select(m => new { m.nation, m.slugNation }).Distinct().ToList();
            var Statuses = movies.Select(m => m.status).Distinct().ToList();
            var Movie = movies.Select(m => new { m.title, m.slugtitle, m.nation, m.nameCategories, m.slugnameCategories, m.backgroundImage, m.description, m.image }).ToList();
            return new { Category, movieTypes, Nations, Statuses, Movie };
        }*/

        public async Task<dynamic> GetMovieTypeAndCategory()
{
            var Category = await _databaseContext.Categories.ToListAsync();

            var movies = await _databaseContext.Movies
                .Where(m => m.Block == false)
                .Select(m => new
                {
                    idmovie = m.IdMovie,
                    title = m.Title,
                    slugTitle = m.SlugTitle,
                    description = m.Description,
                    nameMovieType = m.TypeMovie,
                    slugNameMovieType = m.SlugTypeMovie,
                    nation = m.Nation,
                    slugNation = m.SlugNation,
                    status = m.Status,
                    image = m.Image,
                    nameCategories = string.Join(", ", m.SubCategories.Select(sc => sc.Category.NameCategories)),
                    slugnameCategories = string.Join(", ", m.SubCategories.Select(sc => sc.Category.SlugNameCategories)),
                    backgroundImage = m.BackgroundImage
                })
                .ToListAsync();

            var movieTypes = movies
                .Select(m => new { m.nameMovieType, m.slugNameMovieType })
                .Distinct()
                .ToList();

            var Nations = movies
                .Select(m => new { m.nation, m.slugNation })
                .Distinct()
                .ToList();
            var MovieTitle = movies
                .Select(m => new { m.title, m.slugTitle })
                .Distinct()
                .ToList();
            var Statuses = movies
                .Select(m => m.status)
                .Distinct()
                .ToList();

            var Movie = movies
                .Select(m => new {
                    m.idmovie,
                    m.title,
                    m.slugTitle,
                    m.nation,
                    m.nameCategories,
                    m.slugnameCategories,
                    m.backgroundImage,
                    m.description,
                    m.image

                })
                .ToList();

            // ✅ Lấy danh sách phim mới nhất
            var sqlNewestMovies = @"
                    SELECT 
                        m.""Id"",
                        m.""Title"", 
                        m.""SlugTitle"",
                        m.""Nation"",
                        COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                        COALESCE(STRING_AGG(DISTINCT c.""SlugNameCategories"", ', '), '') AS ""SlugNameCategories"",
                        m.""BackgroundImage"",
                        m.""Description"",
                        m.""Image"",
                        lm.""CreatedAt"",
                        lm.""Episode""
                    FROM ""Movie"" m
                    LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                    LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                    LEFT JOIN (
                        SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""CreatedAt"", ""Episode"", ""UrlMovie""
                        FROM ""LinkMovie""
                        ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                    ) lm ON m.""Id"" = lm.""IdMovie""
                    WHERE m.""Block"" = false
                    GROUP BY 
                        m.""Title"", m.""SlugTitle"", m.""Nation"", m.""BackgroundImage"", 
                        m.""Description"", m.""Image"", m.""Id"",
                        lm.""CreatedAt"", lm.""Episode"", lm.""UrlMovie""
                    ORDER BY lm.""CreatedAt"" DESC NULLS LAST
                    LIMIT 12;
                ";

            var UpdateNewestMovie = await _databaseContext.Database.SqlQueryRaw<MovieSimpleDTOs>(sqlNewestMovies).ToListAsync();

            // ✅ Lấy danh sách phim đã hoàn thành (Status = 5)
            var FinishedMovie = await _databaseContext.Movies
            .Where(m => m.Block == false && m.Status == "5")
            .Select(m => new {
                m.IdMovie,
                m.Title,
                m.Description,
                m.Image,
                m.BackgroundImage,
                m.TypeMovie,
                m.SlugTitle,
                m.SlugNation,
                m.Nation,
                nameCategories = string.Join(", ", m.SubCategories.Select(sc => sc.Category.NameCategories)),
                slugNameCategories = string.Join(", ", m.SubCategories.Select(sc => sc.Category.SlugNameCategories))
            })
            .ToListAsync();


            return new
            {
                Category,
                movieTypes,
                Nations,
                Statuses,
                Movie,
                UpdateNewestMovie,
                FinishedMovie,
                MovieTitle
            };
        }

    }
}
