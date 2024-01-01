#nullable enable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EmojiInput_Model;
using EmojiInput.Main.Control;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record FocusCursorMover(
    EmojiFilteredModel FilteredModel,
    TextBlock SelectedDescription,
    EmojiIconCollection IconCollection,
    TextBox SearchTextBox,
    ScrollViewer ScrollViewer)
{
    public void Subscribe()
    {
        Point oldEnteredPos;
        for (var i = 0; i < IconCollection.ReservedImages.Count; i++)
        {
            int index = i;
            var image = IconCollection.ReservedImages[i];
            image.MouseEnter += ((_, e) =>
            {
                var newEnteredPos = e.MouseDevice.GetPosition(SearchTextBox);
                if (oldEnteredPos == newEnteredPos) return;
                oldEnteredPos = newEnteredPos;
                MoveCursor(index);
            });
        }
    }

    public void MoveCursor(int nextCursor)
    {
        IconCollection.LocateCursor(nextCursor);
        SelectedDescription.Text = nextCursor < FilteredModel.List.Count
            ? $"{FilteredModel.List[nextCursor].Description} ({FilteredModel.List[nextCursor].ConcatAliases()})"
            : "-";
        ScrollToCursor();
    }

    // いい感じにスクロール調整
    public void ScrollToCursor()
    {
        int newScroll = IconCollection.ImageSize * (IconCollection.Cursor / IconCollection.ColumnSize);
        double delta = newScroll - ScrollViewer.VerticalOffset;

        double viewportHeight = ScrollViewer.ViewportHeight;
        if (delta < 0)
        {
            ScrollViewer.ScrollToVerticalOffset(newScroll);
        }
        else if (viewportHeight - IconCollection.ImageSize < delta)
        {
            if (delta < viewportHeight)
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + IconCollection.ImageSize);
            else
                ScrollViewer.ScrollToVerticalOffset(newScroll);
        }
    }

    public void PreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Back)
        {
            focusSearchTextBox();
            return;
        }

        if (IconCollection.CurrentSize == 0) return;

        if (checkMoveCursorByKey(e.Key, out var nextCursor) == false) return;

        MoveCursor(nextCursor);
        IconCollection.Focus();
    }

    public void PreviewTextInput(TextCompositionEventArgs e)
    {
        if (e.Text.Length == 0) return;
        if (char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '_')
        {
            // 英数字かアンダースコアが入力されたら検索欄にフォーカス
            focusSearchTextBox();
        }
    }

    private bool checkMoveCursorByKey(Key key, out int nextCursor)
    {
        nextCursor = IconCollection.Cursor;
        switch (key)
        {
        case Key.Left:
            if (IconCollection.IsFocused == false) return false;
            nextCursor--;
            break;
        case Key.Right:
            if (IconCollection.IsFocused == false) return false;
            nextCursor++;
            break;
        case Key.Up:
            if (IconCollection.IsFocused == false) return true;
            nextCursor -= IconCollection.ColumnSize;
            break;
        case Key.Down:
            if (IconCollection.IsFocused == false) return true;
            nextCursor += IconCollection.ColumnSize;
            break;
        default:
            return false;
        }

        nextCursor = clampCursor(nextCursor);

        return true;
    }

    private int clampCursor(int nextCursor)
    {
        while (nextCursor < 0)
        {
            nextCursor += IconCollection.ColumnSize;
        }

        while (nextCursor >= IconCollection.CurrentSize)
        {
            nextCursor -= IconCollection.ColumnSize;
        }

        return Math.Max(0, nextCursor);
    }

    private void focusSearchTextBox()
    {
        MoveCursor(0);
        SearchTextBox.Focus();
    }
}