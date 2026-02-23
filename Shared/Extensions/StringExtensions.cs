using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class StringExtensions
    {
        public static string? NormalizeArabicText(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    char firstChar = words[i][0]; 

                    if (firstChar == 'أ' || firstChar == 'إ' || firstChar == 'آ')
                    {
                        words[i] = "ا" + words[i].Substring(1);
                    }
                }
            }
            string result = string.Join(" ", words);
            text = result.Replace("ة", "ه");

            return text;
        }

    }
}
