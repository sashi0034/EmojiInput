#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record EmojiLoadProcess(
    EmojiDatabase EmojiDatabase,
    EmojiSettingModel SettingModel,
    EmojiViewList ViewList)
{
    private CancellationTokenSource _cancellation = new();

    private Dictionary<string, BitmapImage> _skinIconCache = new();

    /// <summary>
    /// 画像ファイルを読み込み、BitmapImageに変換する 
    /// </summary>
    public async Task StartAsync(CancellationToken cancel)
    {
        _cancellation.Cancel();
        _cancellation = new CancellationTokenSource();
        await Task.Run(() => { process(cancel); }, cancel.LinkToken(_cancellation));
    }

    private void process(CancellationToken cancel)
    {
        // 先にすべて無効にしておく
        for (int index = 0; index < EmojiDatabase.Count; index++)
        {
            ViewList[index].IsValid = false;
        }

        for (int index = 0; index < EmojiDatabase.Count; index++)
        {
            cancel.ThrowIfCancellationRequested();
            var emoji = EmojiDatabase[index];
            var iconPath = emoji.HasSkinTones
                ? EmojiUtils.GetSkinIconPath(emoji.Aliases[0], SettingModel.SkinKey)
                : EmojiUtils.GetIconPath(emoji.Aliases[0]);

            var iconUri = new Uri(iconPath, UriKind.Relative);
            if (ViewList[index].Bitmap.UriSource == iconUri)
            {
                // すでに読み込み済み
                ViewList[index].IsValid = true;
                continue;
            }

            try
            {
                // 読み込み
                var bitmap = emoji.HasSkinTones ? inquireSkinIconCache(iconUri) : loadBitmap(iconUri);
                ViewList[index].Bitmap = bitmap;
                ViewList[index].IsValid = true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }

    private BitmapImage inquireSkinIconCache(Uri iconUri)
    {
        if (_skinIconCache.TryGetValue(iconUri.OriginalString, out var hit)) return hit;

        var loaded = loadBitmap(iconUri);
        _skinIconCache[iconUri.OriginalString] = loaded;
        return loaded;
    }

    private static BitmapImage loadBitmap(Uri iconUri)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = iconUri;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}