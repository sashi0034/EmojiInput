#nullable enable

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmojiInput_Model;
using EmojiInput.Main.Detail;

namespace EmojiInput.Main;

public record EmojiFlushing(
    IconCollection IconCollection,
    EmojiViewList EmojiViewList)
{
    public async Task StartAsync(IEnumerable<EmojiData> filteredData, CancellationToken cancel)
    {
        int index = -1;
        foreach (var data in filteredData)
        {
            cancel.ThrowIfCancellationRequested();
            index++;
            var dataView = EmojiViewList[data.Index];
            while (dataView.IsValid == false)
            {
                // 画像が読み込まれるまで待機
                await Task.Delay(Consts.Large_100, cancel);
            }

            var fixedIndex = index;
            IconCollection.Dispatcher.Invoke(() => { IconCollection.ChangeSource(fixedIndex, dataView.Bitmap); });
        }
    }
}