#nullable enable

using System;

namespace EmojiInput_Model;

public enum EmojiIconSizeKind
{
    Small,
    Large,
}

public class EmojiIconSizeModel
{
    public EmojiIconSizeKind Kind { get; set; } = EmojiIconSizeKind.Small;
}