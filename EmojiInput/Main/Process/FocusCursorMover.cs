﻿#nullable enable

using System.Windows.Controls;
using System.Windows.Input;
using EmojiInput_Model;
using EmojiInput.Main.Control;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record FocusCursorMover(
    EmojiFilteredList FilteredList,
    TextBlock SelectedDescription,
    EmojiIconCollection IconCollection,
    TextBox SearchTextBox)
{
    public void MoveCursor(int nextCursor)
    {
        IconCollection.LocateCursor(nextCursor);
        SelectedDescription.Text = nextCursor < FilteredList.List.Count
            ? FilteredList.List[nextCursor].Description
            : "?";
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
            nextCursor -= EmojiIconCollection.ColumnSize;
            break;
        case Key.Down:
            nextCursor += EmojiIconCollection.ColumnSize;
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
            nextCursor += EmojiIconCollection.ColumnSize;
        }

        while (nextCursor >= IconCollection.CurrentSize)
        {
            nextCursor -= EmojiIconCollection.ColumnSize;
        }

        return nextCursor;
    }

    private void focusSearchTextBox()
    {
        MoveCursor(0);
        SearchTextBox.Focus();
    }
}