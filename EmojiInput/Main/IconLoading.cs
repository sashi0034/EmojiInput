#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using EmojiInput_Model;
using EmojiInput.Main.Detail;

namespace EmojiInput.Main;

public record IconLoading(
    EmojiDatabase EmojiDatabase,
    Dispatcher Dispatcher,
    IconCollection IconCollection)
{
    public async Task StartAsync(CancellationToken cancel)
    {
        var bitmapImages = new List<BitmapImage>();

        foreach (var emoji in EmojiDatabase)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                var iconPath = "Resource/emoji_icon/" + emoji.ImageFilename;
                bitmap.UriSource = new Uri(iconPath, UriKind.Relative);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                bitmapImages.Add(bitmap);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                continue;
            }

            if (bitmapImages.Count >= Consts.Large_100) await flushImagesAsync(bitmapImages, cancel);
        }

        await flushImagesAsync(bitmapImages, cancel);
    }

    private async Task flushImagesAsync(List<BitmapImage> bitmapImages, CancellationToken cancel)
    {
        bool ok = false;
        Dispatcher.Invoke(() =>
        {
            foreach (var src in bitmapImages)
            {
                var image = new Image
                {
                    Width = 64,
                    Source = src
                };
                IconCollection.Add(image);
            }

            bitmapImages.Clear();
            ok = true;
        });
        while (true)
        {
            await Task.Delay(Consts.Large_100, cancel);
            if (ok) break;
        }
    }
}