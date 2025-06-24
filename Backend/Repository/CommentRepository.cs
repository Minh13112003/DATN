using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DoAnTotNghiep.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DatabaseContext _dbContext;
        public CommentRepository(DatabaseContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<bool> AddComment(string username, CommentDTOs commentDTOs)
        {
            if (commentDTOs.Content == null || commentDTOs.SlugTitle == null) return false;
            var Movie = await _dbContext.Movies.FirstOrDefaultAsync(i => i.SlugTitle == commentDTOs.SlugTitle);
            if (Movie == null) return false; 
            var comment = new Comment
            {
                IdComment = Guid.NewGuid().ToString(),
                IdUserName = username,
                IdMovie = Movie.IdMovie,
                Content = commentDTOs.Content,
                TimeComment = DateTimeHelper.GetdateTimeVNNow()
            };
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComment(string role,string username, string Idcomment)
        {
            if (username == null || Idcomment == null) return false;
            var comment = await _dbContext.Comments.FirstOrDefaultAsync(i => (i.IdUserName == username || role == "Admin") && i.IdComment == Idcomment);
            if (comment == null) return false;
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<CommentToShowDTOs>> GetAllComment(string UserName)
        {
            var sql = @"
        SELECT 
            cm.""IdComment"",
            cm.""IdMovie"", 
            cm.""IdUserName"", 
            cm.""Content"",
            cm.""TimeComment"",
            m.""Title"",
            CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM ""Report"" r 
                    WHERE r.""IdComment"" = cm.""IdComment"" 
                        AND r.""Status"" != 2
                        AND (r.""UserNameAdminFix"" = @username OR r.""Status"" = 0)
                )
                THEN CAST(1 AS bit)
                ELSE CAST(0 AS bit)
            END AS IsReported
        FROM ""Comment"" cm
        JOIN ""Movie"" m ON cm.""IdMovie"" = m.""Id""
    ";

            var param = new NpgsqlParameter("@username", UserName);

            return await _dbContext.Database
                .SqlQueryRaw<CommentToShowDTOs>(sql, param)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetCommentByMovie(string slugMovie)
        {
            var sql = @"
                    SELECT 
                        cm.""IdComment"",
                        cm.""IdMovie"", 
                        cm.""IdUserName"", 
                        cm.""Content"",
                        cm.""TimeComment""
                    FROM ""Comment"" cm
                    JOIN ""Movie"" m ON cm.""IdMovie"" = m.""Id""
                    WHERE m.""SlugTitle"" = @slugMovie;";

            return await _dbContext.Comments
                .FromSqlRaw(sql, new NpgsqlParameter("@slugMovie", slugMovie))
                .ToListAsync();
        }

        public Task<List<CommentToShowDTOs>> GetCommentReport(string UserName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateComment(string username,CommentUpdateDTOs commentDTOs)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var comment = await _dbContext.Comments.FirstOrDefaultAsync(i => i.IdUserName ==  username && i.IdComment == commentDTOs.IdComment);
                if (comment == null) return false;
                comment.Content = commentDTOs.Content;
                comment.TimeComment = DateTimeHelper.GetdateTimeVNNow();
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }   
}
