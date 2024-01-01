#nullable enable

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EmojiInput_Model;
using EmojiInput_Utils;
using ModernWpf.Controls;

namespace EmojiInput.Main.Process;

public record MenuSetup(
    EmojiSkinData SkinData,
    EmojiSettingModel SettingModel,
    MenuItem SkinMenu,
    Image SelectedSkinImage,
    MenuItem IconSizeMenu)
{
    public void Invoke()
    {
        // 肌の色
        foreach (var skin in SkinData.Keys.Prepend(""))
        {
            bool isChecked = skin == SettingModel.SkinKey;
            var path = $"/Resource/emoji_icon/aliased/hand{(skin == "" ? "" : $"_{skin}")}.png";
            var skinImage = new Image()
            {
                Source = new BitmapImage(new Uri(path, UriKind.Relative)),
                Width = 20,
            };
            if (isChecked) SelectedSkinImage.Source = skinImage.Source;
            SkinMenu.Items.Add(new RadioMenuItem
            {
                Header = skinImage,
                IsChecked = isChecked
            });
        }

        if (SkinMenu.Items[0] is Image image) SelectedSkinImage.Source = image.Source;

        // アイコンサイズ
        if (IconSizeMenu.Items[SettingModel.IconSize.ToInt()] is RadioMenuItem checkingIconSize)
            checkingIconSize.IsChecked = true;
    }
}