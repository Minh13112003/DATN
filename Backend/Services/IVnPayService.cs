using DoAnTotNghiep.Model.VnPayModel;

namespace DoAnTotNghiep.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentRequestModel model, HttpContext context, string UserName);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
