#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EmojiInput_Model;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record EmojiLoadProcess(
    EmojiDatabase EmojiDatabase,
    EmojiViewList ViewList)
{
    public async Task StartAsync(CancellationToken cancel)
    {
        await Task.Run(() => { process(cancel); }, cancel);
    }

    private void process(CancellationToken cancel)
    {
        for (int index = 0; index < EmojiDatabase.Count; index++)
        {
            cancel.ThrowIfCancellationRequested();
            var emoji = EmojiDatabase[index];
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                var iconPath = "Resource/emoji_icon/" + emoji.ImageFilename;
                bitmap.UriSource = new Uri(iconPath, UriKind.Relative);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                ViewList[index].SetBitmap(bitmap);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}