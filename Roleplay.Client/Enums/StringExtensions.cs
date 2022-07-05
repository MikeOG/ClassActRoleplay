using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Client.Enums
{
    public static class StringExtensions
    {
        public static string Truncate(this string data, int maxlength, bool returnsNulls = false)
        {
            return string.IsNullOrWhiteSpace(data) ? (returnsNulls ? (string)null : string.Empty) : (data.Length <= maxlength ? data : data.Substring(0, maxlength));
        }

        public static bool StartsWithVowel(this string str)
        {
            return ((IEnumerable<string>)new string[5]
            {
                "a",
                "e",
                "i",
                "o",
                "u"
            }).Any<string>((Func<string, bool>)(v => str.ToLower().StartsWith(v)));
        }

        public static string ToTitleCase(this string str)
        {
            string[] strArray = str.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < strArray.Length; ++index)
            {
                string str1 = strArray[index];
                strArray[index] = str1.Substring(0, 1).ToUpper() + str1.Substring(1).ToLower();
            }
            return string.Join(" ", strArray);
        }

        public static string AddSpacesToCamelCase(this string source)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in source)
            {
                if (char.IsUpper(c) && stringBuilder.Length > 0)
                    stringBuilder.Append(' ');
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        /*public static string RemoveSpaces(this string source)
        {
            return source.Replace(" ", "");
        }*/

        public static string FirstLetterToUpper(this string source)
        {
            if (source == null)
                return (string)null;
            return source.Length > 1 ? char.ToUpper(source[0]).ToString() + source.Substring(1) : source.ToUpper();
        }

        public static IEnumerable<string> SplitToLengths(this string s, int len)
        {
            for (int i = 0; i < s.Length; i += len)
                yield return s.Substring(i, Math.Min(len, s.Length - i));
        }

        public static string BytesToString(byte[] buffer)
        {
            string str = "";
            for (int index = 0; index < buffer.Length; ++index)
                str += ((char)buffer[index]).ToString();
            return str;
        }

        public static byte[] StringToBytes(string data)
        {
            byte[] numArray = new byte[data.Length];
            for (int index = 0; index < data.Length; ++index)
                numArray[index] = (byte)data[index];
            return numArray;
        }
    }
}
