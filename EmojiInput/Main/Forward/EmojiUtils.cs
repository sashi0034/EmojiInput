#nullable enable

namespace EmojiInput.Main.Forward;

public static class EmojiUtils
{
    public static string GetSkinIconPath(string aliases, string skin)
    {
        var skinSuffix = skin != ""
            ? "_" + skin
            : "";
        return $"Resource/emoji_icon/aliased/{aliases}{skinSuffix}.png";
    }

    public static string GetIconPath(string aliases)
    {
        return GetSkinIconPath(aliases, "");
    }
}