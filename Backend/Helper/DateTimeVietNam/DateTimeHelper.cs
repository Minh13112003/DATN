namespace DoAnTotNghiep.Helper.DateTimeVietNam
{
    public class DateTimeHelper
    {
        public static string GetdateTimeVNNow()
        {
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);

            string formattedTime = vietnamTime.ToString("dd/MM/yyyy HH:mm:ss");

            return formattedTime;
        }
        public static DateTime GetDateTimeVnNowWithDateTime()
        {
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            return vietnamTime;
        }
        public static DateTime GetDateTimeVnNowWithDateTimeUTC()
        {
            TimeZoneInfo vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            DateTime utcTime = vietnamTime.ToUniversalTime(); // Chuyển về UTC
            return DateTime.SpecifyKind(utcTime, DateTimeKind.Utc); // Đảm bảo Kind=UTC
        }
        public static string ToStringDateTime(DateTime vietnamTime)
        {
            return vietnamTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static int CalculateAge(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age)) age--;  // nếu chưa đến sinh nhật trong năm nay thì trừ 1
            return age;
        }
    }
}
