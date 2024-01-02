#nullable enable

using EmojiInput.Main.Control;

namespace EmojiInput.Main.Process;

public record HistoryPresenter(
    EmojiHistoryDropdown HistoryDropdown)
{
    public void Subscribe()
    {
        HistoryDropdown.OnDropdownStarted += () => { };
    }
}