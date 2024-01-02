#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmojiInput_Model;
using EmojiInput_Utils;

namespace EmojiInput.Main.Process;

public record EmojiSendProcess(
    // Window Window,
    EmojiSettingModel SettingModel,
    EmojiSkinData SkinData
)
{
    private IntPtr _lastForegroundWindow;

    private bool _isSending;
    private readonly List<EmojiData> _sendingQueue = new();

    public void RegisterForegroundWindow()
    {
        _lastForegroundWindow = Win32.GetForegroundWindow();
    }

    public async Task StartAsync(EmojiData selectedEmoji, CancellationToken cancel)
    {
        lock (_sendingQueue)
        {
            if (_sendingQueue.Count == 0) _isSending = false;
            _sendingQueue.Add(selectedEmoji);
        }

        // 1つだけプロセスが動くようにする
        if (_isSending) return;

        _isSending = true;
        await process(cancel);
        _isSending = false;
    }

    private async Task process(CancellationToken cancel)
    {
        // キューがなくなるまで送信処理
        var sourceWindow = Win32.GetForegroundWindow();
        while (true)
        {
            EmojiData? selectedEmoji = null;
            lock (_sendingQueue)
            {
                if (_sendingQueue.Count > 0)
                {
                    selectedEmoji = _sendingQueue[0];
                    _sendingQueue.RemoveAt(0);
                }
            }

            if (selectedEmoji == null) break;

            // アクティブウィンドウを切り替えて
            Win32.SetForegroundWindow(_lastForegroundWindow);

            // 送信処理
            System.Windows.Forms.SendKeys.SendWait(selectedEmoji.HasSkinTones && SettingModel.SkinKey != ""
                ? SkinData.GetSkinEmoji(selectedEmoji.Aliases[0], SettingModel.SkinKey)
                : selectedEmoji.EmojiCharacter);

            // 受理されてなさそうな時があるので一応待機
            // await Task.Delay(Consts.Enough_100, cancel);

            cancel.ThrowIfCancellationRequested();
        }

        Win32.SetForegroundWindow(sourceWindow);
    }
}