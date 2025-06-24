namespace DoAnTotNghiep.Services
{
    public interface IEmailAuthenticationServices
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
