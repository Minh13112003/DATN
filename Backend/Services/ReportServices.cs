using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class ReportServices : IReportServices
    {
        private readonly IReportRepository _repository;
        public ReportServices(IReportRepository repository) {
            _repository = repository;
        }

        public async Task<bool> DeleteReport(string IdReport)
        {
            return await _repository.DeleteReport(IdReport);
        }

        public async Task<bool> ExecuteCommentReport(string IdComment, string UserName)
        {
            return await _repository.ExecuteCommentReport(IdComment, UserName);
        }

        public async Task<List<Report>> GetCommentReport(string IdComment, string UserName)
        {
            return await _repository.GetCommentReport(IdComment, UserName);
        }

        public async Task<List<ReportToShowDTOs>> GetNotificationAdmin(string UserNameAdminFix)
        {
            return await _repository.GetNotificationAdmin(UserNameAdminFix);
        }

        public async Task<List<ReportToShowDTOs>> GetReportAdmin(string UserNameAdminFix)
        {
            return await _repository.GetReportAdmin(UserNameAdminFix);
        }

        public async Task<List<ReportToShowDTOs>> GetReportComment(string UserNameAdminFix)
        {
            return await _repository.GetReportComment(UserNameAdminFix);
        }

        public async Task<List<ReportToShowDTOs>> GetReportMovie(string UserNameAdminFix)
        {
            return await _repository.GetReportMovie(UserNameAdminFix);
        }

        public async Task<List<ReportToShowDTOs>> GetSelfReport(string UserNameReporter)
        {
            return await _repository.GetSelfReport(UserNameReporter);
        }

        public async Task<bool> ReceiveReport(string IdReport, string UserNameAdminFix)
        {
            return await _repository.ReceiveReport(IdReport, UserNameAdminFix);
        }

        public async Task<bool> ResponseCommentReports(List<ResponseReport> reportResponses, string UserName)
        {
            return await _repository.ResponseCommentReports(reportResponses, UserName);
        }

        public async Task<bool> ResponseReport(string UserNameAdminFix, ResponseReport responseReport)
        {
            return await _repository.ResponseReport(UserNameAdminFix, responseReport);
        }

        public async Task<bool> UpReport(string UserNameReporter, UpReportDTOs upReportDTOs)
        {
            return await _repository.UpReport(UserNameReporter, upReportDTOs);
        }
        
    }
}
