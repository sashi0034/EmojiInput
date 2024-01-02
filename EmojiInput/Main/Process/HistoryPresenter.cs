#nullable enable

using System.Linq;
using System.Windows;
using EmojiInput_Model;
using EmojiInput.Main.Control;
using EmojiInput.Main.Forward;

namespace EmojiInput.Main.Process;

public record HistoryPresenter(
    EmojiDatabase Database,
    EmojiSettingPrimitiveData SettingData,
    EmojiViewList EmojiViewList,
    EmojiHistoryDropdown HistoryDropdown)
{
    public void Subscribe()
    {
        HistoryDropdown.OnDropdownStarted += onDropdownStarted;
    }

    private void onDropdownStarted()
    {
        for (var index = 0; index < HistoryDropdown.Images.Count; index++)
        {
            var image = HistoryDropdown.Images[index];
            var history = SettingData.History.Array[index];
            if (history == "")
            {
                image.Visibility = Visibility.Hidden;
            }
            else
            {
                var historyEmoji = Database.FirstOrDefault(e => e.Aliases[0] == history);
                if (historyEmoji == null) continue;
                image.Visibility = Visibility.Visible;
                image.Source = EmojiViewList[historyEmoji.Index].Bitmap;
                HistoryDropdown.Buttons[index].ToolTip = historyEmoji.Description;
            }
        }
    }
}