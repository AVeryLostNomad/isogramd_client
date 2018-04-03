using System;
using System.Collections.Generic;

namespace Isogramd.Util
{
    public class TextUtil
    {
        public static string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

		public static Boolean IsIsogram(string s)
		{
			Dictionary<char, int> charCount = new Dictionary<char, int>();
			foreach(char c in s.ToCharArray())
			{
				if (charCount.ContainsKey(c))
				{
					return false;
				}
				else
				{
					charCount[c] = 1;
				}
			}
			return true;
		}
    }
}