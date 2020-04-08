using System.Text;

public class StringUtils
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
}