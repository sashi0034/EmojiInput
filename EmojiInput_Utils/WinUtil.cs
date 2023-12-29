#nullable enable

namespace EmojiInput_Utils;

public static class WinUtil
{
    public static RECT GetActiveWindowRect()
    {
        IntPtr hwnd = Win32.GetForegroundWindow();
        if (hwnd == IntPtr.Zero)
        {
            return new RECT();
        }

        return Win32.GetWindowRect(hwnd, out RECT rect) ? rect : new RECT();
    }

    public static float GetActiveWindowScaling()
    {
        IntPtr hWnd = Win32.GetForegroundWindow();
        return Win32.GetDpiForWindow(hWnd) / 96.0f;
    }
}