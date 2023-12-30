#nullable enable

using System.Globalization;
using System.Text;

namespace EmojiInput_Utils;

public static class UnicodeUtil
{
    public static string UnicodeToCharacter(string unicode)
    {
        int code = int.Parse(unicode, System.Globalization.NumberStyles.HexNumber);
        return char.ConvertFromUtf32(code);
    }

    public static string CharacterToUtf32(string c)
    {
        var sb = new StringBuilder();
        var textElementEnumerator = StringInfo.GetTextElementEnumerator(c);
        while (textElementEnumerator.MoveNext())
        {
            string textElement = textElementEnumerator.GetTextElement();
            int codePoint = char.ConvertToUtf32(textElement, 0);
            if (sb.Length > 0) sb.Append('_');
            sb.Append($"u{codePoint:X4}");
        }

        return sb.ToString();
    }

    public static string CharacterToUtf16(string c)
    {
        var sb = new StringBuilder();
        foreach (char ch in c)
        {
            if (sb.Length > 0) sb.Append('_');
            sb.Append($"u{(int)ch:X}");
        }

        return sb.ToString();
    }
}