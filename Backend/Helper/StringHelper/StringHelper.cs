using System.Text;

namespace DoAnTotNghiep.Helper.StringHelper
{
    public class StringHelper
    {
        public static string GenerateRandomUppercaseString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var result = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString(); // Ví dụ: "XKJHDU"
        }
    }
}
