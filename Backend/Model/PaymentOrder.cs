
using DoAnTotNghiep.Helper.DateTimeVietNam;

namespace DoAnTotNghiep.Model
{
    public class PaymentOrder
    {
        public string IdPaymentOrder { get; set; }
        public string UserName { get; set; }
        public long OrderCode {  get; set; }
        public string? TransactionId { get; set; } = string.Empty;
        public string Item {  get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTimeHelper.GetDateTimeVnNowWithDateTime();
    }
}
