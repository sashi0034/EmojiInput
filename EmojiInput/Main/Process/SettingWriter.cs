#nullable enable

using System.Threading;
using System.Threading.Tasks;
using EmojiInput_Model;
using EmojiInput_Utils;

namespace EmojiInput.Main.Process;

public record SettingWriter(
    EmojiSettingModel SettingModel)
{
    private CancellationTokenSource _cancellation = new();

    public void RequestSave(CancellationToken cancel)
    {
        _cancellation.Cancel();
        _cancellation = new CancellationTokenSource();
        delayedWrite(cancel.LinkToken(_cancellation)).RunErrorHandler();
    }

    private async Task delayedWrite(CancellationToken cancel)
    {
        // 最後の操作から一定間隔が経ったら保存するようにする
        await Task.Delay(Consts.Enough_300 * 1000, cancel);
        SettingModel.RefreshSave();
    }
}