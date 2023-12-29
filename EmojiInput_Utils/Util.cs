#nullable enable

using System;

namespace EmojiInput_Utils;

public static class Util
{
    public static float Square(this float v) => v * v;

    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }

    public static int ToInt<T>(this T enumValue) where T : Enum
    {
        return Convert.ToInt32(enumValue);
    }
}