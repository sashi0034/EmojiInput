#nullable enable

using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EmojiInput_Utils;
using EmojiInput.Utils;

namespace EmojiInput.Main.Process;

public record PopupProcess(
    Window Window,
    TextBox searchTextBox)
{
    public async Task StartAsync(CancellationToken cancel)
    {
        if (Window.IsActive) return;
        Window.Dispatcher.Invoke(() =>
        {
            Window.WindowStyle = WindowStyle.None;
            popupOnActiveWindow();
            searchTextBox.Focus();
            searchTextBox.SelectAll();
            // _focusCursorMover.MoveCursor(0);
        });

        await Task.Delay(1, cancel);

        Window.Dispatcher.Invoke(() => { Window.WindowStyle = WindowStyle.SingleBorderWindow; });
    }

    private void popupOnActiveWindow()
    {
        var rect = WinUtil.GetActiveWindowRect();
        var center = new Vector2(rect.Left + rect.Right, rect.Top + rect.Bottom) / 2;
        var tl = center / (float)ControlUtil.GetWindowScaling(Window);
        float activeScaling = WinUtil.GetActiveWindowScaling();
        if (activeScaling == 0) return;

        Window.Left = tl.X - (Window.Width / activeScaling) / 2;
        Window.Top = tl.Y - (Window.Height / activeScaling) / 2;
        // Window.Topmost = true;
        Window.Activate();
        Window.Show();
    }
}