using System.Windows;
using System.Windows.Controls;

namespace EmojiInput.Main.Detail;

public partial class IconCollection : UserControl
{
    public IconCollection()
    {
        InitializeComponent();
    }

    public void Add(UIElement uiElement)
    {
        stackPanel.Children.Add(uiElement);
    }
}