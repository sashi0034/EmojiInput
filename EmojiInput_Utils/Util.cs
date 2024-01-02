#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

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

    public static T ToEnum<T>(this int value) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        var forEach = source as T[] ?? source.ToArray();
        foreach (T obj in forEach)
            action(obj);
        return forEach;
    }

    public static IEnumerable<T> ForEachIndexed<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        int num = 0;
        var forEach = source as T[] ?? source.ToArray();
        foreach (T obj in forEach)
            action(obj, num++);
        return forEach;
    }

    public static List<T> Clone<T>(this IEnumerable<T> source)
    {
        return new List<T>(source);
    }

    public static string? GetCurrentExecutingDir()
    {
        return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}