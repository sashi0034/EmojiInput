#nullable enable

using System;

namespace EmojiInput_Model;

public enum EmojiIconSizeKind
{
    Small,
    Large,
}

public class EmojiSettingModel
{
    public string SkinKey { get; set; } = "";

    public EmojiIconSizeKind IconSize { get; set; } = EmojiIconSizeKind.Small;
}