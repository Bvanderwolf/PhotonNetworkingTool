namespace Utils
{
    using System.Text;
    using UnityEngine;

    public static class StringUtils
    {
        /// <summary>
        /// Returns given string with white spaces between
        /// words with upper characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string AddWhiteSpaceAtUppers(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return "";

            StringBuilder newText = new StringBuilder(s.Length * 2);
            newText.Append(s[0]);
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]) && s[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(s[i]);
            }

            return newText.ToString();
        }

        public static void Print(params object[] a)
        {
            if (a != null && a.Length != 0)
            {
                string log = "";
                foreach (var s in a)
                {
                    log += $"{s.ToString()} | ";
                }
                Debug.Log(log);
            }
        }
    }
}