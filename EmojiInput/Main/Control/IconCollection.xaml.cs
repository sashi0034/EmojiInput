using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace EmojiInput.Main.Control;

public partial class IconCollection : UserControl
{
    public const int ColumnSize = 10;

    private readonly List<Image> _reservedImaged = new();

    public IconCollection()
    {
        InitializeComponent();

        for (int i = 0; i < ColumnSize; ++i)
        {
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
        }

        pushGridRow(rootGrid);
    }

    public void Reserve(int length)
    {
        while (_reservedImaged.Count < length)
        {
            var newImage = new Image()
            {
                Width = 64
            };
            appendElement(newImage);
        }
    }

    public void Resize(int length)
    {
        for (int i = 0; i < length; ++i)
        {
            _reservedImaged[i].Visibility = Visibility.Visible;
        }

        for (int i = length; i < _reservedImaged.Count; ++i)
        {
            _reservedImaged[i].Visibility = Visibility.Collapsed;
        }
    }

    public void ChangeSource(int index, BitmapImage image)
    {
        _reservedImaged[index].Source = image;
    }

    public void LocateCursor(int index)
    {
        Grid.SetColumn(cursorBorder, index % ColumnSize);
        Grid.SetRow(cursorBorder, index / ColumnSize);
    }

    private void appendElement(Image uiElement)
    {
        int c = _reservedImaged.Count % ColumnSize;
        int r = _reservedImaged.Count / ColumnSize;

        if (r >= rootGrid.RowDefinitions.Count)
        {
            // 行が足りないから増やす
            pushGridRow(rootGrid);
        }

        Grid.SetColumn(uiElement, c);
        Grid.SetRow(uiElement, r);
        rootGrid.Children.Add(uiElement);

        // キャッシュにも格納
        _reservedImaged.Add(uiElement);
    }

    private static void pushGridRow(Grid parent)
    {
        parent.RowDefinitions.Add(new RowDefinition()
        {
            Height = new GridLength(1, GridUnitType.Star)
        });
    }
}