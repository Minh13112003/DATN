using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;

namespace DoAnTotNghiep.Repository
{
    public interface IReportRepository
    {
        Task<bool> UpReport(string UserNameReporter,UpReportDTOs upReportDTOs);
        Task<List<ReportToShowDTOs>> GetSelfReport(string UserNameReporter);
        Task<List<ReportToShowDTOs>> GetReportAdmin(string UserNameAdminFix);
        Task<bool> ReceiveReport(string IdReport, string UserNameAdminFix);
        Task<bool> ResponseReport(string UserNameAdminFix, ResponseReport responseReport);
        Task<bool> DeleteReport(string IdReport);
        Task<List<Report>> GetCommentReport(string IdComment, string UserName);
        /*Task<bool> ResponseCommentReport(string IdComment, string UserName);*/
        Task<bool> ExecuteCommentReport(string IdComment, string UserName);
        
        Task<bool> ResponseCommentReports(List<ResponseReport> reportResponses, string UserName);
        Task<List<ReportToShowDTOs>> GetReportMovie(string UserNameAdminFix);
        Task<List<ReportToShowDTOs>> GetReportComment(string UserNameAdminFix);
        Task<List<ReportToShowDTOs>> GetNotificationAdmin(string UserNameAdminFix);
    }
}
