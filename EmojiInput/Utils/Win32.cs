#nullable enable

using System;
using System.Runtime.InteropServices;

namespace EmojiInput.Utils;

public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

public static class Win32
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("User32.dll")]
    public static extern uint GetDpiForWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);

    [DllImport("user32.dll")]
    public static extern int UnregisterHotKey(IntPtr hWnd, int id);
}