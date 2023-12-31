#nullable enable

using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace EmojiInput.Main.Forward;

public class EmojiView
{
    public readonly int Index;
    public bool IsValid { get; private set; } = false;
    public BitmapImage Bitmap { get; private set; } = new();

    public EmojiView(int index)
    {
        Index = index;
        Bitmap.Freeze();
    }

    public void SetBitmap(BitmapImage bitmapImage)
    {
        IsValid = true;
        Bitmap = bitmapImage;
    }
}

public class EmojiViewList : List<EmojiView>
{
    public EmojiViewList(int length)
    {
        for (int i = 0; i < length; ++i)
        {
            this.Add(new EmojiView(i));
        }
    }
}