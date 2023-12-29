#nullable enable

using System.Windows;

namespace EmojiInput.Utils;

public static class ControlUtil
{
    public static double GetWindowScaling(Window window)
    {
        PresentationSource? source = PresentationSource.FromVisual(window);

        return source?.CompositionTarget != null
            ? source.CompositionTarget.TransformToDevice.M11 // M11 gives the horizontal DPI scaling factor
            : 1.0; // Default scale factor is 1.0 (100%)
    }
}