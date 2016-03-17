using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Zamov.Helpers
{
    public static class TransliteHelper
    {
        static readonly List<Char> Cyr = new List<Char>(new Char[] {'а','б','в',
            'г','д','е','ё','ж','з','и','й','к','л','м','н','о','п','р','с',
            'т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я' });
        static readonly List<String> Lat = new List<string>(new string[] {"a","b","v",
            "g","d","e","jo","zh","z","i","j","k","l","m","n","o","p","r","s",
            "t","u","f","h","c","ch","sh","w","#","y","","je","ju","ja" });
        
        public static string TransliterateString(string input)
        {
            var builder = new StringBuilder();
            foreach (char c in input)
            {
                int index = Cyr.IndexOf(char.ToLower(c));
                switch (char.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.UppercaseLetter:
                        if (index > -1)
                        {
                            string translitChars = Lat[index];
                            builder.Append(char.ToUpper(translitChars[0]));
                            if (translitChars.Length > 1)
                                builder.Append(translitChars.Substring(1));
                        }
                        else
                            builder.Append(char.ToUpper(c));
                        break;
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.OtherLetter:
                        if (index > -1)
                            builder.Append(Lat[index]);
                        else
                            builder.Append(c);
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }
    }
}
