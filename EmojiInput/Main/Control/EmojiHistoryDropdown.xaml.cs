using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EmojiInput_Model;
using ModernWpf.Controls;

namespace EmojiInput.Main.Control;

public partial class EmojiHistoryDropdown : UserControl
{
    public event Action? OnDropdownStarted;
    public event Action<int>? OnDButtonClicked;

    private readonly List<Button> _buttons = new();
    public IReadOnlyList<Button> Buttons => _buttons;

    private readonly List<Image> _images = new();
    public IReadOnlyList<Image> Images => _images;

    public EmojiHistoryDropdown()
    {
        InitializeComponent();

        for (int i = 0; i < EmojiHistoryArray.Size; ++i)
        {
            var button = new Button();
            button.Click += button_Click;
            _buttons.Add(button);

            var image = new Image();
            button.Content = image;
            _images.Add(image);

            uniformGrid.Children.Add(button);
        }
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        int index = _buttons.IndexOf(button);
        OnDButtonClicked?.Invoke(index);
    }

    private void splitButton_OnClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        OnDropdownStarted?.Invoke();
    }
}