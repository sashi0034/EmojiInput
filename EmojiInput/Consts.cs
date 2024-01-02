#nullable enable

using EmojiInput_Utils;

namespace EmojiInput;

public static class Consts
{
    public const string AppProductName = "EmojiInput";

    public const string AppExecutiveFileName = "EmojiInput.exe";

    public const int HotKeyId_1 = 1;

    public const int MessageHotKey_0x0312 = 0x0312;

    public const int Enough_100 = 100;

    public const int Enough_250 = 250;

    public const int Enough_300 = 300;

    public const int Enough_500 = 500;

    public const int Enough_1000 = 500;

    public const int Enough_2000 = 2000;

    public static string GetCurrentExecutingPath()
    {
        return Util.GetCurrentExecutingDir() + @"\" + Consts.AppExecutiveFileName;
    }
}