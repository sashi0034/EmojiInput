#nullable enable

using EmojiInput_Model;

namespace EmojiInput.Main.Forward;

public static class EmojiUtils
{
    public static string GetIconFilename(EmojiData emoji, string skin)
    {
        var skinSuffix = emoji.HasSkinTones && skin != ""
            ? "_" + skin
            : "";
        return $"{emoji.Aliases[0]}{skinSuffix}";
    }

    public const string IconDirectory = "Resource/emoji_icon/aliased/";

    public static string GetIconPath(EmojiData emoji, string skin)
    {
        return $"{IconDirectory}{GetIconFilename(emoji, skin)}.png";
    }
}