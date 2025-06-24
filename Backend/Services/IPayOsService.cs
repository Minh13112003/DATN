using DoAnTotNghiep.Model;
using Net.payOS.Types;

namespace DoAnTotNghiep.Services
{
    public interface IPayOsService
    {
        Task<CreatePaymentResult> CreatePaymentLink(int type, string UserName);
        Task<bool> HandleWebhook(string requestBody, string receivedSignature);
        Task<List<PaymentOrder>> GetPaymentOrders(string UserName);
        Task<PaymentLinkInformation> GetDetailPaymentOrders(string UserName, long? OrderCode);
        Task<bool> CancelPaymentOrder(long Ordercode);
    }
}
