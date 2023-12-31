﻿#nullable enable

using System.Collections.Generic;
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

    public async Task StartAsync(IReadOnlyList<EmojiData> filteredData, CancellationToken cancel)
    {
        _cancellation.Cancel();
        _cancellation = new CancellationTokenSource();
        await process(filteredData, cancel.LinkToken(_cancellation));
    }

    private async Task process(IReadOnlyList<EmojiData> filteredData, CancellationToken cancel)
    {
        IconCollection.Resize(filteredData.Count);

        int index = -1;
        int queued = 0;
        foreach (var data in filteredData)
        {
            cancel.ThrowIfCancellationRequested();
            index++;
            var dataView = EmojiViewList[data.Index];
            while (dataView.IsValid == false || queued >= Consts.Enough_100)
            {
                // 画像が読み込まれるまで待機
                queued = 0;
                await Task.Delay(Consts.Enough_2000, cancel);
            }

            var fixedIndex = index;
            queued++;
            IconCollection.Dispatcher.Invoke(() =>
            {
                IconCollection.ChangeSource(fixedIndex, dataView.Bitmap);
            });
        }
    }
}