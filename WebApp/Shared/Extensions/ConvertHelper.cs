using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Shared.Extensions
{
    public static class ConvertHelper
    {
        public static string ConvertUnicode(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var res = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            res = res.Replace("đ", "d");
            res = res.Replace("Đ", "D");

            return res;
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return string.Empty;
        }
    }
}
