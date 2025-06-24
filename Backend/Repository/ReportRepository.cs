using DoAnTotNghiep.Data;
using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Helper.DateTimeVietNam;
using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace DoAnTotNghiep.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ReportRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> DeleteReport(string IdReport)
        {
            if (!string.IsNullOrEmpty(IdReport))
            {
                var report = await _databaseContext.Reports.FirstOrDefaultAsync(i => i.IdReport == IdReport && (i.Status == 0 || i.Status == 2));
                if (report != null)
                {
                    _databaseContext.Reports.Remove(report);
                    await _databaseContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<Report>> GetCommentReport(string IdComment, string UserName)
        {
            if (string.IsNullOrEmpty(IdComment) || string.IsNullOrEmpty(UserName)) return null;
            var report = await _databaseContext.Reports.Where(i => i.IdComment == IdComment && ((i.UserNameAdminFix == UserName && i.Status != 2) || i.Status == 0)).ToListAsync();
            if (report != null)
            {
                return report;
            }
            List<Report> reports = new List<Report>();
            return reports;
        }
        public async Task<bool> ExecuteCommentReport(string IdComment, string UserName)
        {
            if (string.IsNullOrEmpty(IdComment) || string.IsNullOrEmpty(UserName)) return false;
            var reports = await _databaseContext.Reports.Where(i => i.IdComment == IdComment && ((i.UserNameAdminFix == UserName && i.Status != 2) || i.Status == 0)).ToListAsync();
            if (reports != null) 
            {
                foreach (var report in reports)
                {
                    report.UserNameAdminFix = UserName;
                    report.Status = 1;
                    await _databaseContext.SaveChangesAsync();
                    
                }
                return true;
            }
            return false;
        }
        

        public async Task<List<ReportToShowDTOs>> GetReportAdmin(string UserNameAdminFix)
        {

            var sql = @"
                SELECT 
                    r.""IdReport"",
                    r.""IdMovie"",
                    r.""IdComment"",
                    r.""UserNameReporter"",
                    r.""Content"",
                    r.""UserNameAdminFix"",
                    r.""Response"",
                    r.""TimeReport"",
                    r.""TimeResponse"",
                    r.""Status"",
                    c.""IdUserName"" AS ""NameOfUserReported"",
                    c.""Content"" AS ""ContentCommentReported""
                FROM ""Report"" r
                LEFT JOIN ""Comment"" c ON r.""IdComment"" = c.""IdComment""
                WHERE (r.""Status"" = 0 AND r.""Type"" = 'Report Hệ thống')
                    OR 
                    (r.""Status"" = 1 AND r.""UserNameAdminFix"" = @usernameadminfix AND r.""Type"" = 'Report Hệ thống');
                    ";
            return await _databaseContext.Database
        .SqlQueryRaw<ReportToShowDTOs>(sql, new[] { new NpgsqlParameter("@usernameadminfix", UserNameAdminFix) })
            .ToListAsync();
        }
        public async Task<List<ReportToShowDTOs>> GetNotificationAdmin(string UserNameAdminFix)
        {

            var sql = @"
                SELECT 
                    r.""IdReport"",
                    r.""IdMovie"",
                    r.""IdComment"",
                    r.""UserNameReporter"",
                    r.""Content"",
                    r.""UserNameAdminFix"",
                    r.""Response"",
                    r.""TimeReport"",
                    r.""TimeResponse"",
                    r.""Status"",
                    c.""IdUserName"" AS ""NameOfUserReported"",
                    c.""Content"" AS ""ContentCommentReported""
                FROM ""Report"" r
                LEFT JOIN ""Comment"" c ON r.""IdComment"" = c.""IdComment""
                WHERE (r.""Status"" = 0 AND r.""Type"" = 'Đổi Gmail')
                    OR 
                    (r.""Status"" = 1 AND r.""UserNameAdminFix"" = @usernameadminfix AND r.""Type"" = 'Đổi Gmail');
                    ";
            return await _databaseContext.Database
        .SqlQueryRaw<ReportToShowDTOs>(sql, new[] { new NpgsqlParameter("@usernameadminfix", UserNameAdminFix) })
            .ToListAsync();
        }
        public async Task<List<ReportToShowDTOs>> GetReportMovie(string UserNameAdminFix)
        {

            var sql = @"
                SELECT 
                    r.""IdReport"",
                    r.""IdMovie"",
                    r.""IdComment"",
                    r.""UserNameReporter"",
                    r.""Content"",
                    r.""UserNameAdminFix"",
                    r.""Response"",
                    r.""TimeReport"",
                    r.""TimeResponse"",
                    r.""Status"",
                    c.""IdUserName"" AS ""NameOfUserReported"",
                    c.""Content"" AS ""ContentCommentReported""
                FROM ""Report"" r
                LEFT JOIN ""Comment"" c ON r.""IdComment"" = c.""IdComment""
                WHERE (r.""Status"" = 0 AND r.""Type"" = 'Report Movie')
                    OR 
                    (r.""Status"" = 1 AND r.""UserNameAdminFix"" = @usernameadminfix AND r.""Type"" = 'Report Movie');
                    ";
            return await _databaseContext.Database
        .SqlQueryRaw<ReportToShowDTOs>(sql, new[] { new NpgsqlParameter("@usernameadminfix", UserNameAdminFix) })
            .ToListAsync();
        }
        public async Task<List<ReportToShowDTOs>> GetReportComment(string UserNameAdminFix)
        {

            var sql = @"
                SELECT 
                    r.""IdReport"",
                    r.""IdMovie"",
                    r.""IdComment"",
                    r.""UserNameReporter"",
                    r.""Content"",
                    r.""UserNameAdminFix"",
                    r.""Response"",
                    r.""TimeReport"",
                    r.""TimeResponse"",
                    r.""Status"",
                    c.""IdUserName"" AS ""NameOfUserReported"",
                    c.""Content"" AS ""ContentCommentReported""
                FROM ""Report"" r
                LEFT JOIN ""Comment"" c ON r.""IdComment"" = c.""IdComment""
                WHERE (r.""Status"" = 0 AND r.""Type"" = 'Report Comment')
                    OR 
                    (r.""Status"" = 1 AND r.""UserNameAdminFix"" = @usernameadminfix AND r.""Type"" = 'Report Comment');
                    ";
            return await _databaseContext.Database
        .SqlQueryRaw<ReportToShowDTOs>(sql, new[] { new NpgsqlParameter("@usernameadminfix", UserNameAdminFix) })
            .ToListAsync();
        }

        public async Task<List<ReportToShowDTOs>> GetSelfReport(string UserNameReporter)
        {
            return await _databaseContext.Reports
                .Where(r => r.UserNameReporter == UserNameReporter)
                .Select(r => new ReportToShowDTOs
                {
                    IdReport = r.IdReport,
                    IdMovie = r.IdMovie,
                    UserNameReporter = r.UserNameReporter,
                    Content = r.Content,
                    UserNameAdminFix = r.UserNameAdminFix,
                    Response = r.Response,
                    TimeReport = r.TimeReport,
                    TimeResponse = r.TimeResponse,
                    Status = r.Status
                })
                .ToListAsync();
        }



        public async Task<bool> ReceiveReport(string IdReport, string UserNameAdminFix)
        {
            if (IdReport != null || UserNameAdminFix != null)
            {
                var report = await _databaseContext.Reports.FirstOrDefaultAsync(r => r.IdReport == IdReport);
                if (report != null)
                {
                    if(report.Status == 0)
                    {
                        report.UserNameAdminFix = UserNameAdminFix;
                        report.Status = 1;
                        await _databaseContext.SaveChangesAsync();
                        return true;
                    }
                    
                }
            }
            return false;
        }

        public async Task<bool> ResponseReport(string UserNameAdminFix, ResponseReport responseReport)
        {
            if (responseReport.IdReport != null && UserNameAdminFix != null)
            {
                var report = await _databaseContext.Reports.FirstOrDefaultAsync(r => r.IdReport == responseReport.IdReport && r.UserNameAdminFix == UserNameAdminFix && r.Status == 1);
                if (report != null)
                {
                    report.Response = responseReport.Response;
                    report.TimeResponse = DateTimeHelper.GetdateTimeVNNow();
                    report.Status = 2;
                    await _databaseContext.SaveChangesAsync();
                    return true;
                }

            }
            return false;
        }

        public async Task<bool> UpReport(string UserNameReporter, UpReportDTOs upReportDTOs)
        {
            if (!string.IsNullOrEmpty(upReportDTOs.Content) && !string.Equals(upReportDTOs.Content, "string", StringComparison.OrdinalIgnoreCase))
            {
                Report report = new Report
                {
                    IdReport = Guid.NewGuid().ToString(),
                    UserNameReporter = UserNameReporter,
                    Content = upReportDTOs.Content,
                    Status = 0,
                    TimeReport = DateTimeHelper.GetdateTimeVNNow(),
                    Type =  "Report Hệ thống"
                };

                // Nếu có SlugMovie
                if (!string.IsNullOrWhiteSpace(upReportDTOs.SlugMovie) &&
                    !string.Equals(upReportDTOs.SlugMovie, "string", StringComparison.OrdinalIgnoreCase))
                {
                    var movie = await _databaseContext.Movies.FirstOrDefaultAsync(i => i.SlugTitle == upReportDTOs.SlugMovie);

                    if (movie != null)
                    {
                        report.IdMovie = movie.IdMovie;
                        report.Type = "Report Phim";
                    }
                    else
                    {
                        return false;
                    }
                }

                // Nếu có IdComment
                if (!string.IsNullOrWhiteSpace(upReportDTOs.IdComment) &&
                    !string.Equals(upReportDTOs.IdComment, "string", StringComparison.OrdinalIgnoreCase))
                {
                    var comment = await _databaseContext.Comments.FirstOrDefaultAsync(i => i.IdComment == upReportDTOs.IdComment);
                    if (comment != null)
                    {
                        if (UserNameReporter == comment.IdUserName)
                        {
                            throw new InvalidOperationException("Bạn không thể tự báo cáo chính mình.");
                        }
                        report.Type = "Report Comment";
                        report.IdComment = upReportDTOs.IdComment;
                    }
                }

                await _databaseContext.Reports.AddAsync(report);
                await _databaseContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        /*public async Task<bool> ResponseCommentReport(string IdComment, string UserName)
        {
            if (string.IsNullOrEmpty(IdComment) || string.IsNullOrEmpty(UserName)) return false;
            var reports = await _databaseContext.Reports.Where(i => i.IdComment == IdComment && i.UserNameAdminFix == UserName || i.Status == 1).ToListAsync();
            if (reports != null)
            {
                var comment = await _databaseContext.Comments.FirstOrDefaultAsync(i => i.IdComment == IdComment);
                var movie = await _databaseContext.Movies.FirstOrDefaultAsync(i => i.IdMovie == comment.IdMovie);
                foreach (var report in reports)
                {
                    
                    report.Status = 2;
                    report.TimeResponse = DateTimeHelper.GetdateTimeVNNow();
                    report.Response = "Đã xử lý Comment : " + comment.Content + " của phim : " + movie.Title + " .Cám ơn bạn đã báo cáo";
                }
                return true;
            }
            return false;
        }*/
        public async Task<bool> ResponseCommentReports(List<ResponseReport> reportResponses, string UserName)
        {
            if (reportResponses == null || !reportResponses.Any() || string.IsNullOrEmpty(UserName))
                return false;

            var reportIds = reportResponses.Select(r => r.IdReport).ToList();

            var reports = await _databaseContext.Reports
                .Where(r => reportIds.Contains(r.IdReport) && r.UserNameAdminFix == UserName && r.Status == 1)
                .ToListAsync();

            foreach (var report in reports)
            {
                var input = reportResponses.FirstOrDefault(r => r.IdReport == report.IdReport);
                if (input != null)
                {
                    var comment = await _databaseContext.Comments.FirstOrDefaultAsync(c => c.IdComment == report.IdComment);
                    var movie = await _databaseContext.Movies.FirstOrDefaultAsync(m => m.IdMovie == comment.IdMovie);

                    string defaultMessage = $"Đã xử lý Comment: {comment.Content} của phim: {movie.Title}. Cám ơn bạn đã báo cáo.";

                    report.Status = 2;
                    report.TimeResponse = DateTimeHelper.GetdateTimeVNNow();
                    report.Response = string.IsNullOrWhiteSpace(input.Response) ? defaultMessage : input.Response;
                }
            }

            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
