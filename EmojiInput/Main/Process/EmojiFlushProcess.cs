#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmojiInput_Model;
using EmojiInput_Utils;
using EmojiInput.Main.Control;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record EmojiFlushProcess(
    EmojiIconCollection IconCollection,
    EmojiViewList EmojiViewList)
{
    private CancellationTokenSource _cancellation = new();

    /// <summary>
    /// 読み込まれたBitmapImageを画面上へ反映する 
    /// </summary>
    public async Task StartAsync(IReadOnlyList<EmojiData> filteredData, CancellationToken cancel)
    {
        _cancellation.Cancel();
        _cancellation = new CancellationTokenSource();
        await process(filteredData, cancel.LinkToken(_cancellation));
    }

    private async Task process(IReadOnlyList<EmojiData> filteredData, CancellationToken cancel)
    {
        IconCollection.Resize(filteredData.Count);
        var oldSources = IconCollection.ReservedImages
            .Take(filteredData.Count)
            .Select(r => r.IsVisible ? r.Source : null)
            .ToList();

        int index = -1;
        int queued = 0;
        foreach (var data in filteredData)
        {
            cancel.ThrowIfCancellationRequested();
            index++;
            var dataView = EmojiViewList[data.Index];
            while (dataView.IsValid == false || queued >= Consts.Enough_250)
            {
                // 画像が読み込まれるまで待機
                queued = 0;
                await Task.Delay(Consts.Enough_500, cancel);
            }

            if (dataView.IsValid && oldSources[index] == dataView.Bitmap)
            {
                // 対象を変える必要がない
                continue;
            }

            var fixedIndex = index;
            queued++;
            IconCollection.Dispatcher.Invoke(() =>
            {
                cancel.ThrowIfCancellationRequested();
                IconCollection.ChangeSource(fixedIndex, dataView.Bitmap);
            });
        }
    }
}