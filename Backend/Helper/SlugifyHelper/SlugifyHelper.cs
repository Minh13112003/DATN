using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DoAnTotNghiep.Helper.SlugifyHelper
{
    
    public static class SlugHelper
    {
        public static string Slugify(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            // Chuyển Unicode về ASCII
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            string asciiText = sb.ToString().Normalize(NormalizationForm.FormC);

            // Chuyển thành chữ thường
            asciiText = asciiText.ToLowerInvariant();

            asciiText = asciiText.Replace("đ", "d").Replace("Đ", "d");

            // Thay thế các ký tự không phải chữ cái hoặc số bằng dấu "-"
            asciiText = Regex.Replace(asciiText, @"[^a-z0-9đĐ\s-]", "");

            // Thay thế dấu cách bằng dấu "-"
            asciiText = Regex.Replace(asciiText, @"\s+", "-").Trim('-');

            return asciiText;
        }

        public static string Unslugify(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Chuyển dấu "-" về khoảng trắng
            return text.Replace("-", " ");
        }
        public static string GenerateSlug(string input)
        {
            // Chuyển về chữ thường
            input = input.ToLowerInvariant();

            // Loại bỏ dấu tiếng Việt hoàn toàn
            input = RemoveVietnameseAccents(input);

            // Thay khoảng trắng bằng dấu "-"
            input = Regex.Replace(input, @"\s+", "-");

            // Giữ dấu "," nhưng loại bỏ các ký tự không hợp lệ
            input = Regex.Replace(input, @"[^a-z0-9,-]", "");

            return input;
        }

        public static string RemoveVietnameseAccents(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark && uc != UnicodeCategory.LowercaseLetter && uc != UnicodeCategory.UppercaseLetter)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }

}
