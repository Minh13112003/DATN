using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Helper.SlugifyHelper;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;




namespace DoAnTotNghiep.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DatabaseContext _dbContext;

        public MovieRepository(DatabaseContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        Dictionary<int, string> movieStatusDict = new Dictionary<int, string>
        {
            { 0, "Chưa có lịch" },
            { 1, "Sắp chiếu" },
            { 2, "Đang cập nhật" },
            { 3, "Đang chiếu" },
            { 4, "Đã kết thúc" },
            { 5, "Đã hoàn thành" }
        };

        public async Task<PaginatedMoviesResultDTO> GetFavoriteMoviesBySlugTitlesAsync(List<string> slugTitles, int pageNumber, int pageSize)
        {
            if (slugTitles == null || !slugTitles.Any())
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Tạo danh sách parameter kiểu: @slug0, @slug1, ...
            var parameters = new List<NpgsqlParameter>();
            var slugConditions = new List<string>();

            for (int i = 0; i < slugTitles.Count; i++)
            {
                var paramName = $"slug{i}"; 
                slugConditions.Add($"m.\"SlugTitle\" = @{paramName}"); 
                parameters.Add(new NpgsqlParameter(paramName, slugTitles[i])); 
            }

            var whereClause = string.Join(" OR ", slugConditions);

            var totalRecords = await _dbContext.Movies
                .Where(m => slugTitles.Contains(m.SlugTitle))
                .CountAsync();

            // Truy vấn lấy danh sách phim với phân trang
            var sql = $@"
                SELECT 
                    m.""Id"", 
                    m.""Title"",  
                    m.""Description"", 
                    m.""Nation"", 
                    m.""TypeMovie"", 
                    m.""Status"", 
                    m.""NumberOfMovie"",
                    m.""Duration"", 
                    m.""Quality"", 
                    m.""Language"", 
                    m.""View"", 
                    m.""Image"",
                    m.""IsVip"",
                    m.""Block"",
                    m.""NameDirector"",
                    m.""BackgroundImage"",
                    m.""Point"",
                    lm.""Episode"",
                    lm.""CreatedAt"",               
                    COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                    COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                    lm.""UrlMovie"" AS ""UrlMovie""
                FROM ""Movie"" m
                LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                LEFT JOIN (
                    SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                    FROM ""LinkMovie""
                    ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                ) lm ON m.""Id"" = lm.""IdMovie""
                LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                WHERE {whereClause}
                GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
                ORDER BY m.""Id""
                LIMIT @pageSize OFFSET @offset;";

            // Thêm tham số cho phân trang
            parameters.Add(new NpgsqlParameter("@pageSize", pageSize));
            parameters.Add(new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize));

            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> IncreaseMovieView(string titleSlug)
        {
            var movie = await _dbContext.Movies
                .FirstOrDefaultAsync(m => m.SlugTitle == titleSlug);

            if (movie == null) return false;

            movie.View += 1;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedMoviesResultDTO> GetNewestMovie(int pageNumber, int pageSize)
        {
            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi
            var totalRecords = await _dbContext.Movies
                .Where(m => m.Block == false)
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
                        SELECT 
                            m.""Id"", 
                            m.""Title"", 
                            m.""Description"", 
                            m.""Nation"", 
                            m.""TypeMovie"", 
                            m.""Status"", 
                            m.""NumberOfMovie"", 
                            m.""Duration"", 
                            m.""Quality"", 
                            m.""Language"", 
                            m.""View"", 
                            m.""Image"", 
                            m.""BackgroundImage"",
                            m.""IsVip"",
                            m.""Block"",
                            m.""NameDirector"",
                            m.""Point"",
                            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                            lm.""Episode"" AS ""Episode"",
                            lm.""CreatedAt"" AS ""CreatedAt"",
                            lm.""UrlMovie"" AS ""UrlMovie""
                        FROM ""Movie"" m
                        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                        LEFT JOIN (
                            SELECT DISTINCT ON (""IdMovie"") *
                            FROM ""LinkMovie""
                            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                        ) lm ON m.""Id"" = lm.""IdMovie""
                        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                        WHERE m.""Block"" = false
                        GROUP BY 
                            m.""Id"", m.""Title"", m.""Description"", m.""Nation"", m.""TypeMovie"", 
                            m.""Status"", m.""NumberOfMovie"", m.""Duration"", m.""Quality"", 
                            m.""Language"", m.""View"", m.""Image"", m.""BackgroundImage"", 
                            m.""IsVip"", m.""Block"", m.""NameDirector"", m.""Point"",
                            lm.""Episode"", lm.""CreatedAt"", lm.""UrlMovie""
                        ORDER BY lm.""CreatedAt"" DESC NULLS LAST
                        LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<bool> AddHistoryMovie(string IdMovie, string UserName)
        {
            if (IdMovie == null || UserName == null) return false;
            var History = await _dbContext.History.FirstOrDefaultAsync(h => h.IdMovie == IdMovie && h.UserName == UserName);
            if (History != null)
            {
                return true;
            }
            History history = new History
            {
                IdMovie = IdMovie,
                UserName = UserName,
                ViewAt = DateTimeHelper.GetDateTimeVnNowWithDateTime(),
            };
            await _dbContext.History.AddAsync(history);
            var movie = await _dbContext.Movies
                .FirstOrDefaultAsync(m => m.IdMovie == IdMovie);
            if (movie == null) return false;
            movie.View += 1;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedMoviesResultDTO> GetHistoryMovie(string UserName, int pageNumber, int pageSize)
        {
            if (string.IsNullOrEmpty(UserName))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi
            var totalRecords = await _dbContext.Movies
                .Join(_dbContext.History,
                    m => m.IdMovie,
                    h => h.IdMovie,
                    (m, h) => new { Movie = m, History = h })
                .Where(mh => mh.History.UserName == UserName && mh.Movie.Block == false)
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
                    SELECT 
                        m.""Id"", 
                        m.""Title"",  
                        m.""Description"", 
                        m.""Nation"", 
                        m.""TypeMovie"", 
                        m.""Status"", 
                        m.""NumberOfMovie"",
                        m.""Duration"", 
                        m.""Quality"", 
                        m.""Language"", 
                        m.""View"", 
                        m.""Image"",
                        m.""IsVip"",
                        m.""Block"",
                        m.""NameDirector"",
                        m.""BackgroundImage"",
                        m.""Point"",
                        lm.""Episode"",
                        lm.""CreatedAt"",               
                        COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                        COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                        lm.""UrlMovie"" AS ""UrlMovie""
                    FROM ""Movie"" m
                    INNER JOIN ""History"" h ON m.""Id"" = h.""IdMovie""
                    LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                    LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                    LEFT JOIN (
                        SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                        FROM ""LinkMovie""
                        ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                    ) lm ON m.""Id"" = lm.""IdMovie""
                    LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                    LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                    WHERE h.""UserName"" = @userName
                    AND m.""Block"" = false
                    GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
                    ORDER BY m.""Id""
                    LIMIT @pageSize OFFSET @offset;";

                        // Tạo danh sách tham số
                        var parameters = new List<NpgsqlParameter>
                {
                    new NpgsqlParameter("@userName", UserName),
                    new NpgsqlParameter("@pageSize", pageSize),
                    new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
                };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PaginatedMoviesResultDTO> GetFilteredMovies(MovieFilterDto filter, int pageNumber, int pageSize)
        {
            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            var whereClauses = new List<string>();
            var parameters = new List<NpgsqlParameter>();

            // Xây dựng điều kiện lọc
            if (filter.Genres?.Any() == true)
            {
                whereClauses.Add(@"
            m.""Id"" IN (
                SELECT sc.""IdMovie""
                FROM ""SubCategories"" sc
                JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                WHERE c.""SlugNameCategories"" = ANY(@genres)
                GROUP BY sc.""IdMovie""
                HAVING COUNT(DISTINCT c.""SlugNameCategories"") = @genresCount
            )");
                parameters.Add(new NpgsqlParameter("@genres", filter.Genres));
                parameters.Add(new NpgsqlParameter("@genresCount", filter.Genres.Count));
            }

            if (filter.Countries?.Any() == true)
            {
                whereClauses.Add(@"m.""SlugNation"" = ANY(@countries)");
                parameters.Add(new NpgsqlParameter("@countries", filter.Countries));
            }

            if (filter.Type?.Any() == true)
            {
                whereClauses.Add(@"m.""SlugTypeMovie"" = ANY(@types)");
                parameters.Add(new NpgsqlParameter("@types", filter.Type));
            }

            if (filter.Status?.Any() == true)
            {
                whereClauses.Add(@"CAST(m.""Status"" AS TEXT) = ANY(@status)");
                parameters.Add(new NpgsqlParameter("@status", filter.Status));
            }

            // Thêm điều kiện m.Block = false
            whereClauses.Add(@"m.""Block"" = false");

            // Xây dựng WHERE clause
            string whereSql = whereClauses.Count > 0
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : "WHERE m.\"Block\" = false";

            // Đếm tổng số bản ghi bằng LINQ
            var query = _dbContext.Movies.AsQueryable();

            if (filter.Genres?.Any() == true)
            {
                var genres = filter.Genres.ToList();
                var genreCount = genres.Count;
                query = query.Where(m => _dbContext.SubCategories
                    .Join(_dbContext.Categories,
                        sc => sc.IdCategory,
                        c => c.IdCategories,
                        (sc, c) => new { sc.IdMovie, c.SlugNameCategories })
                    .Where(sc => genres.Contains(sc.SlugNameCategories))
                    .GroupBy(sc => sc.IdMovie)
                    .Where(g => g.Count() == genreCount)
                    .Select(g => g.Key)
                    .Contains(m.IdMovie)); // Sửa từ m.IdMovie thành m.Id
            }

            if (filter.Countries?.Any() == true)
            {
                var countries = filter.Countries.ToList();
                query = query.Where(m => countries.Contains(m.SlugNation));
            }

            if (filter.Type?.Any() == true)
            {
                var types = filter.Type.ToList();
                query = query.Where(m => types.Contains(m.SlugTypeMovie));
            }

            if (filter.Status?.Any() == true)
            {
                var statuses = filter.Status.ToList();
                query = query.Where(m => statuses.Contains(m.Status.ToString()));
            }

            query = query.Where(m => m.Block == false);

            // Đếm tổng số bản ghi
            var totalRecords = await query.CountAsync();

            // Truy vấn SQL với phân trang
            string sql = $@"
                        SELECT 
                            m.""Id"", 
                            m.""Title"", 
                            m.""Description"", 
                            m.""Nation"", 
                            m.""TypeMovie"", 
                            m.""Status"", 
                            m.""NumberOfMovie"", 
                            m.""Duration"",
                            m.""Quality"", 
                            m.""Language"", 
                            m.""View"", 
                            m.""Image"",
                            m.""IsVip"",
                            m.""Block"",
                            m.""NameDirector"",
                            m.""BackgroundImage"",
                            m.""Point"",
                            lm.""Episode"",
                            lm.""CreatedAt"",
                            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
                        FROM ""Movie"" m
                        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                        LEFT JOIN (
                            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                            FROM ""LinkMovie""
                            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                        ) lm ON m.""Id"" = lm.""IdMovie""
                        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                        {whereSql}
                        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
                        ORDER BY m.""Id""
                        LIMIT @pageSize OFFSET @offset;";

            // Thêm tham số phân trang
            parameters.Add(new NpgsqlParameter("@pageSize", pageSize));
            parameters.Add(new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize));

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public async Task<bool> AddMovie(MovieToAddDTOs movieToAddDTOs)
        {
            string IdMovie = Guid.NewGuid().ToString();

            // Kiểm tra phim đã tồn tại chưa
            var existingMovie = await _dbContext.Movies.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Title == movieToAddDTOs.Title);

            if (existingMovie != null)
            {
                return false; // Phim đã tồn tại
            }
            int statusValue = movieStatusDict
    .FirstOrDefault(x => x.Value == movieToAddDTOs.StatusText).Key;

            // Tạo phim mới
            var movie = new Movie
            {
                IdMovie = IdMovie,
                Title = movieToAddDTOs.Title,
                SlugTitle = SlugHelper.Slugify(movieToAddDTOs.Title),
                Description = movieToAddDTOs.Description,
                Nation = movieToAddDTOs.Nation,
                TypeMovie = movieToAddDTOs.TypeMovie,
                SlugTypeMovie = SlugHelper.Slugify(movieToAddDTOs.TypeMovie),
                Status = statusValue.ToString(),
                NumberOfMovie = 0,
                Duration = movieToAddDTOs.Duration,
                Quality = movieToAddDTOs.Quality,
                Language = movieToAddDTOs.Language,
                Block = movieToAddDTOs.Block,
                NameDirector = movieToAddDTOs.NameDirector,
                SlugNameDirector = SlugHelper.Slugify(movieToAddDTOs.NameDirector),
                IsVip = movieToAddDTOs.IsVip,
                View = 0,
                SlugNation = SlugHelper.Slugify(movieToAddDTOs.Nation),
                Point = 0,
                Image = "hihi",
                BackgroundImage = "haha"
            };
            await _dbContext.Movies.AddAsync(movie);

            // Xử lý danh mục (Category)
            string[] categories = movieToAddDTOs.NameCategories.Split(',')
                .Select(c => c.Trim()) // Xóa khoảng trắng dư thừa
                .Distinct() // Loại bỏ trùng lặp
                .ToArray();

            string[] NameActors = movieToAddDTOs.NameActors.Split(',')
                .Select(c => c.Trim()) // Xóa khoảng trắng dư thừa
                .Distinct() // Loại bỏ trùng lặp
                .ToArray();

            foreach (string categoryName in categories)
            {
                var existingCategory = await _dbContext.Categories
                    .AsNoTracking() // Không để EF theo dõi entity này
                    .FirstOrDefaultAsync(u => u.NameCategories == categoryName);

                string categoryId;
                if (existingCategory == null)
                {
                    // Tạo mới Category
                    categoryId = Guid.NewGuid().ToString();
                    var newCategory = new Category
                    {
                        IdCategories = categoryId,
                        NameCategories = categoryName,
                        SlugNameCategories = SlugHelper.Slugify(categoryName)
                    };
                    await _dbContext.Categories.AddAsync(newCategory);
                }
                else
                {
                    categoryId = existingCategory.IdCategories;

                    // Đính kèm vào DbContext để tránh lỗi tracking
                    // _dbContext.Categories.Attach(existingCategory);
                }

                // Tạo mới SubCategory mà không tạo thực thể mới của Movie & Category
                var newSubCategory = new SubCategory
                {
                    IdMovie = IdMovie,
                    IdCategory = categoryId
                };
                await _dbContext.SubCategories.AddAsync(newSubCategory);
            }

            foreach (string nameActor in NameActors)
            {
                var existingActor = await _dbContext.Actors
                    .AsNoTracking() // Không để EF theo dõi entity này
                    .FirstOrDefaultAsync(u => u.ActorName == nameActor);

                string Idactor;
                if (existingActor == null)
                {
                    // Tạo mới Actor
                    Idactor = Guid.NewGuid().ToString();
                    var newActor = new Actor
                    {
                        IdActor = Idactor,
                        ActorName = nameActor,
                        SlugActorName = SlugHelper.Slugify(nameActor)
                    };
                    await _dbContext.Actors.AddAsync(newActor);
                }
                else
                {
                    Idactor = existingActor.IdActor;

                    // Đính kèm vào DbContext để tránh lỗi tracking
                    // _dbContext.Categories.Attach(existingCategory);
                }

                // Tạo mới SubCategory mà không tạo thực thể mới của Movie & Category
                var newSubActor = new SubActor
                {
                    IdMovie = IdMovie,
                    IdActor = Idactor

                };
                await _dbContext.SubActors.AddAsync(newSubActor);
            }

            // Lưu vào database
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateSubCategory(string IdMovie, string IdCategory)
        {
            var movieExists = await _dbContext.Movies.AnyAsync(i => i.IdMovie == IdMovie);
            var categoryExists = await _dbContext.Categories.AnyAsync(i => i.IdCategories == IdCategory);

            if (!movieExists || !categoryExists)
            {
                return false; // Nếu không tồn tại Movie hoặc Category, trả về false
            }

            var existingSubCategory = await _dbContext.SubCategories
                .FirstOrDefaultAsync(i => i.IdMovie == IdMovie && i.IdCategory == IdCategory);

            if (existingSubCategory != null)
            {
                return false; // Tránh chèn trùng lặp
            }

            var newSubCategory = new SubCategory
            {
                IdMovie = IdMovie,
                IdCategory = IdCategory,
                Movie = new Movie { IdMovie = IdMovie },
                Category = new Category { IdCategories = IdCategory }
            };

            // Đánh dấu Movie và Category là Unchanged để EF không chèn mới
            _dbContext.Entry(newSubCategory.Movie).State = EntityState.Unchanged;
            _dbContext.Entry(newSubCategory.Category).State = EntityState.Unchanged;

            try
            {
                await _dbContext.SubCategories.AddAsync(newSubCategory);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMovie(string IdMovie)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(i => i.IdMovie == IdMovie);
            if (movie != null)
            {
                _dbContext.Movies.Remove(movie);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<PaginatedMoviesResultDTO> GetAllMovie(string role, int pageNumber, int pageSize)
        {
            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi
            int totalRecords;
            if (role == "Admin")
            {
                totalRecords = await _dbContext.Movies.CountAsync();
            }
            else
            {
                totalRecords = await _dbContext.Movies
                    .Where(m => m.Block == false)
                    .CountAsync();
            }

            // Xây dựng truy vấn SQL với phân trang
            string sql;
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            if (role == "Admin")
            {
                sql = @"
            SELECT 
                m.""Id"", 
                m.""Title"", 
                m.""Description"", 
                m.""Nation"", 
                m.""TypeMovie"", 
                m.""Status"", 
                m.""NumberOfMovie"", 
                m.""Duration"", 
                m.""Quality"", 
                m.""Language"", 
                m.""View"", 
                m.""Image"", 
                m.""BackgroundImage"",
                m.""IsVip"",
                m.""Block"",
                m.""NameDirector"",
                m.""Point"",
                COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                lm.""UrlMovie"",
                lm.""Episode"",
                lm.""CreatedAt""
            FROM ""Movie"" m
            LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
            LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
            LEFT JOIN (
                SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                FROM ""LinkMovie""
                ORDER BY ""IdMovie"", ""CreatedAt"" DESC
            ) lm ON m.""Id"" = lm.""IdMovie""
            LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
            LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
            GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
            ORDER BY m.""Id""
            LIMIT @pageSize OFFSET @offset;";
            }
            else
            {
                sql = @"
            SELECT 
                m.""Id"", 
                m.""Title"", 
                m.""Description"", 
                m.""Nation"", 
                m.""TypeMovie"", 
                m.""Status"", 
                m.""NumberOfMovie"", 
                m.""Duration"", 
                m.""Quality"", 
                m.""Language"", 
                m.""View"", 
                m.""Image"", 
                m.""BackgroundImage"",
                m.""IsVip"",
                m.""Block"",
                m.""NameDirector"",
                m.""Point"",
                COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                lm.""UrlMovie"",
                lm.""Episode"",
                lm.""CreatedAt""
            FROM ""Movie"" m
            LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
            LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
            LEFT JOIN (
                SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                FROM ""LinkMovie""
                ORDER BY ""IdMovie"", ""CreatedAt"" DESC
            ) lm ON m.""Id"" = lm.""IdMovie""
            LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
            LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
            WHERE m.""Block"" = false
            GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
            ORDER BY m.""Id""
            LIMIT @pageSize OFFSET @offset;";
            }

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByCategory(string SlugCategory, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(SlugCategory))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi bằng LINQ
            var totalRecords = await _dbContext.Movies
                .Join(_dbContext.SubCategories,
                    m => m.IdMovie,
                    sc => sc.IdMovie,
                    (m, sc) => new { Movie = m, SubCategory = sc })
                .Join(_dbContext.Categories,
                    ms => ms.SubCategory.IdCategory,
                    c => c.IdCategories,
                    (ms, c) => new { ms.Movie, Category = c })
                .Where(mc => mc.Category.SlugNameCategories.Contains(SlugCategory) && mc.Movie.Block == false)
                .Select(mc => mc.Movie.IdMovie)
                .Distinct()
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
        WITH MovieFiltered AS (
            SELECT DISTINCT m.""Id""
            FROM ""Movie"" m
            LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
            LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
            WHERE c.""SlugNameCategories"" ILIKE '%' || @slugcategory || '%'
        )
        SELECT 
            m.""Id"", 
            m.""Title"",  
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"",
            m.""Duration"", 
            m.""Quality"", 
            m.""Language"",
            m.""IsVip"",
            m.""Block"",
            m.""NameDirector"",
            m.""View"", 
            m.""Image"",
            m.""BackgroundImage"",
            m.""Point"",
            lm.""Episode"",
            lm.""CreatedAt"",
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        WHERE m.""Id"" IN (SELECT ""Id"" FROM MovieFiltered)
        AND m.""Block"" = false
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY m.""Id""
        LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@slugcategory", SlugCategory),
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<MovieToShowDTOs>> GetMovieByTitleSlug(string titleSlug)
        {
            var sql = @"
                SELECT 
                    m.""Id"", 
                    m.""Title"",  
                    m.""Description"", 
                    m.""Nation"", 
                    m.""TypeMovie"", 
                    m.""Status"", 
                    m.""NumberOfMovie"",
                    m.""Duration"", 
                    m.""Quality"", 
                    m.""Language"", 
                    m.""View"", 
                    m.""Image"",
                    m.""IsVip"",
                    m.""Block"",
                    m.""NameDirector"",
                    m.""BackgroundImage"",
                    m.""Point"",
                    lm.""Episode"",
                    lm.""CreatedAt"",
                    COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                    COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                    COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
                    FROM ""Movie"" m
                LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                LEFT JOIN (
                    SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                    FROM ""LinkMovie""
                    ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                ) lm ON m.""Id"" = lm.""IdMovie""
                LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                WHERE m.""SlugTitle"" ILIKE '%' || @titleSlug || '%'
                AND m.""Block"" = false
                GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt"";";

            return await _dbContext.Database
        .SqlQueryRaw<MovieToShowDTOs>(sql, new[] { new NpgsqlParameter("@titleSlug", titleSlug) })
            .ToListAsync();
        }
        public async Task<List<MovieToShowDTOs>> GetMovieById(string id)
        {
            var sql = @"
                SELECT 
                    m.""Id"", 
                    m.""Title"",  
                    m.""Description"", 
                    m.""Nation"", 
                    m.""TypeMovie"", 
                    m.""Status"", 
                    m.""NumberOfMovie"",
                    m.""Duration"", 
                    m.""Quality"", 
                    m.""Language"", 
                    m.""View"", 
                    m.""Image"",
                    m.""IsVip"",
                    m.""Block"",
                    m.""NameDirector"",
                    m.""BackgroundImage"",
                    m.""Point"",
                    lm.""Episode"",
                    lm.""CreatedAt"",
                    COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                    COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                    COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
                    FROM ""Movie"" m
                LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                LEFT JOIN (
                    SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                    FROM ""LinkMovie""
                    ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                ) lm ON m.""Id"" = lm.""IdMovie""
                LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                WHERE m.""Id"" = @id
                GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt"";";

            return await _dbContext.Database
        .SqlQueryRaw<MovieToShowDTOs>(sql, new[] { new NpgsqlParameter("@id", id) })
            .ToListAsync();
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByType(string slugtype, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(slugtype))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi bằng LINQ
            var totalRecords = await _dbContext.Movies
                .Where(m => m.SlugTypeMovie.Contains(slugtype) && m.Block == false)
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
        SELECT 
            m.""Id"", 
            m.""Title"", 
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"", 
            m.""Duration"",
            m.""Quality"", 
            m.""Language"", 
            m.""View"", 
            m.""IsVip"",
            m.""Block"",
            m.""NameDirector"",
            m.""Image"", 
            m.""BackgroundImage"",
            m.""Point"",
            lm.""Episode"",
            lm.""CreatedAt"",
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        WHERE m.""SlugTypeMovie"" ILIKE '%' || @slugtype || '%'
        AND m.""Block"" = false
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY m.""Id""
        LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@slugtype", slugtype),
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedMoviesResultDTO> SearchMovie(string? keyword, string role, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(keyword))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            string slugKeyword = SlugHelper.Slugify(keyword);
            Dictionary<int, string> slugStatusDict = movieStatusDict.ToDictionary(
                x => x.Key,
                x => SlugHelper.Slugify(x.Value)
            );

            int totalRecords;
            string sql;
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            var foundPair = slugStatusDict.FirstOrDefault(x => x.Value == slugKeyword);
            if (!foundPair.Equals(default(KeyValuePair<int, string>)))
            {
                // Tìm kiếm theo trạng thái
                string whereCondition = @"WHERE m.""Status"" ILIKE '%' || @statusValue || '%'";
                if (role != "Admin")
                {
                    whereCondition += @" AND m.""Block"" = false";
                }

                // Đếm tổng số bản ghi bằng LINQ
                var statusQuery = _dbContext.Movies.AsQueryable();
                statusQuery = statusQuery.Where(m => m.Status.ToString().Contains(foundPair.Key.ToString()));
                if (role != "Admin")
                {
                    statusQuery = statusQuery.Where(m => m.Block == false);
                }
                totalRecords = await statusQuery.CountAsync();

                sql = $@"SELECT 
            m.""Id"", 
            m.""Title"", 
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"", 
            m.""Duration"", 
            m.""Quality"", 
            m.""Language"", 
            m.""View"", 
            m.""Image"", 
            m.""IsVip"", 
            m.""Block"", 
            m.""NameDirector"", 
            m.""BackgroundImage"", 
            m.""Point"", 
            lm.""Episode"", 
            lm.""CreatedAt"", 
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        {whereCondition}
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY m.""Id""
        LIMIT @pageSize OFFSET @offset;";

                parameters.Add(new NpgsqlParameter("@statusValue", foundPair.Key.ToString()));
            }
            else
            {
                // Tìm kiếm theo từ khóa
                totalRecords = await _dbContext.Movies
                    .Where(m =>
                        m.SlugTitle.Contains(slugKeyword) ||
                        m.SlugTypeMovie.Contains(slugKeyword) ||
                        m.SlugNation.Contains(slugKeyword) ||
                        _dbContext.SubCategories
                            .Join(_dbContext.Categories,
                                sc => sc.IdCategory,
                                c => c.IdCategories,
                                (sc, c) => new { sc.IdMovie, c.SlugNameCategories })
                            .Where(sc => sc.SlugNameCategories.Contains(slugKeyword))
                            .Select(sc => sc.IdMovie)
                            .Contains(m.IdMovie) ||
                        _dbContext.SubActors
                            .Join(_dbContext.Actors,
                                sa => sa.IdActor,
                                a => a.IdActor,
                                (sa, a) => new { sa.IdMovie, a.SlugActorName })
                            .Where(sa => sa.SlugActorName.Contains(slugKeyword))
                            .Select(sa => sa.IdMovie)
                            .Contains(m.IdMovie)
                    )
                    .Where(m => m.Block == false)
                    .CountAsync();

                sql = @"
            WITH MovieFiltered AS (
                SELECT DISTINCT m.""Id""
                FROM ""Movie"" m
                LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                WHERE c.""SlugNameCategories"" ILIKE '%' || @keyword || '%'
            ),
            ActorFiltered AS (
                SELECT DISTINCT m.""Id""
                FROM ""Movie"" m
                LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                WHERE a.""SlugActorName"" ILIKE '%' || @keyword || '%'
            )
            SELECT 
                m.""Id"", 
                m.""Title"", 
                m.""Description"", 
                m.""Nation"", 
                m.""TypeMovie"", 
                m.""Status"", 
                m.""NumberOfMovie"", 
                m.""Duration"", 
                m.""Quality"", 
                m.""Language"", 
                m.""View"", 
                m.""Image"", 
                m.""IsVip"", 
                m.""Block"", 
                m.""NameDirector"", 
                m.""BackgroundImage"", 
                m.""Point"", 
                lm.""Episode"", 
                lm.""CreatedAt"", 
                COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"", 
                COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"", 
                COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie"" 
            FROM ""Movie"" m 
            LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie"" 
            LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories"" 
            LEFT JOIN (
                SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie"" 
                FROM ""LinkMovie"" 
                ORDER BY ""IdMovie"", ""CreatedAt"" DESC 
            ) lm ON m.""Id"" = lm.""IdMovie"" 
            LEFT JOIN ""SubActor"" sa ON m.""Id"" = sc.""IdMovie"" 
            LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor"" 
            WHERE 
                (m.""SlugTitle"" ILIKE '%' || @keyword || '%'
                OR m.""SlugTypeMovie"" ILIKE '%' || @keyword || '%'
                OR m.""SlugNation"" ILIKE '%' || @keyword || '%'
                OR m.""Id"" IN (SELECT ""Id"" FROM ActorFiltered)
                OR m.""Id"" IN (SELECT ""Id"" FROM MovieFiltered)
                )
                AND m.""Block"" = false
            GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
            ORDER BY m.""Id""
            LIMIT @pageSize OFFSET @offset;";

                parameters.Add(new NpgsqlParameter("@keyword", slugKeyword));
            }

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<bool> UpdateMovie(MovieToUpdateDTOs movieToAddDTOs)
        {
            // Kiểm tra phim có tồn tại không
            var existingMovie = await _dbContext.Movies
                .FirstOrDefaultAsync(m => m.IdMovie == movieToAddDTOs.IdMovie);

            if (existingMovie == null)
            {
                return false; // Không tìm thấy phim
            }
            /*int statusValue = movieStatusDict
    .FirstOrDefault(x => x.Value == movieToAddDTOs.StatusText).Key;*/

            // Cập nhật thông tin Movie
            existingMovie.Title = movieToAddDTOs.Title;
            existingMovie.SlugTitle = SlugHelper.Slugify(movieToAddDTOs.Title);
            existingMovie.Description = movieToAddDTOs.Description;
            existingMovie.Nation = movieToAddDTOs.Nation;
            existingMovie.TypeMovie = movieToAddDTOs.TypeMovie;
            existingMovie.SlugTypeMovie = SlugHelper.Slugify(movieToAddDTOs.TypeMovie);
            existingMovie.Status = movieToAddDTOs.Status;
            existingMovie.Duration = movieToAddDTOs.Duration;
            existingMovie.Quality = movieToAddDTOs.Quality;
            existingMovie.Language = movieToAddDTOs.Language;
            existingMovie.Block = movieToAddDTOs.Block;
            existingMovie.NameDirector = movieToAddDTOs.NameDirector;
            existingMovie.SlugNameDirector = SlugHelper.Slugify(movieToAddDTOs.NameDirector);
            existingMovie.IsVip = movieToAddDTOs.IsVip;

            // Xóa tất cả SubCategory cũ liên quan đến Movie
            var oldSubCategories = _dbContext.SubCategories
                .Where(sc => sc.IdMovie == movieToAddDTOs.IdMovie);
            _dbContext.SubCategories.RemoveRange(oldSubCategories);

            var oldSubActor = _dbContext.SubActors
                .Where(sc => sc.IdMovie == movieToAddDTOs.IdMovie);
            _dbContext.SubActors.RemoveRange(oldSubActor);
            // Xử lý danh mục mới
            string[] categories = movieToAddDTOs.NameCategories.Split(',')
                .Select(c => c.Trim()) // Xóa khoảng trắng dư thừa
                .Distinct() // Loại bỏ trùng lặp
                .ToArray();

            string[] NameActors = movieToAddDTOs.NameActors.Split(',')
                .Select(c => c.Trim()) // Xóa khoảng trắng dư thừa
                .Distinct() // Loại bỏ trùng lặp
                .ToArray();

            foreach (string categoryName in categories)
            {
                var existingCategory = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.NameCategories == categoryName);

                string categoryId;
                if (existingCategory == null)
                {
                    // Tạo mới Category nếu chưa có
                    categoryId = Guid.NewGuid().ToString();
                    var newCategory = new Category
                    {
                        IdCategories = categoryId,
                        NameCategories = categoryName,
                        SlugNameCategories = SlugHelper.Slugify(categoryName)
                    };
                    await _dbContext.Categories.AddAsync(newCategory);
                }
                else
                {
                    categoryId = existingCategory.IdCategories;
                }

                // Thêm mới SubCategory
                var newSubCategory = new SubCategory
                {
                    IdMovie = movieToAddDTOs.IdMovie,
                    IdCategory = categoryId
                };
                await _dbContext.SubCategories.AddAsync(newSubCategory);
            }

            foreach (string nameActor in NameActors)
            {
                var existingActor = await _dbContext.Actors
                    .AsNoTracking() // Không để EF theo dõi entity này
                    .FirstOrDefaultAsync(u => u.ActorName == nameActor);

                string Idactor;
                if (existingActor == null)
                {
                    // Tạo mới Actor
                    Idactor = Guid.NewGuid().ToString();
                    var newActor = new Actor
                    {
                        IdActor = Idactor,
                        ActorName = nameActor,
                        SlugActorName = SlugHelper.Slugify(nameActor)
                    };
                    await _dbContext.Actors.AddAsync(newActor);
                }
                else
                {
                    Idactor = existingActor.IdActor;

                    // Đính kèm vào DbContext để tránh lỗi tracking
                    // _dbContext.Categories.Attach(existingCategory);
                }

                // Tạo mới SubCategory mà không tạo thực thể mới của Movie & Category
                var newSubActor = new SubActor
                {
                    IdMovie = movieToAddDTOs.IdMovie,
                    IdActor = Idactor

                };
                await _dbContext.SubActors.AddAsync(newSubActor);
            }

            // Lưu thay đổi vào database
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByNation(string slugNation, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(slugNation))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi bằng LINQ
            var totalRecords = await _dbContext.Movies
                .Where(m => m.SlugNation.ToLower().Contains(slugNation.ToLower()) && m.Block == false)
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
        SELECT 
            m.""Id"", 
            m.""Title"",  
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"",
            m.""Duration"", 
            m.""Quality"", 
            m.""Language"", 
            m.""View"", 
            m.""IsVip"",
            m.""Block"",
            m.""NameDirector"",
            m.""Image"", 
            m.""BackgroundImage"",
            m.""Point"",
            lm.""Episode"",
            lm.""CreatedAt"",
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        WHERE m.""SlugNation"" = @slugNation AND m.""Block"" = false
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY m.""Id""
        LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@slugNation", slugNation),
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByStatus(string status, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(status))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            string slugKeyword = SlugHelper.Slugify(status);
            Dictionary<int, string> slugStatusDict = movieStatusDict.ToDictionary(
                x => x.Key,
                x => SlugHelper.Slugify(x.Value)
            );

            var foundPair = slugStatusDict.FirstOrDefault(x => x.Value == slugKeyword);
            if (foundPair.Equals(default(KeyValuePair<int, string>)))
            {
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            // Đếm tổng số bản ghi bằng LINQ
            var totalRecords = await _dbContext.Movies
                .Where(m => m.Status.Contains(foundPair.Key.ToString()) && m.Block == false)
                .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
        SELECT 
            m.""Id"", 
            m.""Title"", 
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"", 
            m.""Duration"",
            m.""Quality"", 
            m.""Language"", 
            m.""View"", 
            m.""Image"",
            m.""IsVip"",
            m.""Block"",
            m.""NameDirector"",
            m.""BackgroundImage"",
            m.""Point"",
            lm.""Episode"",
            lm.""CreatedAt"",
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        WHERE m.""Status"" ILIKE '%' || @statusValue || '%' AND m.""Block"" = false
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY m.""Id""
        LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
    {
        new NpgsqlParameter("@statusValue", foundPair.Key.ToString()),
        new NpgsqlParameter("@pageSize", pageSize),
        new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
    };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedMoviesResultDTO> GetMovieByActor(string actor, int pageNumber, int pageSize)
        {
            // Kiểm tra tham số đầu vào
            if (string.IsNullOrEmpty(actor))
                return new PaginatedMoviesResultDTO
                {
                    Movies = new List<MovieToShowDTOs>(),
                    TotalRecords = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

            // Kiểm tra pageNumber và pageSize hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 2;

            // Đếm tổng số bản ghi bằng LINQ
            var totalRecords = await _dbContext.Movies
    .Where(m => !m.Block) // Sử dụng !m.Block thay vì m.Block == false cho ngắn gọn
    .Join(_dbContext.SubActors,
        m => m.IdMovie,
        sa => sa.IdMovie,
        (m, sa) => new { Movie = m, SubActor = sa })
    .Join(_dbContext.Actors,
        ms => ms.SubActor.IdActor,
        a => a.IdActor,
        (ms, a) => new { ms.Movie, Actor = a })
    .Where(ma => ma.Actor.SlugActorName.ToLower().Contains(actor.ToLower()) ||
                 ma.Movie.SlugNameDirector.ToLower().Contains(actor.ToLower()))
    .Select(ma => ma.Movie.IdMovie)
    .Distinct()
    .CountAsync();

            // Truy vấn SQL với phân trang
            var sql = @"
                WITH ActorFiltered AS (
                    SELECT DISTINCT m.""Id""
                    FROM ""Movie"" m
                    LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                    LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                    WHERE a.""SlugActorName"" ILIKE '%' || @actor || '%'
                        OR m.""SlugNameDirector"" ILIKE '%' || @actor || '%'
                )
                SELECT 
                    m.""Id"", 
                    m.""Title"",  
                    m.""Description"", 
                    m.""Nation"", 
                    m.""TypeMovie"", 
                    m.""Status"", 
                    m.""NumberOfMovie"",
                    m.""Duration"", 
                    m.""Quality"", 
                    m.""Language"",
                    m.""IsVip"",
                    m.""Block"",
                    m.""NameDirector"",
                    m.""View"", 
                    m.""Image"",
                    m.""BackgroundImage"",
                    m.""Point"",
                    lm.""Episode"",
                    lm.""CreatedAt"",
                    COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
                    COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
                    COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie""
                FROM ""Movie"" m
                LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
                LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
                LEFT JOIN (
                    SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
                    FROM ""LinkMovie""
                    ORDER BY ""IdMovie"", ""CreatedAt"" DESC
                ) lm ON m.""Id"" = lm.""IdMovie""
                LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
                LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
                WHERE m.""Id"" IN (SELECT ""Id"" FROM ActorFiltered)         
                AND m.""Block"" = false
                GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
                ORDER BY m.""Id""
                LIMIT @pageSize OFFSET @offset;";

            // Tạo danh sách tham số
            var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("@actor", actor),
                new NpgsqlParameter("@pageSize", pageSize),
                new NpgsqlParameter("@offset", (pageNumber - 1) * pageSize)
            };

            // Thực thi truy vấn
            var movies = await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(sql, parameters.ToArray())
                .ToListAsync();

            // Trả về kết quả phân trang
            return new PaginatedMoviesResultDTO
            {
                Movies = movies,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<List<MovieToShowDTOs>> GetRelatedMoviesByCategories(string titleSlug, int limit = 6)
        {
            // Bước 1: Lấy thông tin phim hiện tại
            var currentMovie = await GetMovieByTitleSlug(titleSlug);
            if (currentMovie == null || !currentMovie.Any())
            {
                return new List<MovieToShowDTOs>(); // Trả về danh sách rỗng nếu không tìm thấy phim
            }

            // Lấy danh sách thể loại của phim hiện tại
            var categories = currentMovie.First().NameCategories?.Split(", ", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            if (!categories.Any())
            {
                return new List<MovieToShowDTOs>(); // Trả về danh sách rỗng nếu phim không có thể loại
            }

            // Bước 2: Tạo truy vấn để tìm phim liên quan, sắp xếp theo số thể loại chung
            var sql = @"
        SELECT 
            m.""Id"", 
            m.""Title"",  
            m.""Description"", 
            m.""Nation"", 
            m.""TypeMovie"", 
            m.""Status"", 
            m.""NumberOfMovie"",
            m.""Duration"", 
            m.""Quality"", 
            m.""Language"", 
            m.""View"", 
            m.""Image"",
            m.""IsVip"",
            m.""Block"",
            m.""NameDirector"",
            m.""BackgroundImage"",
            m.""Point"",
            lm.""Episode"",
            lm.""CreatedAt"",
            COALESCE(STRING_AGG(DISTINCT c.""NameCategories"", ', '), '') AS ""NameCategories"",
            COALESCE(STRING_AGG(DISTINCT a.""ActorName"", ', '), '') AS ""NameActors"",
            COALESCE(STRING_AGG(DISTINCT lm.""UrlMovie"" || '-Tap-' || lm.""Episode"", ', '), '') AS ""UrlMovie"",
            COUNT(DISTINCT c.""NameCategories"") FILTER (WHERE c.""NameCategories"" = ANY(@categories)) AS ""MatchingCategoryCount""
        FROM ""Movie"" m
        LEFT JOIN ""SubCategories"" sc ON m.""Id"" = sc.""IdMovie""
        LEFT JOIN ""Category"" c ON sc.""IdCategory"" = c.""IdCategories""
        LEFT JOIN ""SubActor"" sa ON m.""Id"" = sa.""IdMovie""
        LEFT JOIN (
            SELECT DISTINCT ON (""IdMovie"") ""IdMovie"", ""Episode"", ""CreatedAt"", ""UrlMovie""
            FROM ""LinkMovie""
            ORDER BY ""IdMovie"", ""CreatedAt"" DESC
        ) lm ON m.""Id"" = lm.""IdMovie""
        LEFT JOIN ""Actor"" a ON sa.""IdActor"" = a.""IdActor""
        WHERE m.""Block"" = false
        AND m.""SlugTitle"" != @titleSlug -- Loại trừ phim hiện tại
        AND c.""NameCategories"" = ANY(@categories) -- Tìm phim có ít nhất một thể loại trùng
        GROUP BY m.""Id"", lm.""UrlMovie"", lm.""Episode"", lm.""CreatedAt""
        ORDER BY ""MatchingCategoryCount"" DESC, m.""View"" DESC -- Sắp xếp theo số thể loại chung, rồi theo lượt xem
        LIMIT @limit;";

            // Bước 3: Thực thi truy vấn
            return await _dbContext.Database
                .SqlQueryRaw<MovieToShowDTOs>(
                    sql,
                    new[]
                    {
                new NpgsqlParameter("@titleSlug", titleSlug),
                new NpgsqlParameter("@categories", categories),
                new NpgsqlParameter("@limit", limit)
                    })
                .ToListAsync();
        }
    }
}
